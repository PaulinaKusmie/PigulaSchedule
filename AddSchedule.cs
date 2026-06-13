
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.PlatformConfiguration;
using PigulaSchedule.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace PigulaSchedule
{
    public partial  class AddSchedule
    {
        private string ocrResult = string.Empty;

        string dbPath = System.IO.Path.Combine(
            FileSystem.AppDataDirectory,
            "pigulaApp.db3");



        public async Task AddScheduleAsync()
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo == null)
                return;

            using var stream = await photo.OpenReadAsync();

            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            ocrResult = await RecognizeWithGemini(ms.ToArray());

            var resultSchedule = ScheduleParser.Parse(ocrResult);

            await SaveData(resultSchedule);
        }


        private async Task<string> RecognizeWithGemini(byte[] imageBytes)
        {
            using var client = new HttpClient();
            var base64 = Convert.ToBase64String(imageBytes);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "image/jpeg",
                                    data = base64
                                }
                            },
                            new
                            {
                                text = @"To jest grafik pracy na jeden miesiąc. 
                                        W pierwszym wierszu są daty (np. 01.05, 02.05...).
                                        W trzecim wierszu są zmiany: ED, EDn, EN, ENn, W lub puste pole.
                                        Traktuj EDn tak samo jak ED, i ENn tak samo jak EN.
                                        Dla każdej daty wypisz zmianę w formacie DATA|ZMIANA.
                                        Jeśli pole jest puste wpisz DW.
                                        Jeśli pole ma W wpisz W.
                                        Wypisz TYLKO pary data|zmiana, bez żadnego dodatkowego tekstu.
                                        Przykład:
                                        01.05|ED
                                        02.05|EN
                                        03.05|DW
                                        04.05|W"
                            }
                        }
                    }
                }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var apiKey = Constans.APIKey;
            var response = await client.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}",
                content);

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "";
        }




        private async System.Threading.Tasks.Task SaveData(List<ShiftDay> jsonPath)
        {
            var database = new SQLiteAsyncConnection(dbPath);

            if (jsonPath == null)
                return;

            await database.CreateTableAsync<ShiftDay>();

            await database.InsertAllAsync(jsonPath);
        }


        public async System.Threading.Tasks.Task DeleteData()
        {
            var database = new SQLiteAsyncConnection(dbPath);
            await database.DeleteAllAsync<ShiftDay>();
        }
    }
}


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
using Application = Microsoft.Maui.Controls.Application;


namespace PigulaSchedule
{
    public partial  class AddSchedule
    {
        private string ocrResult = string.Empty;
        

        string dbPath = System.IO.Path.Combine(
            FileSystem.AppDataDirectory,
            "pigulaApp.db3");

        [Obsolete]
        public async Task AddScheduleAsync()
        {
            string action = await Shell.Current.DisplayActionSheet(
            "Wybierz źródło zdjęcia",
            "Anuluj",
            null,
            "Zrób zdjęcie", "Wybierz z galerii");

            FileResult? photo = action switch
            {
                "Zrób zdjęcie" => await MediaPicker.Default.CapturePhotoAsync(),
                "Wybierz z galerii" => await MediaPicker.Default.PickPhotoAsync(),
                _ => null
            };

            if (photo == null)
                return;

            using var stream = await photo.OpenReadAsync();

            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            ocrResult = await RecognizeWithGemini(ms.ToArray());

            var resultSchedule = await ScheduleParser.ParseAsync(ocrResult);
            if(await IsCorrect(resultSchedule))
            {
                await SaveData(resultSchedule);
            }
        }

        [Obsolete]
        private async Task<bool> IsCorrect(List<ShiftDay> shifts)
        {
            var edDays = shifts
                .Where(x => x.Shift == "ED")
                .Select(x => x.Date.ToString("dd.MM"));
            var enDays = shifts
                .Where(x => x.Shift == "EN")
                .Select(x => x.Date.ToString("dd.MM"));
            var wDays = shifts
                .Where(x => x.Shift == "W")
                .Select(x => x.Date.ToString("dd.MM"));
            var E1Days = shifts
            .Where(x => x.Shift == "E1")
            .Select(x => x.Date.ToString("dd.MM"));

     

            string message =
                $"Dzień: {string.Join(", ", edDays)}\n\n" +
                $"Dzienna krótka: {string.Join(", ", E1Days)}\n\n" +
                $"Noc: {string.Join(", ", enDays)}\n\n" +
                $"Wolne: {string.Join(", ", wDays)}\n\n" +
                "Czy zaimportować ten harmonogram?";

            return await Utilitis.ShowPopUp(
                "Import harmonogramu",
                message,
                "Tak",
                "Nie");
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
                                        W trzecim wierszu są zmiany: ED, EDn, EN, ENn, E1, E2, W lub puste pole.
                                        Traktuj EDn tak samo jak ED, i ENn tak samo jak EN i E2 tak samo jak E1.
                                        Dla każdej daty wypisz zmianę w formacie DATA|ZMIANA.
                                        Jeśli pole jest puste wpisz DW.
                                        Jeśli pole ma W wpisz W.
                                        Wypisz TYLKO pary data|zmiana, bez żadnego dodatkowego tekstu.
                                        Przykład:
                                        01.05|ED
                                        02.05|EN
                                        03.05|DW
                                        04.05|W 
                                        04.05|E1"

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

        public async Task<string> LookForNextShift()
        {
            var database = new SQLiteAsyncConnection(dbPath);
            var today = DateTime.Today;

            ShiftDay result = await database.FindWithQueryAsync<ShiftDay>(
                "SELECT * FROM ShiftDay WHERE Date >= ? AND (Shift = 'ED' OR Shift = 'EN') ORDER BY Date ASC LIMIT 1", today);

            if (result != null)
            {
                return  $" {result.DayName} {ShiftParser(result)}";
            }
            return string.Empty;
        }


        public string ShiftParser(ShiftDay shiftDay)
        {
            switch (shiftDay.Shift)
            {
                case "ED":
                    return "zmiana dzienna";
                case "EDn":
                    return "zmiana dzienna neurochirurgiczna";
                case "EN":
                    return "zmiana nocna";
                case "ENn":
                    return "zmiana nocna neurochirurgiczna";
                case "W":
                    return "urlop";
                case "E1":
                    return "zmiana dzienna krótka";
                case "E2":
                    return "zmiana dzienna krótka";
                default:
                    return "dzień wolny";
            }
        }

        public async Task<string> LookForTodayShift()
        {
            var database = new SQLiteAsyncConnection(dbPath);
            var today = DateTime.Today;

            ShiftDay result = await database.FindWithQueryAsync<ShiftDay>(
                "SELECT * FROM ShiftDay WHERE Date >= ? ORDER BY Date ASC LIMIT 1", today);

            if (result != null)
            {
                return ShiftParser(result);
            }
             return null;
        }


        private async Task SaveData(List<ShiftDay> jsonPath)
        {
            try
            {
                var database = new SQLiteAsyncConnection(dbPath);
                await database.CreateTableAsync<ShiftDay>();
                await database.InsertAllAsync(jsonPath);
            }
            catch (Exception ex)
            {

                await Utilitis.ShowPopUp("Błąd", $"Wystąpił błąd podczas zapisywania danych: {ex.Message}", "OK");

            }
        }



        public async Task DeleteData()
        {


            string action = await Utilitis.GetCurrentPage().DisplayActionSheet(
                "Który miesiąc chcesz usunąć?",
                "Anuluj",
                null,
                "Bieżący miesiąc",
                "Poprzedni miesiąc", "Następny miesiąc");

            switch (action)
            {
                case "Bieżący miesiąc":
                    await DeleteMonth(DateTime.Today);
                    break;
                case "Poprzedni miesiąc":
                    await DeleteMonth(DateTime.Today.AddMonths(-1));
                    break;
                case "Następny miesiąc":
                    await DeleteMonth(DateTime.Today.AddMonths(1));
                    break;
            }

        }

        private async Task DeleteMonth(DateTime month)
        {
            try
            {
                var database = new SQLiteAsyncConnection(dbPath);

                var firstDay = new DateTime(month.Year, month.Month, 1).Ticks;
                var lastDay = new DateTime(month.Year, month.Month,
                    DateTime.DaysInMonth(month.Year, month.Month), 23, 59, 59).Ticks;

                await database.ExecuteAsync(
                    "DELETE FROM ShiftDay WHERE Date >= ? AND Date <= ?",
                    firstDay, lastDay);
            }
            catch(Exception ex) {

                    await Utilitis.ShowPopUp("Błąd", $"Wystąpił błąd podczas zapisywania danych: {ex.Message}", "OK");
            }


        }


    }
}

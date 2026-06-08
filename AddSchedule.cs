
using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.Maui.OCR;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PigulaSchedule.Model;

namespace PigulaSchedule
{
    class AddSchedule
    {
        private string ocrResult = string.Empty;

        string dbPath = Path.Combine(
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

            var result = await OcrPlugin.Default
                .RecognizeTextAsync(ms.ToArray());

            ocrResult = result.AllText;

            var resultSchedule = ScheduleParser.Parse(ocrResult);

            await SaveData(resultSchedule);
        }
        
        

        private async Task SaveData(List<ShiftDay> jsonPath)
        {
            var database = new SQLiteAsyncConnection(dbPath);

            if (jsonPath == null)
                return;

            await database.CreateTableAsync<ShiftDay>();

            await database.InsertAllAsync(jsonPath);
        }
    }
}


using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.PlatformConfiguration;
using PigulaSchedule.Interface;
using PigulaSchedule.Model;
using PigulaSchedule.View;
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
    public partial class AddSchedule
    {

        private readonly IGeminiOcrService _ocrService;
        private readonly IShiftRepository _shiftRepository;

        public AddSchedule(IGeminiOcrService ocrService, IShiftRepository shiftRepository)
        {
            _ocrService = ocrService;
            _shiftRepository = shiftRepository;
        }


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
            var bytes = ms.ToArray();

            await Shell.Current.GoToAsync(nameof(Ohoto), new Dictionary<string, object>
            {
                { "PhotoBytes", bytes }
            });


            var ocrResult = await _ocrService.RecognizeScheduleAsync(ms.ToArray());
            var resultSchedule = await ScheduleParser.ParseAsync(ocrResult);

            if (await IsCorrect(resultSchedule))
            {
                await _shiftRepository.DeleteMonthAsync(resultSchedule.First().Date.AddYears(-1));
                await _shiftRepository.SaveShiftsAsync(resultSchedule);
            }
        }


        public async Task<string> LookForNextShift()
        {
            try
            {
                var result = await _shiftRepository.GetNextShiftAsync(DateTime.Today);
                return result != null ? $" {result.DayName} {ShiftParser(result)}" : string.Empty;
            }
            catch (Exception ex)
            {
                await Utilitis.ShowPopUp("Błąd", $"Błąd podczas pobierania danych z tabeli ShiftDay: {ex.Message}", "OK");
                return string.Empty;
            }
              
         
        }

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
                "Nie",
                "Tak");
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

 

        private async Task SaveData(List<ShiftDay> jsonPath)
        {
            try
            {
               await _shiftRepository.SaveShiftsAsync(jsonPath);
            }
            catch (Exception ex)
            {
                await Utilitis.ShowPopUp("Błąd", $"Wystąpił błąd podczas zapisywania danych: {ex.Message}", "OK");
            }
        }

        public async Task DeleteData()
        {
            try
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
                        await _shiftRepository.DeleteMonthAsync(DateTime.Today);
                        break;
                    case "Poprzedni miesiąc":
                        await _shiftRepository.DeleteMonthAsync(DateTime.Today.AddMonths(-1));
                        break;
                    case "Następny miesiąc":
                        await _shiftRepository.DeleteMonthAsync(DateTime.Today.AddMonths(1));
                        break;
                }
            }
            catch (Exception ex)
            {
                await Utilitis.ShowPopUp("Błąd", $"Wystąpił błąd: {ex.Message}", "OK");
                return;
            }

              

        }

        /// <summary>
        /// To clear old data from the database, we will delete all records that are older than one year from the given shiftDay date.
        /// </summary>
        /// <param name="shiftDay"></param>
        /// <returns></returns>
        public async Task DeleteOldData(ShiftDay shiftDay)
        {
            try
            {
                var dateToDelete = shiftDay.Date.AddYears(-1);
                await _shiftRepository.DeleteMonthAsync(dateToDelete);
            }
            catch (Exception ex)
            {
                await Utilitis.ShowPopUp("Błąd", $"Wystąpił błąd : {ex.Message}", "OK");
                return;
            }
        }

     
      

    }
}

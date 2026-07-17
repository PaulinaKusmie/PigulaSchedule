
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PigulaSchedule.Interface;
using PigulaSchedule.Model;
using PigulaSchedule.Resources;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XCalendar.Core.Models;

namespace PigulaSchedule.ViewModel
{
    public partial class CalendarViewModel : ObservableObject
    {
        [ObservableProperty] private int month = DateTime.Now.Month;
        [ObservableProperty] private int year = DateTime.Now.Year;
        [ObservableProperty] private int day = DateTime.Now.Day;
        [ObservableProperty] private string title;
        [ObservableProperty] private string dayText = " Dzień";
        [ObservableProperty] private string nightText = " Noc"; //
        [ObservableProperty] private string dayOffText = " Wolne";
        [ObservableProperty] private string shortDayText = " Dzień krótka";
        [ObservableProperty] private double daysViewHeightRequest = 500;
        [ObservableProperty] private ObservableCollection<DateTime> datesColor1 = new();
        [ObservableProperty] private ObservableCollection<DateTime> datesColor2 = new();
        [ObservableProperty] private ObservableCollection<DateTime> datesColor3 = new();
        [ObservableProperty] private ObservableCollection<DateTime> datesColor4 = new();

        private SQLiteAsyncConnection database;

        private readonly IShiftRepository _shiftRepository;

        public CalendarViewModel(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task InitializeAsync()
        {
            await LoadData();
        }


        public async Task LoadData()
        {

            await ClearData();

            List<ShiftDay>? shifts = null;
            try
            {
                shifts = await _shiftRepository.GetAllShiftsAsync();
            }
            catch (Exception ex)
            {
                await Utilitis.ShowPopUp("Błąd", $"Błąd podczas pobierania danych z tabeli ShiftDay: {ex.Message}", "OK");
                return;
            }


            if (shifts == null || shifts.Count == 0)
            {
                await Utilitis.ShowPopUp("Błąd", $"Brak danych w tabeli ShiftDay. Dodaj jakikolwiek harmonogram:", "OK");
                return;
            }

            foreach (var shift in shifts)
            {
                switch (shift.Shift)
                {
                    case "ED": datesColor1.Add(shift.Date.Date); break;
                    case "E1": datesColor4.Add(shift.Date.Date); break;
                    case "EN": datesColor2.Add(shift.Date.Date); break;
                    case "W": datesColor3.Add(shift.Date.Date); break;
                }
            }

        }


        private async Task ClearData()
        {
            try
            {
                datesColor1.Clear();
                datesColor2.Clear();
                datesColor3.Clear();
                datesColor4.Clear();
            }
            catch (Exception ex)
            {
                await Utilitis.ShowPopUp("Błąd", $"Błąd podczas usuwania danych z tabeli ShiftDay: {ex.Message}", "OK");
            }
        }
    }
}

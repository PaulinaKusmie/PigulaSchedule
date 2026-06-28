
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PigulaSchedule.Model;
using PigulaSchedule.Resources;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XCalendar.Core.Models;

namespace PigulaSchedule.ViewModel
{
    public partial class CalendarViewModel : ObservableObject
    {
        private int month;
        public int Month
        {
            get => month;
            set
            {
                month = value;
                OnPropertyChanged();
            }
        }

        private int year;
        public int Year
        {
            get => year;
            set
            {
                year = value;
                OnPropertyChanged();
            }
        }

        private int day;

        public int Day
        {
            get => day;
            set
            {
                day = value;
                OnPropertyChanged();
            }
        }

        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }


        private string dayText;
        public string DayText
        {
            get => dayText;
            set
            {
                dayText = value;
                OnPropertyChanged();
            }
        }

        private string nigthText;

        public string NigthText
        {
            get => nigthText;
            set
            {
                nigthText = value;
                OnPropertyChanged();
            }
        }

        private string dayOffText;
        public string DayOffText
        {
            get => dayOffText;
            set
            {
                dayOffText = value;
                OnPropertyChanged();
            }
        }

        private string shortDayText;
        public string ShortDayText
        {
            get => shortDayText;
            set
            {
                shortDayText = value;
                OnPropertyChanged();
            }
        }

        private double daysViewHeightRequest = 500;
        public double DaysViewHeightRequest
        {
            get => daysViewHeightRequest;
            set
            {
                daysViewHeightRequest = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<DateTime> datesColor1 = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> DatesColor1
        {
            get => datesColor1;
            set
            {
                datesColor1 = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DateTime> datesColor2 = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> DatesColor2
        {
            get => datesColor2;
            set
            {
                datesColor2 = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DateTime> datesColor3 = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> DatesColor3
        {
            get => datesColor3;
            set
            {
                datesColor3 = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<DateTime> datesColor4 = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> DatesColor4
        {
            get => datesColor4  ;
            set
            {
                datesColor4 = value;
                OnPropertyChanged();
            }
        }

        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "pigulaApp.db3");

        private SQLiteAsyncConnection database;

        public CalendarViewModel()
        {


            Day = DateTime.Now.Day;
            Month = DateTime.Now.Month;
            Year = DateTime.Now.Year;
            DayText = " Dzień";
            NigthText = " Noc";
            DayOffText = " Wolne";
            ShortDayText = " Dzień krótka";



            database = new SQLiteAsyncConnection(dbPath);
            LoadData();

        }
          
        public async Task LoadData()
        {
            List<ShiftDay> shifts = await database.Table<ShiftDay>().ToListAsync();

            if (shifts.Count == 0)
            {
                Debug.WriteLine("Brak danych w tabeli ShiftDay. Dodaj jakikolwiek harmonogram.");
                return;
            }
            
            foreach (var shift in shifts)
            {
                if (shift.Shift == "ED")
                    DatesColor1.Add(shift.Date.Date);
                else if (shift.Shift == "E1")
                    DatesColor4.Add(shift.Date.Date);
                else if (shift.Shift == "EN")
                    DatesColor2.Add(shift.Date.Date);
                else if (shift.Shift == "W")
                    DatesColor3.Add(shift.Date.Date);
            }

        }
    }
}

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

        private double daysViewHeightRequest = 500 ;
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

        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "pigulaApp.db3");

        private SQLiteAsyncConnection database;

        public  CalendarViewModel()
        {
       

            Day = DateTime.Now.Day;
            Month = DateTime.Now.Month;
            Year = DateTime.Now.Year;
            

            database = new SQLiteAsyncConnection(dbPath);

            List<ShiftDay> shifts = database.Table<ShiftDay>().ToListAsync().Result;

            DatesColor1.Clear(); // czerwony = ED
            DatesColor2.Clear(); // niebieski = EN

            foreach (var shift in shifts)
            {
                if (shift.Shift == "ED")
                    DatesColor1.Add(shift.Date.Date);
                else if (shift.Shift == "EN")
                    DatesColor2.Add(shift.Date.Date);
            }
            Title = $"Twój {Utiliti.IntToNameMonth(DateTime.Now.Month)}";
        }

 
        private void OnDaysUpdated(object sender, EventArgs e)
        {
            List<ShiftDay> shifts = database.Table<ShiftDay>().ToListAsync().Result;

            DatesColor1.Clear(); // czerwony = ED
            DatesColor2.Clear(); // niebieski = EN

            foreach (var shift in shifts)
            {
                if (shift.Shift == "ED")
                    DatesColor1.Add(shift.Date.Date);
                else if (shift.Shift == "EN")
                    DatesColor2.Add(shift.Date.Date);
            }
            //foreach (var day in MyCalendar.Days)
            //{
            //    var shift = shifts.FirstOrDefault(s => s.Date.Date == day.DateTime.Date);

            //    if (shift == null)
            //    {
            //        day.ShiftColor = Colors.Transparent;
            //        day.IsSelected = false;
            //    }
            //    else if (shift.Shift == "ED")
            //    {
            //        day.ShiftColor = Colors.Red;
            //        day.IsSelected = true;
            //    }
            //    else if (shift.Shift == "EN")
            //    {
            //        day.ShiftColor = Colors.Blue;
            //        day.IsSelected = true;
            //    }
            //}
        }


        // LeftAsync - tak samo jak Right
        [RelayCommand]
        public async Task LeftAsync()
        {
            //var target = MyCalendar.NavigatedDate.AddMonths(-1);
            //MyCalendar.Navigate(target - MyCalendar.NavigatedDate);
            //OnPropertyChanged(nameof(MyCalendar));
            //Title = $"Twój {Utiliti.IntToNameMonth(MyCalendar.NavigatedDate.Month)}";
        }

        [RelayCommand]
        public async Task RightAsync()
        {
            //var target = MyCalendar.NavigatedDate.AddMonths(1);
            //MyCalendar.Navigate(target - MyCalendar.NavigatedDate);
            //OnPropertyChanged(nameof(MyCalendar));
            //Title = $"Twój {Utiliti.IntToNameMonth(MyCalendar.NavigatedDate.Month)}";
        }



    }
}

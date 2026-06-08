using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PigulaSchedule.Model;
using PigulaSchedule.Resources;
using Plugin.Maui.Calendar.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private double daysViewHeightRequest;
        public double DaysViewHeightRequest
        {
            get => daysViewHeightRequest;
            set
            {
                daysViewHeightRequest = value;
                OnPropertyChanged();
            }
        }
        
        public List<DateTime> EdDays { get; set; } = new List<DateTime>();
        public List<DateTime> EnDays { get; set; } = new List<DateTime>();
        public Calendar<WorkShift> MyCalendar { get; set; } = new Calendar<WorkShift>();


        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "pigulaApp.db3");

        private SQLiteAsyncConnection database;

        public  CalendarViewModel()
        {
       

            Day = DateTime.Now.Day;
            Month = DateTime.Now.Month;
            Year = DateTime.Now.Year;

            //DaysViewHeightRequest = DeviceDisplay.MainDisplayInfo.Height
            //                        / DeviceDisplay.MainDisplayInfo.Density;

            database = new SQLiteAsyncConnection(dbPath);

            MyCalendar.DaysUpdated += OnDaysUpdated;
            OnDaysUpdated(null, null);

            MyCalendar.NavigatedDate = MyCalendar.NavigatedDate.AddMonths(-1);


            Title = $"Twój {Utiliti.IntToNameMonth(DateTime.Now.Month)}";
        }


        private void OnDaysUpdated(object sender, EventArgs e)
        {
            List<ShiftDay> shifts = database.Table<ShiftDay>().ToListAsync().Result;
            foreach (var day in MyCalendar.Days)
            {
                var shift = shifts.FirstOrDefault(s => s.Date.Date == day.DateTime.Date);

                if (shift == null)
                {
                    day.ShiftColor = Colors.Transparent;
                    day.IsSelected = false;
                }
                else if (shift.Shift == "ED")
                {
                    day.ShiftColor = Colors.Red;
                    day.IsSelected = true;
                }
                else if (shift.Shift == "EN")
                {
                    day.ShiftColor = Colors.Blue;
                    day.IsSelected = true;
                }
            }
        }


        [RelayCommand]
        public async Task LeftAsync()
        {
            try
            {
                


            }
            catch (Exception ex)
            {
            
            }
            finally
            {
          
            }
        }

        [RelayCommand]
        public async Task RightAsync()
        {
            try
            {
                AddSchedule addSchedule = new AddSchedule();
                await addSchedule.DeleteData();

            }
            catch (Exception ex)
            {

            }
            finally
            {
         
            }
        }
    }
}

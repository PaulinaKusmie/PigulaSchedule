using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PigulaSchedule.Model;
using PigulaSchedule.Resources;
using Plugin.Maui.Calendar.Models;
using SQLite;
using System;
using System.Collections.Generic;
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

        private double daysViewHeightRequest = 300 ;
        public double DaysViewHeightRequest
        {
            get => daysViewHeightRequest;
            set
            {
                daysViewHeightRequest = value;
                OnPropertyChanged();
            }
        }


        private Calendar<WorkShift> _myCalendar = new Calendar<WorkShift>();
        public Calendar<WorkShift> MyCalendar
        {
            get => _myCalendar;
            set
            {
                _myCalendar = value;
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

            MyCalendar.DaysUpdated += OnDaysUpdated;
            OnDaysUpdated(null, null);



            MyCalendar.NavigatedDate = MyCalendar.NavigatedDate.AddMonths(-1);
            MyCalendar.NavigationLowerBound = DateTime.Today.AddYears(-2);
            MyCalendar.NavigationUpperBound = DateTime.Today.AddYears(2);
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

                MyCalendar.NavigatedDate = MyCalendar.NavigatedDate.AddMonths(-1);
                Title = $"Twój {Utiliti.IntToNameMonth(MyCalendar.NavigatedDate.Month)}";
                OnPropertyChanged(nameof(MyCalendar));

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
                var target = MyCalendar.NavigatedDate.AddMonths(1);

                MyCalendar.Navigate(
                    target - MyCalendar.NavigatedDate);

                Debug.WriteLine(
                    $"Nowa data: {MyCalendar.NavigatedDate:yyyy-MM-dd}");

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

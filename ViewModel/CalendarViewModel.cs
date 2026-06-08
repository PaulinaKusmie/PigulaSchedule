using CommunityToolkit.Mvvm.ComponentModel;
using PigulaSchedule.Model;
using PigulaSchedule.Resources;
using Plugin.Maui.Calendar.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigulaSchedule.ViewModel
{
    class CalendarViewModel : ObservableObject
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

        Color GetShiftColor(WorkShift shift)
        {
            return shift.ShiftType switch
            {
                "Morning" => Colors.Yellow,
                "Night" => Colors.DarkBlue,
            };
        }

        public EventCollection Events { get; set; } = new();

        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "pigulaApp.db3");

        public CalendarViewModel()
        {

            Day = DateTime.Now.Day;
            Month = DateTime.Now.Month;
            Year = DateTime.Now.Year;


            var database = new SQLiteAsyncConnection(dbPath);
            List<ShiftDay> persons =  database.Table<ShiftDay>().ToListAsync().Result;

            foreach (var VARIABLE in persons)
            {
                var dd = VARIABLE.Date;
            }

            Events.Add(DateTime.Today, new List<object>
            {
                new WorkShift { Title = "Zmiana poranna", Type = "Morning" },
                new WorkShift { Title = "Zmiana nocna", Type = "Night" }
            });

            Title = $"Twój {Utiliti.IntToNameMonth(DateTime.Now.Month)}";
        }
    }
}

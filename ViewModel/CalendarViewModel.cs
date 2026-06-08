using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PigulaSchedule.Model;
using SQLite;

namespace PigulaSchedule.ViewModel
{
    class CalendarViewModel : ObservableObject
    {
        private string month;
        public string Month
        {
            get => month;
            set
            {
                month = value;
                OnPropertyChanged();
            }
        }

        //private string persons;
        //public string Persons
        //{
        //    get => persons;
        //    set
        //    {
        //        persons = value;
        //        OnPropertyChanged();
        //    }
        //}

        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "pigulaApp.db3");

        public CalendarViewModel()
        {

            var database = new SQLiteAsyncConnection(dbPath);
            List<ShiftDay> persons =  database.Table<ShiftDay>().ToListAsync().Result;

            foreach (var VARIABLE in persons)
            {
                var dd = VARIABLE.Date;
            }
            month = 6.ToString();
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCalendar.Core.Interfaces;
using XCalendar.Core.Models;


namespace PigulaSchedule.Model
{
    public class WorkShift : CalendarDay, ICalendarDay
    {

        private Color _shiftColor = Colors.Transparent;

        public Color ShiftColor
        {
            get => _shiftColor;
            set
            {
                _shiftColor = value;
                OnPropertyChanged();
            }
        }
        public DateTime DateTime { get; set; }

        public bool IsSelected { get; set; }

        public bool IsCurrentMonth { get; set; }

        public bool IsToday { get; set; }

        public bool IsInvalid { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

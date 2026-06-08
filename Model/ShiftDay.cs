using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigulaSchedule.Model
{
    public class ShiftDay
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; }
        public string Shift { get; set; }
    }
}

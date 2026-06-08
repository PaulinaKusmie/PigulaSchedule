using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigulaSchedule.Model
{
    public record ShiftDay
    {
        public string Date { get; set; }
        public string Day { get; set; }
        public string Shift { get; set; } // ED / EN / null
    }
}

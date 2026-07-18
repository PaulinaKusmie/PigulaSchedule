using PigulaSchedule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigulaSchedule.Interfaces
{
    public interface IShiftQuery
    {
        Task<List<ShiftDay>> GetAllShiftsAsync();
    }
}

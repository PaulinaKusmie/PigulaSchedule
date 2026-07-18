using PigulaSchedule.Interfaces;
using PigulaSchedule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigulaSchedule.Interface
{
    public interface IShiftRepository : IShiftQuery
    {
        Task<ShiftDay?> GetNextShiftAsync(DateTime fromDate);
        Task<ShiftDay?> GetTodayShiftAsync(DateTime fromDate);
        Task SaveShiftsAsync(List<ShiftDay> shifts);
        Task DeleteMonthAsync(DateTime month);
    }
}

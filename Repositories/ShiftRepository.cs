using PigulaSchedule.Interface;
using PigulaSchedule.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigulaSchedule.Repository
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly string _dbPath;
        private SQLiteAsyncConnection? _connection;

        public ShiftRepository()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "pigulaApp.db3");
        }

        private SQLiteAsyncConnection Database => _connection ??= new SQLiteAsyncConnection(_dbPath);


        public async Task<List<ShiftDay>> GetAllShiftsAsync()
         => await Database.Table<ShiftDay>().ToListAsync();

        public async Task<ShiftDay?> GetNextShiftAsync(DateTime fromDate)
        {
            return await Database.FindWithQueryAsync<ShiftDay>(
                "SELECT * FROM ShiftDay WHERE Date >= ? AND (Shift = 'ED' OR Shift = 'EN' OR Shift = 'E1') ORDER BY Date ASC LIMIT 1",
                fromDate);
        }

        public async Task<ShiftDay?> GetTodayShiftAsync(DateTime fromDate)
        {
            return await Database.FindWithQueryAsync<ShiftDay>(
                "SELECT * FROM ShiftDay WHERE Date >= ? ORDER BY Date ASC LIMIT 1",
                fromDate);
        }

        public async Task SaveShiftsAsync(List<ShiftDay> shifts)
        {
            await Database.CreateTableAsync<ShiftDay>();
            await Database.InsertAllAsync(shifts);
        }

        public async Task DeleteMonthAsync(DateTime month)
        {
            var firstDay = new DateTime(month.Year, month.Month, 1).Ticks;
            var lastDay = new DateTime(month.Year, month.Month,
                DateTime.DaysInMonth(month.Year, month.Month), 23, 59, 59).Ticks;

            await Database.ExecuteAsync(
                "DELETE FROM ShiftDay WHERE Date >= ? AND Date <= ?",
                firstDay, lastDay);
        }

     
    }
}

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
             Database.CreateTableAsync<ShiftDay>();
        }

        private SQLiteAsyncConnection Database => _connection ??= new SQLiteAsyncConnection(_dbPath);

        public async Task<List<ShiftDay>> GetAllShiftsAsync()
         => await Database.Table<ShiftDay>().ToListAsync();


        public async Task<ShiftDay?> GetNextShiftAsync(DateTime fromDate)
        {
            try
            {
                return await Database.FindWithQueryAsync<ShiftDay>(
              "SELECT * FROM ShiftDay WHERE Date >= ? AND (Shift = 'ED' OR Shift = 'EN' OR Shift = 'E1') ORDER BY Date ASC LIMIT 1",
              fromDate);
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Bład pobierania danych dla następnej zmiany {fromDate}: {ex.Message}");
            }
          
        }

        public async Task<ShiftDay?> GetTodayShiftAsync(DateTime fromDate)
        {
            try
            {
                var todayShift = await Database.FindWithQueryAsync<ShiftDay>(
                    "SELECT * FROM ShiftDay WHERE Date = ?",
                    fromDate);
                return todayShift;
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Bład pobierania danych dla dnia {fromDate}: {ex.Message}");
            }

        }

        public async Task SaveShiftsAsync(List<ShiftDay> shifts)
        {
            try
            {
               
                await Database.InsertAllAsync(shifts);
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Bład zapisywania danych: {ex.Message} ");
            }
        }

        public async Task DeleteMonthAsync(DateTime month)
        {
            var firstDay = new DateTime(month.Year, month.Month, 1).Ticks;
            var lastDay = new DateTime(month.Year, month.Month,
                DateTime.DaysInMonth(month.Year, month.Month), 23, 59, 59).Ticks;

            try
            {
                await Database.ExecuteAsync(
                    "DELETE FROM ShiftDay WHERE Date >= ? AND Date <= ?",
                    firstDay, lastDay);
            }
            catch (SQLiteException ex)
            {
                throw new Exception($"Bład pobierania danych dla miesiąca {month}: {ex.Message}");
            }


        }

    }
}

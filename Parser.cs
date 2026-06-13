using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PigulaSchedule.Model;



namespace PigulaSchedule
{
    public static class ScheduleParser
    {

        public static List<ShiftDay> Parse(string text)
        {
            var result = new List<ShiftDay>();

            var lines = text
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Contains('|'));

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length < 2) continue;

                var dateStr = parts[0].Trim();
                var shift = parts[1].Trim().ToUpper();

                // Normalizacja zmian
                shift = shift switch
                {
                    "EDN" => "ED",
                    "ENN" => "EN",
                    _ => shift
                };

                if (!DateTime.TryParseExact(
                        $"{dateStr}.{DateTime.Now.Year}",
                        "dd.MM.yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var date)) continue;

                result.Add(new ShiftDay
                {
                    Date = date,
                    Shift = shift,
                    DayName = date.ToString("ddd", new CultureInfo("pl-PL"))
                });
            }

            return result;
        }




    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PigulaSchedule.Model;

namespace PigulaSchedule
{
    public static class ScheduleParser
    {
        private static string GetShiftForIndex(string fullText, string date)
        {
            // Znajdź ED lub EN w okolicach daty
            var idx = fullText.IndexOf(date);
            if (idx < 0) return "";
            var nearby = fullText.Substring(idx, Math.Min(30, fullText.Length - idx));
            if (nearby.Contains("ED")) return "ED";
            if (nearby.Contains("EN")) return "EN";
            return "";
        }


        public static List<ShiftDay> Parse(string text)
        {
            // normalizacja OCR
            text = text.Replace("\r", " ");
            text = Regex.Replace(text, @"\s+", " ");

            // 1. daty (06.05 format)
            var dates = Regex.Matches(text, @"\d{2}\.\d{2}")
                .Select(m => m.Value)
                .ToList();

            // 2. dni tygodnia (ŚR CZ PI...)
            var days = Regex.Matches(text, @"\b(PO|WT|ŚR|CZ|CZW|PI|SO|NI)\b")
                .Select(m => m.Value)
                .ToList();

            // 3. zmiany (ED / EN)
            var shifts = Regex.Matches(text, @"\b(ED|EN|W)\b")
                .Select(m => m.Value)
                .ToList();

            int count = Math.Min(dates.Count, days.Count);

            var result = new List<ShiftDay>();

            for (int i = 0; i < count; i++)
            {
                result.Add(new ShiftDay
                {
                    Date = dates.ElementAtOrDefault(i),
                    Day = days.ElementAtOrDefault(i),
                    Shift = shifts.ElementAtOrDefault(i)
                });
            }

            return result;
        }
        private static DateOnly ParseDate(string input)
        {
            var parts = input.Split('.');
            return new DateOnly(
                2025, // albo dynamiczny rok
                int.Parse(parts[1]),
                int.Parse(parts[0]));
        }

    }



}

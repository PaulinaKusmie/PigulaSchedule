using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PigulaSchedule
{
    public static class ScheduleParser
    {
        //public static List<GrafikEntry> Parse(string ocrText)
        //{
        //    var lines = ocrText
        //        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
        //        .Select(l => l.Trim())
        //        .ToList();

        //    // Wyciągnij daty (format dd.MM lub d.MM)
        //    var datePattern = @"\d{1,2}\.\d{2}";
        //    var dates = new List<string>();
        //    foreach (var line in lines)
        //    {
        //        var matches = Regex.Matches(line, datePattern);
        //        foreach (Match m in matches)
        //            dates.Add(m.Value);
        //    }

        //    // Wyciągnij dni tygodnia
        //    var dayNames = new[] { "PN", "PO", "WT", "ŚR", "SR", "CZ", "PI", "SO", "NI" };
        //    var days = new List<string>();
        //    foreach (var line in lines)
        //    {
        //        var tokens = line.Split(' ', '|');
        //        foreach (var token in tokens)
        //        {
        //            var t = token.Trim();
        //            if (dayNames.Contains(t))
        //                days.Add(t == "SR" ? "ŚR" : t);
        //        }
        //    }

        //    // Wyciągnij wartości (ED, EN, puste)
        //    var shiftPattern = @"\b(ED|EN)\b";
        //    // Zbierz pozycje w tekście
        //    var allShifts = Regex.Matches(ocrText, shiftPattern)
        //        .Select(m => m.Value)
        //        .ToList();

        //    // Połącz w rekordy
        //    var result = new List<GrafikEntry>();
        //    for (int i = 0; i < dates.Count; i++)
        //    {
        //        result.Add(new GrafikEntry
        //        {
        //            Date = dates.ElementAtOrDefault(i) ?? "",
        //            Day = days.ElementAtOrDefault(i) ?? "",
        //            // ED/EN przypisz tylko tam gdzie OCR je znalazł
        //            Shift = GetShiftForIndex(ocrText, dates.ElementAtOrDefault(i) ?? "")
        //        });
        //    }

        //    return result;
        //}

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

    public class ShiftDay
    {
        public string Date { get; set; }
        public string Day { get; set; }
        public string Shift { get; set; } // ED / EN / null
    }

    public class GrafikEntry
    {
        public string Date { get; set; }
        public string Day { get; set; }
        public string Shift { get; set; } // "ED", "EN", lub ""
    }
}

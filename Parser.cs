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


        //public static List<ShiftDay> Parse(string text)
        //{
        //    // normalizacja OCR
        //    text = text.Replace("\r", " ");
        //    text = Regex.Replace(text, @"\s+", " ");

        //    // 1. daty (06.05 format)
        //    var dates = Regex.Matches(text, @"\d{2}\.\d{2}")
        //        .Select(m => DateTime.ParseExact(
        //            $"{m.Value}.{DateTime.Now.Year}",
        //            "dd.MM.yyyy",
        //            CultureInfo.InvariantCulture))
        //        .ToList();
        //    // 2. dni tygodnia (ŚR CZ PI...)
        //    var days = Regex.Matches(text, @"\b(PO|WT|ŚR|CZ|CZW|PI|SO|NI)\b")
        //        .Select(m => m.Value)
        //        .ToList();

        //    // 3. zmiany (ED / EN)
        //    var shifts = Regex.Matches(text, @"\b(ED|EN|W)\b")
        //        .Select(m => m.Value)
        //        .ToList();

        //    int count = Math.Min(dates.Count, days.Count);

        //    var result = new List<ShiftDay>();

        //    for (int i = 0; i < count; i++)
        //    {
        //        result.Add(new ShiftDay
        //        {
        //            Date = dates.ElementAtOrDefault(i),
        //            DayName = days.ElementAtOrDefault(i),
        //            Shift = shifts.ElementAtOrDefault(i)
        //        });
        //    }

        //    return result;
        //}

        public static List<ShiftDay> Parse(string text)
        {
            // Normalizacja
            text = text.Replace("0s", "05");
            text = text.Replace("ŠR", "ŚR");
            text = text.Replace("SR", "ŚR");
            text = text.Replace("Cz", "CZ");
            text = text.Replace("So", "SO");

            // 1. Daty
            var dates = Regex.Matches(text, @"\d{2}\.\d{2}")
                .Select(m => DateTime.ParseExact(
                    $"{m.Value}.{DateTime.Now.Year}",
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            // 2. Dni tygodnia z pozycjami
            var dayMatches = Regex.Matches(text, @"\b(PO|WT|ŚR|CZ|PI|SO|NI)\b",
                RegexOptions.IgnoreCase);

            // 3. Zmiany z pozycjami  
            var shiftMatches = Regex.Matches(text, @"\b(ED|EN|W)\b",
                RegexOptions.IgnoreCase);

            // Posortuj dni i zmiany po pozycji w tekście
            var days = dayMatches
                .OrderBy(m => m.Index)
                .Select(m => m.Value.ToUpper())
                .ToList();

            var shifts = shiftMatches
                .OrderBy(m => m.Index)
                .Select(m => m.Value.ToUpper())
                .ToList();

            // Uzupełnij brakujące
            while (days.Count < dates.Count) days.Add("?");
            while (shifts.Count < dates.Count) shifts.Add("W");

            var result = new List<ShiftDay>();
            for (int i = 0; i < dates.Count; i++)
            {
                result.Add(new ShiftDay
                {
                    Date = dates[i],
                    DayName = days.ElementAtOrDefault(i) ?? "?",
                    Shift = shifts.ElementAtOrDefault(i) ?? "W"
                });
            }

            return result;
        }

    }



}

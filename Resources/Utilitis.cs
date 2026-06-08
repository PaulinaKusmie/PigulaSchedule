using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigulaSchedule.Resources
{
    public static class Utiliti
    {

        public static string IntToNameMonth(int numberOfMonth)
        {
            return numberOfMonth >= 1 && numberOfMonth <= 12
                ? CultureInfo.GetCultureInfo("pl-PL")
                    .DateTimeFormat
                    .GetMonthName(numberOfMonth)
                : "Nieprawidłowy numer miesiąca";

        }
    }
}

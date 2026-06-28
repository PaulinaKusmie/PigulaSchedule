using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigulaSchedule
{
    public static class Utilitis
    {

        public static string IntToNameMonth(int numberOfMonth)
        {
            return numberOfMonth >= 1 && numberOfMonth <= 12
                ? CultureInfo.GetCultureInfo("pl-PL")
                    .DateTimeFormat
                    .GetMonthName(numberOfMonth)
                : "Nieprawidłowy numer miesiąca";

        }

        public static Page? GetCurrentPage()
        {
            return Application.Current?.Windows.FirstOrDefault()?.Page;
        }

        /// <summary>
        /// Displays a pop-up alert dialog with the specified title, message, and button labels on the current page.
        /// </summary>
        /// <remarks>If <paramref name="accept"/> is an empty string, the alert will display only a single
        /// cancel button. Otherwise, both accept and cancel buttons are shown, allowing the user to confirm or dismiss
        /// the alert.</remarks>
        /// <param name="title">The title text to display at the top of the pop-up alert. Cannot be null.</param>
        /// <param name="message">The message content to display in the pop-up alert. Cannot be null.</param>
        /// <param name="accept">The text for the accept or confirmation button. If empty, only a single cancel button is shown.</param>
        /// <param name="cancel">The text for the cancel or dismiss button. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation of displaying the pop-up alert.</returns>
        public static async Task<bool> ShowPopUp(string title, string message, string cancel, string accept = "")
        {
            var page = GetCurrentPage();
            if (page != null)
            {
                if (accept == string.Empty)
                {
                    await page.DisplayAlert(title, message, cancel);
                    return true;
                }
                else
                {
                     return await page.DisplayAlert(title, message, accept, cancel);
                }
               
            }

            return false;
        }


    }
}

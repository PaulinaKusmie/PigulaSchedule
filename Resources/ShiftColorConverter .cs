using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigulaSchedule.Resources
{
    public class DayColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values[0] == null || values[1] == null || values[2] == null)
                return Colors.Transparent;

            bool isSelected = (bool)values[0];
            bool isToday = (bool)values[1];
            bool isCurrentMonth = (bool)values[2];

            //if (!isCurrentMonth)
            //    return Colors.Transparent;

            if (isSelected)
                return Colors.Red;

            if (isToday)
                return Colors.Orange;

            return Colors.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

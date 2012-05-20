using System;
using System.Globalization;
using System.Windows.Data;

namespace NanaphoNews.Converters
{
    public class DateTimeToTimelineFormatStringConverter : IValueConverter
    {
        private const string FORMAT = "g";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).ToString(FORMAT);
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

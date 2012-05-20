using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NanaphoNews.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return ((bool)value ? Visibility.Visible : Visibility.Collapsed);
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToInverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return ((bool)value ? Visibility.Collapsed : Visibility.Visible);
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace NanaphoNews.Converters
{
    public class PriorityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value == 100 ? true : false);
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return ((bool)value ? 100 : 0);
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }
    }

    public class PriorityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value == 100 ? Visibility.Visible : Visibility.Collapsed);
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PriorityToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value == 100 ? 1.0f : 0.2f);
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PriorityBooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return ((bool)value ? 1.0f : 0.2f);
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

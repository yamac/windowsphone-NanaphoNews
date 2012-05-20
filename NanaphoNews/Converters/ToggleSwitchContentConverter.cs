using System;
using System.Globalization;
using System.Windows.Data;

namespace NanaphoNews.Converters
{
    public class ToggleSwitchContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return
                    (bool)value
                    ? Localization.AppResources.Common_ToggleSwitch_Content_On
                    : Localization.AppResources.Common_ToggleSwitch_Content_Off;
            }
            throw new ArgumentException("Type of the value is incorrect.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

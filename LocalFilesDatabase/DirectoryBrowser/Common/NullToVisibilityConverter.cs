using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DirectoryBrowser.Common.Converters
{
    /// <summary>
    /// Null/Not null to Visibility
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                if (value != null) return Visibility.Visible;
                if (value == null) return Visibility.Collapsed;
            }
            else
            {
                if (value != null) return Visibility.Collapsed;
                if (value == null) return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
           object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}

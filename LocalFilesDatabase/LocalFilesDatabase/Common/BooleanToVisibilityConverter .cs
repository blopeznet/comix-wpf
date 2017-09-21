using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LocalFilesDatabase.Common.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            
            if (value == null) return Visibility.Collapsed;

            if (value is bool)
            {
                if (parameter != null)
                    return (bool)value ? Visibility.Collapsed : Visibility.Visible;
                else
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }


        public object ConvertBack(object value, Type targetType,
           object parameter, CultureInfo culture)
        {
            if (parameter != null)
                return !value.Equals(Visibility.Visible);

            return value.Equals(Visibility.Visible);
        }
    }
}

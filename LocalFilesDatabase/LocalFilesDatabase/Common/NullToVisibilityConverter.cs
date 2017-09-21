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

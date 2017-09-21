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
    public class InvVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            Visibility v = (Visibility)value;

            if (v == Visibility.Visible)
            {
                return Visibility.Collapsed;
            }
            else if (v == Visibility.Collapsed)
            {
                return Visibility.Visible;
            }
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}

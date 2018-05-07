using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DirectoryBrowser.Common.Converters
{
    public class DoubleToMbConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            
            if (value == null) return "";

            if (value is double)
            {
                if ((double)value <= 0)
                    return "";
                else
                    return string.Format("{0:0.00} Mb.", (double)value); 
            }
            else
            {
                return "";
            }
        }


        public object ConvertBack(object value, Type targetType,
           object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

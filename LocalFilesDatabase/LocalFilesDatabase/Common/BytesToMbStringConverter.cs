using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LocalFilesDatabase.Common.Converters
{
    public class BytesToMbStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            
            if (value == null) return "";

            Double bytes = System.Convert.ToDouble(value);
            Double megabytes = (bytes / 1024f) / 1024f;
            megabytes = Math.Round(megabytes, 2);
            return String.Format("{0} Mb", megabytes);
        }


        public object ConvertBack(object value, Type targetType,
           object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

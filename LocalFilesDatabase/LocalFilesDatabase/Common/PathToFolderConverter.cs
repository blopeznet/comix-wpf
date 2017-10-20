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
    public class PathToFolderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            
            if (value == null) return "";
            var path = value.ToString();
            return new DirectoryInfo(path).Name.ToUpper();
        }


        public object ConvertBack(object value, Type targetType,
           object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

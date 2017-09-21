using LocalFilesDatabase.Entities;
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
    public class SnapDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {            
            if (value == null) return "";                        
            return String.Format("({0} Elementos)",((List<ItemFolder>)value).Count);
        }


        public object ConvertBack(object value, Type targetType,
           object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

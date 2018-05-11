using System;
using System.Globalization;
using System.Windows.Data;

namespace DirectoryBrowser.Common.Converters
{
    /// <summary>
    /// Convert double to XX.XX Mb String
    /// </summary>
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

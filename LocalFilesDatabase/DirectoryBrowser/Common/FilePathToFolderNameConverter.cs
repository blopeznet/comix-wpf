using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace DirectoryBrowser.Common.Converters
{

    /// <summary>
    /// File path to filename single
    /// </summary>
    public class FilePathToFolderNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {            
            if (value == null) return "";
            string lastFolderName = Path.GetFileNameWithoutExtension(value.ToString());
            return FirstCharToUpper(lastFolderName);            
        }


        public string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: return "";
                case "": return "";
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public object ConvertBack(object value, Type targetType,
           object parameter, CultureInfo culture)
        {            
            return null;
        }
    }
}

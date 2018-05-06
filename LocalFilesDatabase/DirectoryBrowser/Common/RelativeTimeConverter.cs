using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DirectoryBrowser.Common.Converters
{
    public class RelativeTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {            
            if (parameter != null)
            {
                if (parameter.ToString() == "short")
                {
                    return System.Convert.ToDateTime(value).ToShortDateString();
                }
            }

            if (value == null) return "";            
            return TimeFormatESP(System.Convert.ToDateTime(value));            
        }


        public string TimeFormat(DateTime yourDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - yourDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = System.Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = System.Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }

        public string TimeFormatESP(DateTime yourDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - yourDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "hace un segundo" : "hace " + ts.Seconds + " segundos";

            if (delta < 2 * MINUTE)
                return "hace un minuto";

            if (delta < 45 * MINUTE)
                return "hace " + ts.Minutes + " minutos";

            if (delta < 90 * MINUTE)
                return "hace una hora";

            if (delta < 24 * HOUR)
                return "hace " + ts.Hours + " horas";

            if (delta < 48 * HOUR)
                return "ayer";

            if (delta < 30 * DAY)
                return "hace " + ts.Days + " días";

            if (delta < 12 * MONTH)
            {
                int months = System.Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "hace un mes" : "hace " + months + " meses";
            }
            else
            {
                int years = System.Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "hace un año" : "hace " + years + " años";
            }
        }


        public object ConvertBack(object value, Type targetType,
           object parameter, CultureInfo culture)
        {            
            return null;
        }
    }
}

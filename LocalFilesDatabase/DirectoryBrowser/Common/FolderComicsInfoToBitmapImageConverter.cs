using DirectoryBrowser.Entities;
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
    public class FolderComicsInfoToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return NotFoundImage();
            }
            
            
            
            if (value is FolderComicsInfo)
            {
                BitmapImage bmp = DBService.Instance.LoadFile((FolderComicsInfo)value);

                if (bmp == null)
                {
                    return NotFoundImage();
                }

                return bmp;
            }
            else
            {
                return NotFoundImage();
            }
            
        }

        private BitmapImage NotFoundImage()
        {
            var bmp2 = new BitmapImage();
            bmp2.BeginInit();
            bmp2.UriSource = new Uri(@"/DirectoryBrowser;component/Assets/NotFound.jpg", UriKind.RelativeOrAbsolute);
            bmp2.EndInit();
            return bmp2;
        }

        /// <summary>
        /// Convierte ByteArray en BitmapImage
        /// </summary>
        /// <param name="array">byte[]</param>
        /// <returns>BitmapImage</returns>
        private BitmapImage ByteArrayToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }


        public object ConvertBack(object value, Type targetType,
           object parameter, CultureInfo culture)
        {            
            return null;
        }
    }
}

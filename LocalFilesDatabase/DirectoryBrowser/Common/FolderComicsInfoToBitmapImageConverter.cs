using DirectoryBrowser.Entities;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DirectoryBrowser.Common.Converters
{

    /// <summary>
    /// Folder comic get image from db
    /// </summary>
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

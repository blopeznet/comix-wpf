using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace DirectoryBrowser
{
    public static class UtilsApp
    {
        /// <summary>
        /// Save bitmap image to file into Pictures Folder
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string SaveFileAndGetPath(BitmapImage img)
        {
            String folderpath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\";
            System.IO.Directory.CreateDirectory(folderpath);
            string fileName = folderpath + Guid.NewGuid() + ".jpg";
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            Guid photoID = System.Guid.NewGuid();
            String photolocation = fileName;  //file name 
            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)img));
            using (var filestream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
                encoder.Save(filestream);
            return fileName;
        }

        /// <summary>
        /// BitmapImage to ByteArray 
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        private static byte[] BitmapImageToByteArray(BitmapImage bitmapImage)
        {
            byte[] data = null;
            try
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
            }
            catch { }
            return data;
        }
    }
}

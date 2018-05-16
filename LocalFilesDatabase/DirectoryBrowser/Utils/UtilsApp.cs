using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace DirectoryBrowser
{
    public static class UtilsApp
    {

        public static String GetDocsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CBDExplorer\\";
        }


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

        /// <summary>
        /// Obtiene el thumbnail de un fichero
        /// </summary>
        /// <param name="fileName">Path del fichero</param>
        /// <returns>Bitmap</returns>
        public static Bitmap CreateThumbnail(string fileName)
        {
            try
            {
                int THUMB_SIZE = 192;
                Bitmap thumbnail = WindowsThumbnailProvider.GetThumbnail(
                   fileName, THUMB_SIZE, THUMB_SIZE, ThumbnailOptions.None);
                return thumbnail;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en thumbnail: {0}", ex.Message);
                return null;
            }
        }


    }
}

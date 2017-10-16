using LocalFilesDatabase.Entities;
using LocalFilesDatabase.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LocalFilesDatabase
{
    public static class MainUtils
    {
        
        public static bool SaveFileAndOpen(BitmapImage img)
        {
            
                string fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)+"\\"+ Guid.NewGuid() + ".jpg";                
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                Guid photoID = System.Guid.NewGuid();
                String photolocation = fileName;  //file name 

                encoder.Frames.Add(BitmapFrame.Create((BitmapImage)img));

                using (var filestream = new FileStream(fileName, FileMode.Create))
                    encoder.Save(filestream);

                Process.Start(fileName);
                return true;                        
        }

        #region Converters

        /// <summary>
        /// Convierte ByteArray en String
        /// </summary>
        /// <param name="text">String</param>
        /// <returns>byte[]</returns>
        public static byte[] TextToByteArray(String text)
        {
            return File.ReadAllBytes(text);
        }

        /// <summary>
        /// Convierte ByteArray en String
        /// </summary>
        /// <param name="byteArray">byte[]</param>
        /// <returns>String</returns>
        public static String ByteArrayToString(byte[] byteArray)
        {
            string result = System.Text.Encoding.UTF8.GetString(byteArray);
            return result;
        }

        /// <summary>
        /// Convierte Bitmap en BitmapImage
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        /// <summary>
        /// Convierte Bitmap en ByteArray
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <returns>byte[]</returns>
        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Convierte ByteArray en BitmapImage
        /// </summary>
        /// <param name="array">byte[]</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage ByteArrayToImage(byte[] array)
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

        public static byte[] BitmapImageToByteArray(BitmapImage bitmapImage)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        #endregion

        /// <summary>
        /// Crea una imagen Bitmap del thumbnail de un fichero
        /// </summary>
        /// <param name="fileName">Path del fichero</param>
        /// <returns>Bitmap</returns>
        public static BitmapImage CreateThumbnailBitmapImage(string fileName,ItemInfo info)
        {
            try
            {
                BitmapImage thumbnail = ZipHelper.Instance.UncompressToBitmapImage(fileName,info);
                return thumbnail;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en thumbnail: {0}",ex.Message);
                return null;
            }
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

        /// <summary>
        /// Crea una imagen Bitmap del thumbnail de un fichero
        /// </summary>
        /// <param name="fileName">Path del fichero</param>
        /// <returns>Bitmap</returns>
        public static List<ComicTemp> CreatePagesComic(string fileName,int firstpage)
        {
            try
            {
                if (firstpage == 0)
                {
                    List<ComicTemp> images = ZipHelper.Instance.UncompressToListPages(fileName);
                    return images;
                }
                else
                {
                    List<ComicTemp> images = ZipHelper.Instance.UncompressToListPages(fileName,firstpage);
                    return images;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error generando imagenes: {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Genera lista de directorios y subdirectorios (Método Interno)
        /// </summary>
        /// <param name="snapid">id del snap</param>
        /// <param name="sDir">Path de busqueda</param>
        /// <param name="folders">Lista de directorios</param>
        /// <param name="currentlevel">Nivel de profundidad busqueda</param>
        private static void SearchDirsInternal(string snapid,string sDir,List<ItemFolder> folders,int currentlevel)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    ItemFolder folder = new ItemFolder();
                    folder.Level = currentlevel;
                    folder.Path = d;
                    folder.SnapId = snapid;
                    folders.Add(folder);
                    SearchDirsInternal(snapid,d,folders,currentlevel+1);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Genera un GUID unico
        /// </summary>
        /// <returns>GUID en String</returns>
        public static String GenerateUniqueId()
        {
            Guid g = Guid.NewGuid();            
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            return GuidString;
        }

        /// <summary>
        /// Genera un GUID unico
        /// </summary>
        /// <returns>GUID en String</returns>
        public static Guid GenerateUniqueIdAsGUID()
        {
            Guid g = Guid.NewGuid();
            return g;
        }

        /// <summary>
        /// Genera lista de información de ficheros para una carpeta
        /// </summary>
        /// <param name="snapid">id del snap</param>
        /// <param name="sDir">Carpeta a buscar path string</param>
        /// <param name="folder">Elemento carpeta</param>
        /// <returns></returns>
        public static List<ItemInfo> SearchFiles(string snapid ,string sDir,ItemFolder folder)
        {
            List<ItemInfo> filesinfo = new List<ItemInfo>();
            try
            {
                var ext = new List<string> { ".cbr", ".cbz" };
                List<String> files = Directory.GetFiles(sDir, "*.*").Where(s => ext.Contains(Path.GetExtension(s).ToLower())).ToList();
                foreach (string f in files)
                {

                    ItemInfo item = new ItemInfo();
                    item.SnapId = snapid;
                    item.CoverByteArray = null;
                    item.CreationDate = DateTime.Now;
                    item.Path = f;
                    FileInfo fileInfo = new FileInfo(item.Path);                    
                    string directoryFullPath = fileInfo.DirectoryName;
                    item.DisplayName = System.IO.Path.GetFileNameWithoutExtension(item.Path);
                    item.SourceFolderPath = directoryFullPath;
                    string exte = System.IO.Path.GetExtension(f).ToLower();
                    if (exte == ".cbr")
                        item.Type = ItemType.CBR;
                    if (exte == ".cbz")
                        item.Type = ItemType.CBZ;
                    item.Size = fileInfo.Length;
                    filesinfo.Add(item);
                    Console.WriteLine(String.Format(f));
                }                
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return filesinfo;
        }

        /// <summary>
        /// Genera los datos de un nuevo snap
        /// </summary>
        /// <returns>Elemento Snap</returns>
        public static Snap GenerateNewSnap(String path,String filepath)
        {
            String snapid = MainUtils.GenerateUniqueId();
            //Create Snap
            Snap snap = new Snap();
            Guid id = Guid.NewGuid();
            snap.Id = MainUtils.GenerateUniqueIdAsGUID();
            snap.FolderPath = path;
            snap.FilePath = filepath;
            snap.CreationDate = DateTime.Now;
            snap.LastUpdateDate = DateTime.Now;
            snap.Description = String.Format("Snap created at {0}", DateTime.Now);
            snap.SnapId = snapid;
            return snap;
        }

        /// <summary>
        /// Genera una lista de elementos carpeta con su información
        /// </summary>
        /// <param name="path">Carpeta de donde buscar los elementos</param>
        /// <param name="snapid">id del snap</param>
        /// <returns></returns>
        public static List<ItemFolder> GenerateSearch(String path,String snapid)
        {
            List<ItemFolder> folders = new List<ItemFolder>();
            try
            {                
                List<ItemFolder> tmpfolders = new List<ItemFolder>();
                int currentlevel = 0;
                ItemFolder folder = new ItemFolder();
                folder.Id = MainUtils.GenerateUniqueIdAsGUID();
                folder.Level = currentlevel;
                folder.Path = path;
                folder.SnapId = snapid;
                folder.CreationDate = DateTime.Now;
                folder.LastUpdateDate = DateTime.Now;
                tmpfolders.Add(folder);
                MainUtils.SearchDirsInternal(snapid, path, tmpfolders, currentlevel + 1);
                tmpfolders = tmpfolders.OrderBy(f => f.Path).ToList();
                foreach (ItemFolder f in tmpfolders)
                {
                    f.Files = MainUtils.SearchFiles(snapid, f.Path, f);
                    f.Size = f.Files.Sum(file => file.Size);
                    f.FilesCount = f.Files.Count;
                }
                folders = tmpfolders.Where(f => f.FilesCount > 0).ToList();
                return folders;
            }catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error generating Search {0}", ex.Message);
                return new List<ItemFolder>();
            }
        }

        /// <summary>
        /// Almacenar ficheros
        /// </summary>
        /// <param name="file path"></param>
        public static List<String> LogRecents(string newpath)
        {            
            try
            {
                CommonApplicationData com = new CommonApplicationData("COMIX", "DATA");                
                var systemPath = com.ApplicationFolderPath;
                String LOG_PATH = String.Format("{0}\\COMIX_{1}", systemPath, "recents.dat");

                if (!File.Exists(LOG_PATH))
                {
                    FileStream fs = File.Create(LOG_PATH);
                    fs.Close();
                }
                
                    List<String> logFile = new List<string>();
                    logFile = File.ReadAllLines(LOG_PATH).ToList();
                    FileStream fs2 = File.Create(LOG_PATH);
                    fs2.Close();
                    if (logFile != null)
                    {
                        String sold = logFile.Where(f => f == newpath).FirstOrDefault();
                        if (!String.IsNullOrEmpty(sold))
                         logFile.Remove(sold);
                        if (logFile.Count > 10)
                         logFile.Remove(logFile.Last());
                        logFile.Insert(0, newpath);
                    }                    
                    File.AppendAllLines(LOG_PATH, logFile);
                    return logFile;
                                             
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        public static List<String> RemoveFromRecents(string newpath)
        {
            try
            {
                CommonApplicationData com = new CommonApplicationData("COMIX", "DATA");
                var systemPath = com.ApplicationFolderPath;
                String LOG_PATH = String.Format("{0}\\COMIX_{1}", systemPath, "recents.dat");

                if (!File.Exists(LOG_PATH))
                {
                    FileStream fs = File.Create(LOG_PATH);
                    fs.Close();
                }

                List<String> logFile = new List<string>();
                logFile = File.ReadAllLines(LOG_PATH).ToList();
                FileStream fs2 = File.Create(LOG_PATH);                
                fs2.Close();
                if (logFile != null)
                {
                    String sold = logFile.Where(f => f == newpath).FirstOrDefault();
                    if (!String.IsNullOrEmpty(sold))
                        logFile.Remove(sold);
                    if (logFile.Count > 10)
                        logFile.Remove(logFile.Last());                    
                }
                File.AppendAllLines(LOG_PATH, logFile);
                return logFile;

            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Leer ficheros
        /// </summary>
        /// <param name="file path"></param>
        public static List<String> ReadRecents()
        {
            try
            {
                CommonApplicationData com = new CommonApplicationData("COMIX", "DATA");
                var systemPath = com.ApplicationFolderPath;                
                String LOG_PATH = String.Format("{0}\\COMIX_{1}", systemPath, "recents.dat");
                if (File.Exists(LOG_PATH))
                    return File.ReadAllLines(LOG_PATH).ToList();
                else
                    return new List<string>();
            }
            catch (Exception ex)
            {                
                return new List<string>();
            }
        }
    }
}

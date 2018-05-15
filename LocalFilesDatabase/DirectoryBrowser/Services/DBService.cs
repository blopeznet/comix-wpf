using DirectoryBrowser.Entities;
using DirectoryBrowser.Utils;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace DirectoryBrowser
{
    /// <summary>
    /// Service class interaction database
    /// </summary>
    public class DBService
    {
        /// <summary>
        /// Instance service
        /// </summary>
        private static DBService _Instance;
        public static DBService Instance {
            get  {
                if (_Instance == null)
                {
                    _Instance = new DBService();
                    _Instance.Path = "MyComicFiles.cdb";
                }
                return _Instance;
            }
            set => _Instance = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DBService()
        {            
        }

        /// <summary>
        /// Path database
        /// </summary>
        private String _Path;
        public string Path
        {
            get => _Path;
            set
            {
                _Path = value;                
            }
        }    
        
        /// <summary>
        /// Save folder collection into database
        /// </summary>
        /// <param name="files"></param>
        public void SaveLastFolderCollection(List<FolderComicsInfo> files)
        {
            foreach (FolderComicsInfo folder in files)
            {
                SaveLastFolder(folder);
            }

           List<FolderComicsInfo>  folders = DBService.Instance.GetItemFolders();
        }

        /// <summary>
        /// Generate cover for list folders
        /// </summary>
        /// <param name="files">Generate cover for list folders</param>
        public void GenerateCoversMemorySteam(List<FolderComicsInfo> files,int sourcethumb)
        {
            bool updated = false;
            foreach (FolderComicsInfo folder in files.Where(f => f.CoverExists == false).ToList())
            {

                MemoryStream img = new MemoryStream();

                if (sourcethumb == 0) //From file
                    img = ZipHelper.Instance.UncompressToMemoryStream(folder.FileNameFirst, folder, folder.FolderName);
                else if (sourcethumb == 1)
                { //From system
                    System.Drawing.Bitmap bmp = UtilsApp.CreateThumbnail(folder.FileNameFirst);
                    bmp.Save(img, System.Drawing.Imaging.ImageFormat.Png);
                }

                try
                {
                    var imageResizer = new Simple.ImageResizer.ImageResizer(img.ToArray());
                    byte[] resized = imageResizer.Resize(256, Simple.ImageResizer.ImageEncoding.Jpg).ToArray();
                    Stream stream = new MemoryStream(resized);

                    using (var db = new LiteDatabase(Path))
                    {
                        db.FileStorage.Upload(folder.Id.ToString(), folder.Id.ToString(), stream);
                    }

                }
                catch { }

                folder.CoverExists = true;
                updated = true;

            }

            if (updated)
            {
                int count = App.ViewModel.AllFolders.Count(f => f.CoverExists == false);
                App.ViewModel.StatusMsg = String.Format("Quedan {0} miniaturas por generar.", count);

                DBService.Instance.SaveLastFolderCollection(files);
                App.ViewModel.RaisePropertyChanged("LasFolders");
                App.ViewModel.RaisePropertyChanged("SelectedFolder");
            }
        }

        #region Resize MemoryStream

        private MemoryStream ResizeImage(MemoryStream streamPhoto)
        {                      
                BitmapFrame bfPhoto = ReadBitmapFrame(streamPhoto);

                int nThumbnailSize = 200, nWidth, nHeight;
                if (bfPhoto.Width > bfPhoto.Height)
                {
                    nWidth = nThumbnailSize;
                    nHeight = (int)(bfPhoto.Height * nThumbnailSize / bfPhoto.Width);
                }
                else
                {
                    nHeight = nThumbnailSize;
                    nWidth = (int)(bfPhoto.Width * nThumbnailSize / bfPhoto.Height);
                }
                BitmapFrame bfResize = FastResize(bfPhoto, nWidth, nHeight);
                return ToMemoryStream(bfResize);                
        }

        private MemoryStream ToMemoryStream(BitmapFrame bitmapFrame)
        {
            if (bitmapFrame != null)

            {

                PngBitmapEncoder png = new PngBitmapEncoder();

                png.Frames.Add(bitmapFrame);

                using (MemoryStream mem = new MemoryStream())

                {

                    png.Save(mem);
                    return mem;

                }
            }

            return new MemoryStream();
        }

        private  BitmapFrame FastResize(BitmapFrame bfPhoto, int nWidth, int nHeight)
        {
            TransformedBitmap tbBitmap = new TransformedBitmap(bfPhoto, new System.Windows.Media.ScaleTransform(nWidth / bfPhoto.Width, nHeight / bfPhoto.Height, 0, 0));
            return BitmapFrame.Create(tbBitmap);
        }

        private  byte[] ToByteArray(BitmapFrame bfResize)
        {
            using (MemoryStream msStream = new MemoryStream())
            {
                PngBitmapEncoder pbdDecoder = new PngBitmapEncoder();
                pbdDecoder.Frames.Add(bfResize);
                pbdDecoder.Save(msStream);
                return msStream.ToArray();
            }
        }

        private  BitmapFrame ReadBitmapFrame(Stream streamPhoto)
        {
            BitmapDecoder bdDecoder = BitmapDecoder.Create(streamPhoto, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            return bdDecoder.Frames[0];
        }
        
        #endregion

        /// <summary>
        /// Get bitmap image for folder comic info
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public BitmapImage LoadFile(FolderComicsInfo info)
        {
            using (var db = new LiteDatabase(Path))
            {

            LiteFileInfo file = db.FileStorage.FindById(info.Id.ToString());
            if (file!=null)
                {
                    try
                    {
                        using (var fStream = file.OpenRead())
                        {
                            var ms = new MemoryStream();
                            fStream.CopyTo(ms);
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            ms.Position = 0;
                            ms.Seek(0, SeekOrigin.Begin);
                            bitmap.DecodePixelHeight = 384;
                            bitmap.DecodePixelWidth = 256;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = ms;
                            bitmap.EndInit();
                            bitmap.Freeze();
                            return bitmap;

                        }
                    }catch 
                    {
                        return new BitmapImage();
                    }
                }

            }

            return new BitmapImage();
        }
              
        /// <summary>
        /// Add folder to db
        /// </summary>
        /// <param name="newsnap">new snap</param>
        public void SaveLastFolder(FolderComicsInfo newitem)
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<FolderComicsInfo>("folders");
                FolderComicsInfo old = items.Find(Query.EQ("FolderName", newitem.FolderName)).OrderBy(x => x.FolderName).FirstOrDefault();
                if (old != null)
                    items.Delete(old.Id);
                if (newitem != null)
                 items.Insert(newitem);
            }
        }
        
        /// <summary>
        /// Get collection folders
        /// </summary>
        /// <returns>Directory list</returns>
        public List<FolderComicsInfo> GetItemFolders()
        {
            if (String.IsNullOrEmpty(Path))
                throw new Exception("Database path not exist");

            using (var db = new LiteDatabase(Path))
            {
                var items = db.GetCollection<FolderComicsInfo>("folders");
                return items.FindAll().ToList();
            }
        }

    }
}

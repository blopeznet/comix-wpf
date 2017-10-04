using System;
using System.IO;
using System.Linq;
using SevenZip;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Threading;
using LocalFilesDatabase.Entities;
using System.ComponentModel;

namespace LocalFilesDatabase.Utils
{
    public class ZipHelper
    {
        #region ----------------SINGLETON----------------
        public static readonly ZipHelper Instance = new ZipHelper();

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private ZipHelper()
        {

            try
            {
                //be sure 7zip is initialized
                if (Environment.Is64BitOperatingSystem && Environment.Is64BitProcess)
                    SevenZipExtractor.SetLibraryPath(DirectoryHelper.Combine(CBRFolders.Dependencies, "7z64.dll"));
                else
                    SevenZipExtractor.SetLibraryPath(DirectoryHelper.Combine(CBRFolders.Dependencies, "7z.dll"));
            }
            catch (Exception err)
            {
            }
            finally
            {
            }
        }

        #endregion

        /// <summary>
        /// ask for an extractor
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public SevenZipExtractor GetExtractor(string filePath)
        {
            SevenZipExtractor temp = null;


            try
            {
                temp = new SevenZipExtractor(filePath);
            }
            catch (Exception err)
            {
                ReleaseExtractor(temp);

            }
            finally
            {

            }

            return temp;
        }

        /// <summary>
        /// release the given extracor
        /// </summary>
        /// <param name="extractor"></param>
        public void ReleaseExtractor(SevenZipExtractor extractor)
        {
            try
            {
                if (extractor != null)
                {
                    extractor.Dispose();
                    extractor = null;
                }
            }
            catch (Exception err)
            {

            }
            finally
            {

            }
        }

        #region ----------------FOLDERS----------------

        /// <summary>
        /// self compress a folder to a content
        /// </summary>
        /// <param name="outputFileName"></param>
        /// <param name="inputFolder"></param>
        /// <param name="resultCount"></param>
        /// <returns></returns>
        public bool CompressFolder(string outputFileName, string inputFolder, out int resultCount)
        {
            SevenZip.SevenZipCompressor cp = null;

            try
            {
                cp = new SevenZip.SevenZipCompressor();
                cp.ArchiveFormat = SevenZip.OutArchiveFormat.Zip;

                string[] outputFiles = new DirectoryInfo(inputFolder).GetFiles("*.*").Select(p => p.FullName).ToArray();

                using (FileStream fs = new FileStream(outputFileName, FileMode.Create))
                {
                    cp.CompressFiles(fs, outputFiles);
                }

                resultCount = outputFiles.Count();
                return true;
            }
            catch (Exception err)
            {
                resultCount = 0;
                return false;
            }
            finally
            {
                cp = null;
            }
        }

        /// <summary>
        /// self uncompress a content to folder
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFolder"></param>
        /// <param name="resultCount"></param>
        /// <returns></returns>
        public bool UncompressToFolder(string inputFile, string outputFolder, out int resultCount)
        {
            SevenZipExtractor temp = null;

            try
            {
                temp = GetExtractor(inputFile);

                DirectoryHelper.Check(outputFolder);

                temp.PreserveDirectoryStructure = false;
                temp.ExtractArchive(outputFolder);
                resultCount = temp.ArchiveFileData.Count;

                return true;
            }
            catch (Exception err)
            {
                resultCount = 0;
                return false;
            }
            finally
            {
                ReleaseExtractor(temp);
            }
        }

        /// <summary>
		/// self uncompress a content to BitmapImage
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
        public BitmapImage UncompressToBitmapImage(string inputFile,ItemInfo info)
        {
            SevenZipExtractor temp = null;

            try
            {
                temp = GetExtractor(inputFile);
                BitmapImage bitmap = null;
                List<ArchiveFileInfo> list = temp.ArchiveFileData.OrderBy(f => f.FileName).ToList();
                info.TotalPages = list.Where(f => (f.FileName.EndsWith(".jpg") || f.FileName.EndsWith(".png"))).Count();
                String filenamepng = list.Where(f => (f.FileName.EndsWith(".jpg") || f.FileName.EndsWith(".png"))).FirstOrDefault().FileName;
                using (MemoryStream ms = new MemoryStream())
                {
                    temp.ExtractFile(filenamepng, ms);
                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    ms.Position = 0;
                    bitmap.StreamSource = ms;
                    bitmap.DecodePixelHeight = 192;
                    bitmap.DecodePixelWidth = 128;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
                return bitmap;
            }
            catch (Exception err)
            {

                String uri = DirectoryHelper.Combine(CBRFolders.Dependencies, "NotFound.jpg");
                var bmp = new BitmapImage(new Uri(uri, UriKind.Relative));
                return bmp;
            }
            finally
            {
                ReleaseExtractor(temp);
            }
        }

        /// <summary>
		/// self uncompress a content to List to BitmapImage
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
        public List<ComicTemp> UncompressToListPages(string inputFile)
        {
            SevenZipExtractor temp = null;

            try
            {
                temp = GetExtractor(inputFile);
                List<ArchiveFileInfo> list = temp.ArchiveFileData.OrderBy(f => f.FileName).ToList();
                App.ViewModel.WorkingMsg = String.Format("GENERANDO PAGINAS...");
                List<ComicTemp> pages = new List<ComicTemp>();
                int pageno = 1;
                foreach (ArchiveFileInfo file in list.Where(f => (f.FileName.EndsWith(".jpg") || f.FileName.EndsWith(".png"))).ToList())
                {
                    ComicTemp tmppng = new ComicTemp() { Source = file.FileName, NoPage = pageno };
                    pages.Add(tmppng);
                    pageno += 1;
                }
                App.ViewModel.TotalPagesLoaded = pageno-1;

                foreach (ComicTemp p in pages.Take(4))
                    UncompressImage(temp, p);


                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += delegate (object s, DoWorkEventArgs args)
                {
                    SevenZipExtractor tmpextractor = GetExtractor(inputFile);
                    foreach (ComicTemp p in ((List<ComicTemp>)args.Argument).Skip(4))
                    {
                        UncompressImage(tmpextractor, p);

                    }
                    ReleaseExtractor(tmpextractor);

                };

                worker.RunWorkerAsync(pages);
                return pages.ToList();
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("Error al generar páginas {0}", err.Message);
                return null;
            }
            finally
            {
                ReleaseExtractor(temp);
            }
        }



        private void UncompressImage(SevenZipExtractor extractor, ComicTemp tmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                tmp.Loaded = true;
                extractor.ExtractFile(tmp.Source, ms);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                ms.Position = 0;
                bitmap.StreamSource = ms;
                bitmap.DecodePixelWidth = 1368;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                App.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    tmp.Image = bitmap;
                }));
            }
        }

        #endregion


    }
}

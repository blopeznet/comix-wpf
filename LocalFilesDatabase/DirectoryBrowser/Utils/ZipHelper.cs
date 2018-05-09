using System;
using System.IO;
using System.Linq;
using SevenZip;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Threading;
using System.ComponentModel;
using System.Drawing;
using DirectoryBrowser.Entities;

namespace DirectoryBrowser.Utils
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
        public BitmapImage UncompressToBitmapImage(string inputFile, FolderComicsInfo info,String DirectoryPath)
        {
            SevenZipExtractor temp = null;

            try
            {
                temp = GetExtractor(inputFile);
                BitmapImage bitmap = null;                
                List<ArchiveFileInfo> list = temp.ArchiveFileData.OrderBy(f => f.FileName).ToList();                
                ArchiveFileInfo filenamepng = 
                    list.Where(f => (f.FileName.EndsWith(".jpg") || f.FileName.EndsWith(".png")) 
                    && (f.FileName.Contains("MACOSX")==false) 
                    && (f.Size>0) 
                    && (f.IsDirectory == false)).FirstOrDefault();
                using (MemoryStream ms = new MemoryStream())
                {
                    temp.ExtractFile(filenamepng.Index, ms);       
                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    ms.Position = 0;
                    ms.Seek(0, SeekOrigin.Begin);
                    bitmap.DecodePixelHeight = 384;
                    bitmap.DecodePixelWidth = 256;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    //Save image for test
                    //String path = DirectoryPath +  "\\snap"+System.IO.Path.GetExtension(filenamepng);
                    //Save(bitmap,path);   

                    return bitmap;
                }
                return bitmap;
            }
            catch (Exception err)
            {
                var bmp = new BitmapImage();
                return bmp;
            }
            finally
            {
                ReleaseExtractor(temp);
            }
        }


        public MemoryStream UncompressToMemoryStream(string inputFile, FolderComicsInfo info, String DirectoryPath)
        {
            SevenZipExtractor temp = null;

            try
            {
                temp = GetExtractor(inputFile);
                List<ArchiveFileInfo> list = temp.ArchiveFileData.OrderBy(f => f.FileName).ToList();
                ArchiveFileInfo filenamepng =
                    list.Where(f => (f.FileName.EndsWith(".jpg") || f.FileName.EndsWith(".png"))
                    && (f.FileName.Contains("MACOSX") == false)
                    && (f.Size > 0)
                    && (f.IsDirectory == false)).FirstOrDefault();
                MemoryStream ms = new MemoryStream();
                temp.ExtractFile(filenamepng.Index, ms);                    
                ms.Position = 0;
                ms.Seek(0, SeekOrigin.Begin);
                return ms;                                                    
            }
            catch (Exception err)
            {
                MemoryStream ms = new MemoryStream();
                return ms;
            }
            finally
            {
                ReleaseExtractor(temp);
            }
        }

        public List<ComicTemp> UncompressToComicPageCollection(string inputFile)
        {
            SevenZipExtractor temp = null;
            List<ComicTemp> mscollection = new List<ComicTemp>();
            try
            {
                temp = GetExtractor(inputFile);
                List<ArchiveFileInfo> list = temp.ArchiveFileData.OrderBy(f => f.CreationTime).OrderBy(f=>f.FileName).ToList();
                List<ArchiveFileInfo> filenamepngs =
                    list.Where(f => (f.FileName.EndsWith(".jpg") || f.FileName.EndsWith(".png"))
                    && (f.FileName.Contains("MACOSX") == false)
                    && (f.Size > 0)
                    && (f.IsDirectory == false)).ToList();

                int nopage = 1;
                foreach(ArchiveFileInfo file in filenamepngs)
                {
                    MemoryStream ms = new MemoryStream();
                    temp.ExtractFile(file.Index, ms);
                    ms.Position = 0;
                    ms.Seek(0, SeekOrigin.Begin);
                    mscollection.Add(new ComicTemp() { Source = ms,Loaded=false, NoPage=nopage,info = file});
                    nopage++;
                }
                
                return mscollection;
            }
            catch (Exception err)
            {
                List<ComicTemp> mscoll = new List<ComicTemp>();
                return mscoll;
            }
            finally
            {
                ReleaseExtractor(temp);
            }
        }


        private void Save(BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        /// <summary>


        #endregion


    }
}

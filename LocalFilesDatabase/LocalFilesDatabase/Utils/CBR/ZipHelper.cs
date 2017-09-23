using System;
using System.IO;
using System.Linq;
using SevenZip;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Threading;

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
        public SevenZipExtractor GetExtractor( string filePath )
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
        public BitmapImage UncompressToBitmapImage(string inputFile)
        {
            SevenZipExtractor temp = null;

            try
            {
                temp = GetExtractor(inputFile);
                BitmapImage bitmap = null;                
                List<ArchiveFileInfo> list = temp.ArchiveFileData.OrderBy(f => f.FileName).ToList();

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
                var bmp = new BitmapImage(new Uri(uri,UriKind.Relative));
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
        public List<BitmapImage> UncompressToListBitmapImages(string inputFile)
        {
            SevenZipExtractor temp = null;

            try
            {
                temp = GetExtractor(inputFile);
                BitmapImage bitmap = null;
                List<ArchiveFileInfo> list = temp.ArchiveFileData.OrderBy(f => f.FileName).ToList();
                List<ArchiveFileInfo> filenamespng = list.Where(f => (f.FileName.EndsWith(".jpg") || f.FileName.EndsWith(".png"))).ToList();
                List<BitmapImage> pages = new List<BitmapImage>();
                int processed = 1;
                App.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    App.ViewModel.WorkingMsg = String.Format("GENERANDO {0} DE {1} PAGINAS",processed,filenamespng.Count-1);
                }));

                foreach (ArchiveFileInfo file in filenamespng)
                {
                    String filenamepng = file.FileName;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        temp.ExtractFile(filenamepng, ms);
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        ms.Position = 0;
                        bitmap.StreamSource = ms;
                        bitmap.DecodePixelWidth = 1368;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        pages.Add(bitmap);
                        App.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                        {
                            App.ViewModel.WorkingMsg = String.Format("GENERANDO {0} DE {1} PAGINAS", processed, filenamespng.Count - 1);
                        }));
                        processed += 1;
                    }
                }                                
                return pages;
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

        #endregion


    }
}

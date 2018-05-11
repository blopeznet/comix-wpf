using DirectoryBrowser.Entities;
using DirectoryBrowser.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DirectoryBrowser.ViewModel
{
    public partial class MainViewModel        
    {

        #region and Variables

        /// <summary>
        /// Progress generation images for pages reader
        /// </summary>
        private Double _ProgressLoad;
        public Double ProgressLoad
        {
            get => _ProgressLoad;
            set
            {

                _ProgressLoad = value;
                if (_ProgressLoad >= 100)
                    ComicLoaded = true;
                RaisePropertyChanged("ProgressLoad");
            }

        }

        /// <summary>
        /// Flag viewer all pages are loaded
        /// </summary>
        private bool _ComicLoaded;
        public bool ComicLoaded
        {
            get => _ComicLoaded;
            set
            {

                _ComicLoaded = value;
                RaisePropertyChanged("ComicLoaded");
            }
        }

        /// <summary>
        /// Flag using fullscreen
        /// </summary>
        private bool _UseFullScreen;
        public bool UseFullScreen
        {
            get => _UseFullScreen;
            set
            {

                _UseFullScreen = value;
                RaisePropertyChanged("UseFullScreen");
            }
        }

        /// <summary>
        /// Collection pages viewer
        /// </summary>
        private List<ComicTemp> _Pages;
        public List<ComicTemp> Pages
        {
            get => _Pages;
            set
            {

                _Pages = value;
                RaisePropertyChanged("Pages");
            }
        }

        /// <summary>
        /// Timer background worker images comic
        /// </summary>
        DispatcherTimer dispatcherTimerPages = new System.Windows.Threading.DispatcherTimer();

        /// <summary>
        /// Background images comic updating
        /// </summary>
        private readonly BackgroundWorker workerPages = new BackgroundWorker();

        /// <summary>
        /// Update image comics viewer by count each 10 seconds
        /// </summary>
        private int eachbypages = 3;


        #endregion

        /// <summary>
        /// Method init reader
        /// </summary>
        /// <param name="path">Path file viewer</param>
        public void InitReader(String path)
        {
            Pages = ZipHelper.Instance.UncompressToComicPageCollection(path);
            UpdateCover(Pages.FirstOrDefault());
            ComicLoaded = false;
            ProgressLoad = 1;            
            dispatcherTimer.Tick += new EventHandler(dispatcherTimerPages_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Dispatcher timer update background images comic viewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimerPages_Tick(object sender, EventArgs e)
        {
            UpdatePages();
        }

        /// <summary>
        /// Method who runs backgroundworker
        /// </summary>
        /// <returns></returns>
        private async Task UpdatePages()
        {
            workerPages.DoWork += workerPages_DoWork;
            workerPages.RunWorkerAsync();
        }

        /// <summary>
        /// Work event for upgrade images comic viewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void workerPages_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            if (App.ViewModel.Pages != null && App.ViewModel.Pages.Count > 0)
            {

                int count = App.ViewModel.Pages.Count(f => f.Loaded == false);                                
                if (count > 0)
                {
                    UpdateSources(App.ViewModel.Pages.Where(f => f.Loaded == false).Take(eachbycomics).OrderBy(f => f.NoPage).ToList());
                }
                else
                    ProgressLoad = 100;                                
            }
           
        }

        
        /// <summary>
        /// Update first image from file to viewer
        /// </summary>
        /// <param name="page"></param>
        public void UpdateCover(ComicTemp page)
        {
            try
            {                
                    var imageResizer = new Simple.ImageResizer.ImageResizer(page.Source.ToArray());
                    byte[] resized = imageResizer.Resize(256, Simple.ImageResizer.ImageEncoding.Jpg).ToArray();
                    Stream stream = new MemoryStream(resized);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = page.Source;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    page.Image = bitmap;
                    page.Loaded = true;                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Update all images from file to viewer
        /// </summary>
        /// <param name="pages"></param>
        public void UpdateSources(List<ComicTemp> pages)
        {
            
            try
            {
                
                foreach (ComicTemp page in pages)
                {
                    var imageResizer = new Simple.ImageResizer.ImageResizer(page.Source.ToArray());
                    byte[] resized = imageResizer.Resize(256, Simple.ImageResizer.ImageEncoding.Jpg).ToArray();
                    Stream stream = new MemoryStream(resized);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = page.Source;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    page.Image = bitmap;
                    page.Loaded = true;

                    int loaded = App.ViewModel.Pages.Count(f => f.Loaded == true);
                    int total = App.ViewModel.Pages.Count;
                    ProgressLoad = (100 * loaded) / total;

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }            
        }
    }
}

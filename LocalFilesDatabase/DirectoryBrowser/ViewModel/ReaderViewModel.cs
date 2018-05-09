using DirectoryBrowser.Entities;
using DirectoryBrowser.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DirectoryBrowser.ViewModel
{
    public partial class MainViewModel        
    {

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


        public void InitReader(String path)
        {
            Pages = ZipHelper.Instance.UncompressToComicPageCollection(path);
            UpdateCover(Pages.FirstOrDefault());
            dispatcherTimer.Tick += new EventHandler(dispatcherTimerPages_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();
        }


        DispatcherTimer dispatcherTimerPages = new System.Windows.Threading.DispatcherTimer();
        private readonly BackgroundWorker workerPages = new BackgroundWorker();

        private void dispatcherTimerPages_Tick(object sender, EventArgs e)
        {
            UpdatePages();
        }

        private async Task UpdatePages()
        {
            workerPages.DoWork += workerPages_DoWork;
            workerPages.RunWorkerAsync();
        }

        private int eachbypages = 3;

        private void workerPages_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            if (App.ViewModel.Pages != null && App.ViewModel.Pages.Count > 0)
            {

                int count = App.ViewModel.Pages.Count(f => f.Loaded == false);
                //App.ViewModel.StatusMsg = String.Format("Quedan {0} miniaturas por generar.", count);
                if (count > 0)
                {
                    UpdateSources(App.ViewModel.Pages.Where(f => f.Loaded == false).Take(eachbycomics).OrderBy(f => f.NoPage).ToList());
                    if ((count > eachbycomics))
                        App.ViewModel.StatusMsg = String.Format("Quedan {0} miniaturas por generar...", count - eachbycomics);
                }
                else
                    App.ViewModel.StatusMsg = String.Format("Todas las miniaturas han sido generadas.");

            }

        }

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
                }                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }            
        }
    }
}

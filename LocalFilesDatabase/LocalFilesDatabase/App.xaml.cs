using LocalFilesDatabase.Entities;
using LocalFilesDatabase.ViewModel;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace LocalFilesDatabase
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }

        

        protected override void OnStartup(StartupEventArgs e)
        {         
            base.OnStartup(e);
        }


        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            App.ViewModel.IsWorking = true;
            App.ViewModel.WorkingMsg = ("LAUNCHING APP");
            App.ViewModel.usefullscreen = false;
            App.ViewModel.ShowScrollBar = System.Windows.Controls.ScrollBarVisibility.Auto;
            App.ViewModel.RecentFiles = MainUtils.ReadRecents();
            if (App.ViewModel.RecentFiles.Count > 0){
                if (System.IO.File.Exists(App.ViewModel.RecentFiles[0]))
                {
                    await App.ViewModel.OpenFileLogic(App.ViewModel.RecentFiles[0]);
                    ItemInfo last = DBService.Instance.GetLastComic();
                    if (last != null)
                    {
                        App.ViewModel.SelectedFile = last;
                        await App.ViewModel.ShowReader(App.ViewModel.SelectedFile.Path, (MetroWindow)App.Current.MainWindow, App.ViewModel.SelectedFile.CurrentPages);
                    }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                App.ViewModel.WorkingMsg = "Loading recents...";
                await Task.Delay(100);
            }

            //// Navigate to xaml page                        
            App.ViewModel.WorkingMsg = String.Empty;
            App.ViewModel.IsWorking = false;
        }
        
        public static MainViewModel ViewModel
        {
            get
            {
                MainViewModel vm = ((ViewModelLocator)Application.Current.Resources["Locator"]).Main;
                return vm;
            }
        }

        private async void Comic_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left && e.ClickCount == 2) {

                Entities.ItemInfo info = (Entities.ItemInfo)(((System.Windows.Controls.Grid)sender).DataContext);
                if (System.IO.File.Exists(info.Path))
                {
                    DBService.Instance.SaveLastComic(info);
                    await App.ViewModel.ShowReader(info.Path, (MahApps.Metro.Controls.MetroWindow)App.Current.MainWindow, info.CurrentPages);
                }
            }            
        }
    }
}

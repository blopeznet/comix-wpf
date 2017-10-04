using LocalFilesDatabase.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
            App.ViewModel.RecentFiles = MainUtils.ReadRecents();
            if (App.ViewModel.RecentFiles.Count > 0){
                if (System.IO.File.Exists(App.ViewModel.RecentFiles[0]))
                    await App.ViewModel.OpenFileLogic(App.ViewModel.RecentFiles[0]);
            }
            for (int i = 0; i < 10; i++)
            {
                App.ViewModel.WorkingMsg = "Loading recents...";
                await Task.Delay(100);
            }
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

        private async void GridCover_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left && e.ClickCount == 2) {

                Entities.ItemInfo info = (Entities.ItemInfo)(((System.Windows.Controls.Grid)sender).DataContext);
                await App.ViewModel.ShowReader(info.Path,(MahApps.Metro.Controls.MetroWindow)App.Current.MainWindow);
            }
        }
    }
}

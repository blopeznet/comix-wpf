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


        [STAThread]
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Launch();
        }

         static void Launch()
        {

            System.Threading.Thread t = new System.Threading.Thread(async () => await LoadMain());
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        public static async Task LoadMain()
        {
            await Application.Current.Dispatcher.Invoke(
               async () =>
               {

                   MainWindow m = new LocalFilesDatabase.MainWindow();
                   m.Show();
                   App.ViewModel.IsWorking = true;
                   App.ViewModel.WorkingMsg = ("LAUNCHING APP");
                   App.ViewModel.RecentFiles = MainUtils.ReadRecents();
                   for (int i = 0; i < 10; i++)
                   {
                       App.ViewModel.WorkingMsg = "Loading recents...";
                       await Task.Delay(1);
                   }
                   App.ViewModel.WorkingMsg = String.Empty;
                   App.ViewModel.IsWorking = false;
               });

        }
        
        public static MainViewModel ViewModel
        {
            get
            {
                MainViewModel vm = ((ViewModelLocator)Application.Current.Resources["Locator"]).Main;
                return vm;
            }
        }
    }
}

using DirectoryBrowser.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DirectoryBrowser
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {



        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            
            var mainWindow = new MainWindow();            
            mainWindow.Show();

            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null)
            {
                if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null &&
                    AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Count()>0)
                {
                    String arg = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0];
                    if (!String.IsNullOrEmpty(arg))
                    {
                        App.ViewModel.FilterMsg = arg;
                        await mainWindow.OpenFileFromArg(arg);
                        App.ViewModel.FilterMsg = String.Empty;
                    }
                }
            }
        }


        public static MainViewModel ViewModel
        {
            get
            {
                if (Application.Current != null)
                {
                    MainViewModel vm = ((ViewModelLocator)Application.Current.Resources["Locator"]).Main;
                    return vm;
                }
                else
                    return new MainViewModel();
            }
        }
    }
}

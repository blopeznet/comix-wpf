using DirectoryBrowser.ViewModel;
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

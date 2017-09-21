using LocalFilesDatabase.Entities;
using LocalFilesDatabase.Utils;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalFilesDatabase
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();            
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.OpenFileDialog();
        }       

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var dialogfile = new System.Windows.Forms.SaveFileDialog())
            {
                dialogfile.Filter = "Database File (.db)|*.db";
                dialogfile.FileName = DateTime.Now.ToString("yyyyMMdd_hhss")+".db";
                System.Windows.Forms.DialogResult resultsave = dialogfile.ShowDialog();
                if (resultsave == System.Windows.Forms.DialogResult.OK)
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            String path = dialog.SelectedPath;
                            DBService.Instance.Path = dialogfile.FileName;
                            await
                                App.ViewModel.AddNewSnap(path);
                            await
                                App.ViewModel.LoadData(DBService.Instance.Path);
                        }
                    }

                }
            }

        }        

        private async void buttonLaunchSearch_Click(object sender, RoutedEventArgs e)
        {
            await App.ViewModel.SearchCollectionIntoDatabase();
        }

        private async void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings settings = new MetroDialogSettings();
            settings.ColorScheme = MetroDialogColorScheme.Theme;
            settings.AffirmativeButtonText = "OK";
            settings.NegativeButtonText = "CANCELAR";
            MessageDialogResult result = await this.ShowMessageAsync("CERRAR", "DESEAS CERRAR LA BASE DE MINIATURAS?", MessageDialogStyle.AffirmativeAndNegative,settings);
            if (result == MessageDialogResult.Affirmative)
            {
                App.ViewModel.CloseDataBase();
            }
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings settings = new MetroDialogSettings();
            settings.ColorScheme = MetroDialogColorScheme.Theme;
            settings.AffirmativeButtonText = "OK";
            settings.NegativeButtonText = "CANCELAR";
            MessageDialogResult result = await this.ShowMessageAsync("CERRAR", "DESEAS ELIMINAR LA BASE DE MINIATURAS?", MessageDialogStyle.AffirmativeAndNegative, settings);
            if (result == MessageDialogResult.Affirmative)
            {
                String path = DBService.Instance.Path;
                App.ViewModel.CloseDataBase();
                File.Delete(path);
                App.ViewModel.RecentFiles = MainUtils.RemoveFromRecents(path);
            }
        }

        private async void buttonFile_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings settings = new MetroDialogSettings();
            settings.ColorScheme = MetroDialogColorScheme.Theme;
            settings.AffirmativeButtonText = "OK";
            if (System.IO.File.Exists(App.ViewModel.SelectedFile.Path)){
                Process.Start(App.ViewModel.SelectedFile.Path);
            }
            else
                await this.ShowMessageAsync("ABRIR FICHERO", "EL FICHERO NO SE ENCUENTRA DISPONIBLE.");
        }

        private async void buttonFolder_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings settings = new MetroDialogSettings();
            settings.ColorScheme = MetroDialogColorScheme.Theme;
            settings.AffirmativeButtonText = "OK";
            if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(App.ViewModel.SelectedFile.Path)))
            {
                Process.Start(App.ViewModel.SelectedFolder.Path);
            }
            else
                await this.ShowMessageAsync("ABRIR DIRECTORIO", "EL DIRECTORIO NO SE ENCUENTRA DISPONIBLE.");
        }

        private async void buttonfast_Click(object sender, RoutedEventArgs e)
        {
            String path = ((Button)sender).Content.ToString();
            if (System.IO.File.Exists(path))
            {                
                await App.ViewModel.LoadData(path); 
            }
            else {

                MetroDialogSettings settings = new MetroDialogSettings();
                settings.ColorScheme = MetroDialogColorScheme.Theme;
                settings.AffirmativeButtonText = "OK";
                settings.NegativeButtonText = "CANCELAR";
                MessageDialogResult result = await this.ShowMessageAsync("ABRIR FICHERO", "EL FICHERO NO SE ENCUENTRA DISPONIBLE, DESEA ELIMINAR DE LA LISTA?", MessageDialogStyle.AffirmativeAndNegative, settings);
                if (result == MessageDialogResult.Affirmative)
                {                    
                    App.ViewModel.RecentFiles = MainUtils.RemoveFromRecents(path);
                }                
            }
            
        }
    }
}

using DirectoryBrowser.Entities;
using DirectoryBrowser.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace DirectoryBrowser
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
       
        public MainWindow()
        {
            InitializeComponent();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MenuToggleButton.IsChecked = true;
        }


        private void tv_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MyTreeViewItem item = (MyTreeViewItem)e.NewValue;       
            if (item!=null)
            App.ViewModel.SelectedFolder = item.Tag;
            
        }



        private async Task<bool> OpenFileDialog(String path)
        {                        
           DBService.Instance.Path = path;
           App.ViewModel.FilterMsg = String.Empty;
           App.ViewModel.StatusMsg = String.Empty;
           List<FolderComicsInfo> items = DBService.Instance.GetItemFolders();
           App.ViewModel.AllFolders =  items.OrderBy(i=>i.FolderName).ToList();
           App.ViewModel.LasFolders = App.ViewModel.AllFolders.OrderByDescending(i => i.Date).Take(10).ToList();
           App.ViewModel.SelectedFolder = App.ViewModel.AllFolders.FirstOrDefault();           
           App.ViewModel.sourceCollection = await App.ViewModel.PopulateTreeView(App.ViewModel.AllFolders, '\\');
           App.ViewModel.showMenu = false;
           return true;
        }



        private async Task<bool> OpenFolderDialog(String path)
        {

            String namedbfile  = 
                String.Format("{0}_{1}.db", 
                System.IO.Path.GetFileName(path),
                DateTime.Now.ToString("yyyyddMM_HHmm"));


            String filename = System.AppDomain.CurrentDomain.BaseDirectory+ "MyData\\" + namedbfile;
            App.ViewModel.FilterMsg = String.Empty;
            App.ViewModel.StatusMsg = String.Empty;
            System.IO.Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "MyData\\");               
            App.ViewModel.IsWorking = true;
            App.ViewModel.WorkingMsg = "GENERANDO LIBRERIA...";                
            DBService.Instance.Path = filename;
            List<FolderComicsInfo> items = new List<FolderComicsInfo>();
            items = PShellHelper.Instance.GenerateIndexCollection(path);
            if (items != null && items.Count > 0)
                DBService.Instance.SaveLastFolderCollection(items);
            App.ViewModel.AllFolders = items.OrderBy(f=>f.FolderName).ToList();
            App.ViewModel.LasFolders = App.ViewModel.AllFolders.OrderByDescending(i=>i.Date).Take(10).ToList();
            App.ViewModel.SelectedFolder = App.ViewModel.AllFolders.FirstOrDefault();
            App.ViewModel.sourceCollection = await App.ViewModel.PopulateTreeView(App.ViewModel.AllFolders, '\\');                                
            App.ViewModel.IsWorking = false;               
            App.ViewModel.WorkingMsg = String.Empty;
            App.ViewModel.showMenu = false;
            return true;
                        
        }     

        private async void buttonAbrir_Click(object sender, RoutedEventArgs e)
        {
            
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Database File (.db)|*.db";
            System.Windows.Forms.DialogResult res = openFileDialog.ShowDialog();
            string path = openFileDialog.FileName;
            if (!String.IsNullOrEmpty(path) && res == System.Windows.Forms.DialogResult.OK)
            {                                
                await Task.Run(() =>
                {
                    App.ViewModel.IsWorking = true;
                    OpenFileDialog(path);
                    App.ViewModel.IsWorking = false;
                });                               
            }
        }

        private async void buttonCrear_Click(object sender, RoutedEventArgs e)
        {

            LocalFilesDatabase.FSControls.FolderPickerLib.FolderPickerDialog dlg = 
                new LocalFilesDatabase.FSControls.FolderPickerLib.FolderPickerDialog();
            dlg.InitialPath = "C:";

            if (dlg.ShowDialog() == true)
            {
                String path = dlg.SelectedPath;
                await Task.Run(() =>
                {
                    App.ViewModel.IsWorking = true;
                    OpenFolderDialog(path);
                    App.ViewModel.IsWorking = false;
                });
            }            
        }

        private void buttonCerrar_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.SelectedFolder = null;
            App.ViewModel.AllFolders = null;
            App.ViewModel.LasFolders = null;
            App.ViewModel.SearchedFolders = null;
            App.ViewModel.sourceCollection = null;
            App.ViewModel.StatusMsg = String.Empty;
            App.ViewModel.FilterMsg = String.Empty;
            App.ViewModel.showMenu = true;
        }

        private async void buttonExplore_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(App.ViewModel.SelectedFolder.FolderName)))
            {
                Process.Start(App.ViewModel.SelectedFolder.FolderName);
            }
            else
                await this.ShowMessageAsync("ABRIR DIRECTORIO", "EL DIRECTORIO NO SE ENCUENTRA DISPONIBLE.");
        }

        private void comboSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FolderComicsInfo item = (FolderComicsInfo)comboSearch.SelectedItem;
            App.ViewModel.SelectedFolder = item;
        }

        private void MenuToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.LasFolders == null || App.ViewModel.LasFolders.Count == 0)
                MenuToggleButton.IsChecked = true;
        }

        private void MenuToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ListLastGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            App.ViewModel.SelectedFolder = (FolderComicsInfo)((Grid)sender).DataContext;

        }

    

        private async void comboSearch_KeyUp(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter && !String.IsNullOrEmpty(comboSearch.Text))
            {
                App.ViewModel.IsWorking = true;
                await Task.Delay(100);
                App.ViewModel.SearchedFolders = App.ViewModel.AllFolders.Where(f => f.FolderName.Contains(comboSearch.Text)).ToList();
                App.ViewModel.sourceCollection =
                    await App.ViewModel.PopulateTreeView(App.ViewModel.SearchedFolders, '\\');
                App.ViewModel.SelectedFolder = App.ViewModel.SearchedFolders.FirstOrDefault();                
                App.ViewModel.FilterMsg = String.Format("Filtro ({0})",comboSearch.Text);
                comboSearch.Text = String.Empty;
                App.ViewModel.IsWorking = false;
            }
            else if (e.Key == Key.Enter && String.IsNullOrEmpty(comboSearch.Text))
            {
                App.ViewModel.IsWorking = true;
                await Task.Delay(100);
                App.ViewModel.SearchedFolders = App.ViewModel.AllFolders.ToList();
                App.ViewModel.sourceCollection =
                await App.ViewModel.PopulateTreeView(App.ViewModel.SearchedFolders, '\\');
                App.ViewModel.SelectedFolder = App.ViewModel.SearchedFolders.FirstOrDefault();
                App.ViewModel.FilterMsg = String.Empty;
                App.ViewModel.IsWorking = false;
            }
            
        }        
    }
}

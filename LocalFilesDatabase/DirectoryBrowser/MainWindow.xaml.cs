using DirectoryBrowser.Entities;
using DirectoryBrowser.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DirectoryBrowser
{
    /// <summary>
    /// Class Main Window
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
       
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// Event window loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MenuToggleButton.IsChecked = true;
        }

        /// <summary>
        /// Treeview selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tv_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MyTreeViewItem item = (MyTreeViewItem)e.NewValue;       
            if (item!=null)
            App.ViewModel.SelectedFolder = item.Tag;
            
        }

        /// <summary>
        /// Open file when app starts argument
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task OpenFileFromArg(String path)
        {
            await Task.Run(() =>
            {
                App.ViewModel.IsWorking = true;
                App.ViewModel.OpenFileDialog(path);
                App.ViewModel.IsWorking = false;
            });
        }
        
        /// <summary>
        /// Event Open existing file from menu left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonAbrir_Click(object sender, RoutedEventArgs e)
        {
            
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Comic Database File (.cdb)|*.cdb";
            System.Windows.Forms.DialogResult res = openFileDialog.ShowDialog();
            string path = openFileDialog.FileName;
            if (!String.IsNullOrEmpty(path) && res == System.Windows.Forms.DialogResult.OK)
            {                                
                await Task.Run(() =>
                {
                    App.ViewModel.IsWorking = true;
                    App.ViewModel.OpenFileDialog(path);
                    App.ViewModel.IsWorking = false;
                });                               
            }
        }

        /// <summary>
        /// Create new db from folder from menu left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    App.ViewModel.OpenFolderDialog(path);
                    App.ViewModel.IsWorking = false;
                });
            }            
        }

        /// <summary>
        /// Close current database buttton menu left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCerrar_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.CloseCurrentDatabase();
        }

        /// <summary>
        /// Button explore folder click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonExplore_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(App.ViewModel.SelectedFolder.FolderName)))            
                Process.Start(App.ViewModel.SelectedFolder.FolderName);            
            else            
                await App.ViewModel.DisplayPopUp("El directorio no se encuentra disponible.", "ACEPTAR");                          
        }

        /// <summary>
        /// Show/Hide menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.LasFolders == null || App.ViewModel.LasFolders.Count == 0)
                MenuToggleButton.IsChecked = true;
        }

        /// <summary>
        /// Event click last folders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListLastGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            App.ViewModel.SelectedFolder = (FolderComicsInfo)((Grid)sender).DataContext;

        }

        #region Search

        /// <summary>
        /// Search key on textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Search changue combo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FolderComicsInfo item = (FolderComicsInfo)comboSearch.SelectedItem;
            App.ViewModel.SelectedFolder = item;
        }

        #endregion

        /// <summary>
        /// Show preview files content folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewFilesButton_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.IsShowingFiles = true;
        }

        /// <summary>
        /// Show Setup dialog
        /// </summary>      
        private async void buttonConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            String id = "RootDialog";            
            Views.SetupDialogView view = new Views.SetupDialogView();
            view.MinWidth = App.Current.MainWindow.Width / 2;
            view.Margin = new System.Windows.Thickness(0, 0, 0, 0);
            //show the dialog
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(view, id);
        }

        /// <summary>
        /// Event hide list files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHideFiles_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.IsShowingFiles = false;
        }

        /// <summary>
        /// On mouse push name, display file content with viewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridFile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                String path = ((TextBlock)(e.OriginalSource)).DataContext.ToString();
                if (System.IO.File.Exists(path))
                {
                    if (App.ViewModel.ReaderUsedSelected == 0)
                        Process.Start(path);
                    else
                    {
                        App.ViewModel.InitReader(path);
                        ReaderWindow r = new ReaderWindow();
                        r.ShowDialog();

                    }
                }
            }
            catch (Exception ex) { App.ViewModel.StatusMsg = ex.Message; }

        }
    }
}

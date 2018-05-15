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
        private async void PreviewFilesButton_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteRunFilesDialog(sender);
        }

        /// <summary>
        /// Execute Run Files dialog method
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private async Task ExecuteRunFilesDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new FilesView
            {
                DataContext = App.ViewModel.SelectedFolder
            };
            
            view.MinWidth = this.Width / 2;
            view.MaxHeight = this.Height / 2;
            view.Margin = new Thickness(0,0,0,0);
            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", null, ExtendedClosingEventHandler);
        }        

        /// <summary>
        /// Cancel dialog files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return;

            //OK, lets cancel the close...
            eventArgs.Cancel();           

            //lets run a fake operation for 3 seconds then close this baby.
            Task.Delay(TimeSpan.FromSeconds(3))
                .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async void buttonConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            String id = "RootDialog";            
            Views.SetupDialogView view = new Views.SetupDialogView();
            view.MinWidth = App.Current.MainWindow.Width / 2;
            view.Margin = new System.Windows.Thickness(0, 0, 0, 0);
            //show the dialog
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(view, id);
        }
    }
}

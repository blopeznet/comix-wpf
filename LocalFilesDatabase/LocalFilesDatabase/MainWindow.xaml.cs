using LocalFilesDatabase.Entities;
using LocalFilesDatabase.Utils;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LocalFilesDatabase.FSControls.FolderPickerLib;
using System.Windows.Threading;

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
            this.Loaded += MainWindow_Loaded;
            this.Unloaded += MainWindow_Unloaded;
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            W10Utils.ModeChangeEvent -= W10Utils_ModeChangeEvent;
        }

        private void W10Utils_ModeChangeEvent(DeviceMode obj)
        {
            App.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (obj == DeviceMode.Tablet)
                    UpdateScreen(true);
                if (obj == DeviceMode.PC)
                    UpdateScreen(false);
            }));
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Isfullscreen = App.ViewModel.usefullscreen;            
            W10Utils.ModeChangeEvent += W10Utils_ModeChangeEvent;
        }

        #region full screen window

        private bool _Isfullscreen = false;
        public bool Isfullscreen
        {
            get => _Isfullscreen;
            set
            {
                bool change = _Isfullscreen != value;

                _Isfullscreen = value;
                NotifyPropertyChanged("Isfullscreen");
                if (change)
                    UpdateScreen(_Isfullscreen);
            }
        }

        bool fullscreenactive = false;

        private void UpdateFullScreen()
        {
            fullscreenactive = !fullscreenactive;
            if (fullscreenactive)
            {

                Isfullscreen = true;
                showappbar = true;
                UpdateTopBar();
            }
            else
            {
                Isfullscreen = false;
                showappbar = false;
                UpdateTopBar();
                this.GridMenu.Visibility = Visibility.Visible;
            }
        }


        public void UpdateScreen(bool fullscreen)
        {            
            if (fullscreen)
            {
                this.Hide();
                Taskbar tb = new Taskbar();
                System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens[0];
                var rect = screen.WorkingArea;
                this.Top = rect.Top - 2;
                this.Left = rect.Left - 2;
                this.Width = screen.WorkingArea.Width + 3;
                if (!tb.AutoHide)
                    this.Height = screen.WorkingArea.Height + tb.Size.Height + 2;
                else
                    this.Height = screen.WorkingArea.Height + 4;                
                this.Topmost = false;
                this.ResizeMode = ResizeMode.NoResize;
                this.IgnoreTaskbarOnMaximize = true;
                this.WindowStyle = WindowStyle.None;
                this.UseNoneWindowStyle = true;
                this.IsCloseButtonEnabled = false;
                this.Show();                
            }
            else
            {
                this.Hide();
                MainWindow r = new MainWindow();                
                r.Show();
                this.Close();
            }    
        }

        #endregion


        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.OpenFileDialog();
        }       

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.OpenFileDialogFolderForSave();            
        }        

        private async void buttonLaunchSearch_Click(object sender, RoutedEventArgs e)
        {
            await App.ViewModel.SearchCollectionIntoDatabase();
            UpdateItemDetailView(false);
        }

        private void ButtonSincro_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.UpdateExistingFolder();
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
                DBService.Instance.SaveLastFolder(App.ViewModel.SelectedFolder);
                DBService.Instance.SaveLastComic(App.ViewModel.SelectedFile);
                await App.ViewModel.ShowReader(App.ViewModel.SelectedFile.Path,this,App.ViewModel.SelectedFile.CurrentPages);
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

        /// <summary>
        /// Evento elementos de lista ficheros recientes abiertos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Button for close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Appbar

        /// <summary>
        /// Flag for show/hide appbar
        /// </summary>
        private bool showappbar = false;

        /// <summary>
        ///Show hide appbar
        /// </summary>
        private void GridMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateTopBar();
        }

        /// <summary>
        /// Seleccionar comic flipview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Comic_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left && e.ClickCount == 2)
            {

                Entities.ItemInfo info = (Entities.ItemInfo)(((System.Windows.Controls.StackPanel)sender).DataContext);
                if (System.IO.File.Exists(info.Path))
                    await App.ViewModel.ShowReader(info.Path, (MahApps.Metro.Controls.MetroWindow)App.Current.MainWindow,info.CurrentPages);
            }
        }
        
        /// <summary>
        /// Method show or hide appbar
        /// </summary>
        private void UpdateTopBar()
        {
            showappbar = !showappbar;
            if (showappbar)
            {
                Storyboard myStoryboard = (Storyboard)this.Resources["sbShowTopBar"];
                Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as ThicknessAnimationUsingKeyFrames, GridMenu);
                myStoryboard.Begin();
            }
            else
            {
                Storyboard myStoryboard = (Storyboard)this.Resources["sbHideTopBar"];
                Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as ThicknessAnimationUsingKeyFrames, GridMenu);
                myStoryboard.Begin();
            }
        }

        public void UpdateItemDetailView(bool foward=true)
        {
            if (foward)
            {
                this.GridFolders.Visibility = Visibility.Collapsed;
                if (App.ViewModel.SelectedFile!=null)
                this.ListViewFiles.ScrollIntoView(App.ViewModel.SelectedFile);
                this.GridFiles.Visibility = Visibility.Visible;
            }
            else
            {
                this.GridFiles.Visibility = Visibility.Collapsed;
                if (App.ViewModel.SelectedFolder != null)
                    this.ListViewFiles.ScrollIntoView(App.ViewModel.SelectedFolder);
                this.GridFolders.Visibility = Visibility.Visible;                
            }
        }

        #endregion

        #region InotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void buttonUnRead_Click(object sender, RoutedEventArgs e)
        {
            MarkUnRead = !_MarkUnRead;
            if (MarkUnRead)
                MarkRead = false;
            UpdateMarkMode();
        }

        private void buttonRead_Click(object sender, RoutedEventArgs e)
        {
            MarkRead = !MarkRead;
            if (MarkRead)
                MarkUnRead = false;
            UpdateMarkMode();
        }

        private void UpdateMarkMode()
        {
            if (MarkRead || MarkUnRead)
            {
                ListViewFiles.SelectedIndex = -1;
                ListViewFiles.SelectionMode = SelectionMode.Multiple;
                this.ListViewFiles.SelectionChanged += ListViewFiles_SelectionChanged;

            }
            else
            {
                this.ListViewFiles.SelectionChanged -= ListViewFiles_SelectionChanged;
                ListViewFiles.SelectedIndex = 0;
                ListViewFiles.SelectionMode = SelectionMode.Single;
            }
        }

        private void ListViewFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ItemInfo info = (ItemInfo)e.AddedItems[0];
                if (MarkRead)
                {
                    App.ViewModel.Files.Where(p => p.Id == info.Id).FirstOrDefault().CurrentPages = info.TotalPages;
                    DBService.Instance.UpdateItemFile(info.Path, info.TotalPages);
                }
                else if (MarkUnRead)
                {
                    App.ViewModel.Files.Where(p => p.Id == info.Id).FirstOrDefault().CurrentPages = 0;
                    DBService.Instance.UpdateItemFile(info.Path, 0);
                }
            }
        }

        private bool _MarkRead = false;
        public bool MarkRead
        {
            get => _MarkRead;
            set
            {
                _MarkRead = value;
                buttonRead.IsChecked = _MarkRead;
                NotifyPropertyChanged("MarkRead");                
            }
        }

        private bool _MarkUnRead = false;
        public bool MarkUnRead
        {
            get => _MarkUnRead;
            set
            {
                _MarkUnRead = value;
                buttonUnRead.IsChecked = _MarkUnRead;                
                NotifyPropertyChanged("MarkUnRead");
            }
        }

        private void buttonBackToCollectionList_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemDetailView(false);
        }
    }
}

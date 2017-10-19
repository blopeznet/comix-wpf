using LocalFilesDatabase.Entities;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LocalFilesDatabase
{

    /// <summary>
    /// Lógica de interacción para ReaderWindow.xaml
    /// </summary>
    public partial class ReaderWindow : MetroWindow, INotifyPropertyChanged
    {


        #region Internal Variables

        /// <summary>
        /// Window reference
        /// </summary>
        private MetroWindow _mainWindowReference;
        /// <summary>
        /// Pages to show
        /// </summary>
        public List<ComicTemp> _pages { get; set; }

        /// <summary>
        /// Scrollviewer
        /// </summary>
        private ScrollViewer _currentScroll;
        
        /// <summary>
        /// Flag adjust by width or by height
        /// </summary>
        private bool _IsFit;
        public bool IsFit
        {
            get => _IsFit;
            set
            {
                _IsFit = value;
                NotifyPropertyChanged("IsFit");
                UpdateAdjust(_IsFit);
            }
        }

        /// <summary>
        /// Height adjust image
        /// </summary>
        private Double _HeightDisplay;
        public double HeightDisplay
        {
            get => _HeightDisplay;
            set
            {
                _HeightDisplay = value;
                NotifyPropertyChanged("HeightDisplay");
            }
        }

        /// <summary>
        /// Width adjust image
        /// </summary>
        private Double _WidthDisplay;
        public double WidthDisplay
        {
            get => _WidthDisplay;
            set
            {
                _WidthDisplay = value;
                NotifyPropertyChanged("WidthDisplay");
            }
        }


        #endregion

        public ReaderWindow()
        {
            InitializeComponent();
            this.Loaded += ReaderWindow_Loaded;
            this.Unloaded += ReaderWindow_Unloaded;
        }

        private void ReaderWindow_Unloaded(object sender, RoutedEventArgs e)
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

        bool fullscreenactive = false;

        private void UpdateFullScreen()
        {
            fullscreenactive = !fullscreenactive;
            if (fullscreenactive)
            {

                Isfullscreen = true;
                UpdateAdjust(false);
                showappbar = true;
                UpdateTopBar();
            }
            else
            {
                Isfullscreen = false;
                UpdateAdjust(true);
                showappbar = false;
                UpdateTopBar();
                this.GridMenu.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Control with keyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left
                || e.Key == Key.Right || e.Key == Key.PageDown || e.Key == Key.PageUp
                || e.Key == Key.F11)
            {

                ScrollViewer sv = FindChild<ScrollViewer>(FvPages);

                if (sv != null)
                {
                    if (e.Key == Key.F11 && App.ViewModel.usefullscreen==false)
                    {
                        UpdateFullScreen();
                    }


                    if (e.Key == Key.Down)
                    {
                        sv.ScrollToVerticalOffset(sv.VerticalOffset + 10);
                        return;
                    }

                    if (e.Key == Key.Up)
                    {
                        sv.ScrollToVerticalOffset(sv.VerticalOffset - 10);
                        return;
                    }

                    if (e.Key == Key.Left)
                    {
                        sv.ScrollToVerticalOffset(sv.VerticalOffset - 30);
                        return;
                    }

                    if (e.Key == Key.Down)
                    {
                        sv.ScrollToVerticalOffset(sv.VerticalOffset + 30);
                        return;
                    }

                    if (e.Key == Key.PageUp && FvPages.SelectedIndex >= 1)
                    {
                        FvPages.Visibility = Visibility.Collapsed;
                        PART_BackButton_Click(null, null);
                        FvPages.Visibility = Visibility.Visible;
                        FvPages.SelectedIndex -= 1;
                        return;
                    }

                    if (e.Key == Key.PageDown && FvPages.SelectedIndex <= FvPages.Items.Count - 2)
                    {
                        FvPages.Visibility = Visibility.Collapsed;
                        PART_ForwardButton_Click(null, null);
                        FvPages.Visibility = Visibility.Visible;
                        FvPages.SelectedIndex += 1;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Event Load initial appearance window reader
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReaderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            W10Utils.ModeChangeEvent += W10Utils_ModeChangeEvent;
            showappbar = true;
            if (IsFit)
            {
                HeightDisplay = CurrentReaderWindow.ActualHeight - 32;
            }
            else
            {
                WidthDisplay = this.CurrentReaderWindow.ActualWidth;                
            }
                        
            UpdateTopBar();

            W10Utils.ShowNotification(String.Format("Abierto {0}",App.ViewModel.SelectedFile.DisplayName));
        }

        #region Methods

        /// <summary>
        /// Method update adjust by width or height
        /// </summary>
        /// <param name="isfit"></param>
        private void UpdateAdjust(bool isfit)
        {
            if (isfit)
            {
                HeightDisplay = CurrentReaderWindow.Height - 32;
            }
            else
            {
                WidthDisplay = this.ActualWidth;
            }
        }

        /// <summary>
        /// LOad initial pages
        /// </summary>
        /// <param name="pages">Pages to load</param>
        /// <param name="mainwreference">Window reference</param>
        public void LoadPages(List<ComicTemp> pages, MetroWindow mainwreference,int firstpage)
        {
            _mainWindowReference = mainwreference;
            _pages = pages;                                   
            IsFit = false;
            FvPages.ItemsSource = _pages;
            FvPages.SelectedIndex = firstpage-1;
            Isfullscreen = App.ViewModel.usefullscreen; 
        }

        #endregion

        #region full screen window

        /// <summary>
        /// Bool know is fullscreen
        /// </summary>
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
        
        /// <summary>
        /// Method update screen to full
        /// </summary>
        /// <param name="fullscreen"></param>
        private void UpdateScreen(bool fullscreen)
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
                this.Topmost = true;
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
                ReaderWindow r = new ReaderWindow();
                r.LoadPages(_pages, _mainWindowReference,FvPages.SelectedIndex);
                r.Show();
                this.Close();
            }
        }

        #endregion

        #region Appbar

        /// <summary>
        /// Flag for show/hide appbar
        /// </summary>
        private bool _showappbar = false;
        public bool showappbar {
            get => _showappbar;
            set
            {
                _showappbar = value;
                NotifyPropertyChanged("showappbar");
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

        /// <summary>
        ///Show hide appbar
        /// </summary>        
        private void buttonHideMenu_Click(object sender, RoutedEventArgs e)
        {
            UpdateTopBar();
        }

        /// <summary>
        ///Show hide appbar
        /// </summary>
        private void GridMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateTopBar();
        }

        /// <summary>
        ///Show hide appbar
        /// </summary>
        private void ImageGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                IsFit = !IsFit;
                UpdateAdjust(IsFit);
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

        /// <summary>
        /// Button for close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Button for adjust image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdjust_Click(object sender, RoutedEventArgs e)
        {
            IsFit = !IsFit;
        }


        #region Load previous/next file

        private async void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            UpdateTopBar();
            await LoadNext();
        }

        private async void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            UpdateTopBar();
            await LoadPrev();
        }

        private async Task<bool> LoadNext()
        {
            try
            {
                int current = Array.IndexOf(App.ViewModel.Files.ToArray(), App.ViewModel.SelectedFile);
                if (current <= App.ViewModel.Files.Count - 1)
                {
                    ItemInfo next = App.ViewModel.Files[current + 1];
                    if (next != null)
                        return await UpdateReader(next);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar siguiente {0}", ex.Message);
            }

            return false;
        }

        private async Task<bool> LoadPrev()
        {
            try
            {                
                int current = Array.IndexOf(App.ViewModel.Files.ToArray(), App.ViewModel.SelectedFile);
                if (current >= 1)
                {
                    ItemInfo next = App.ViewModel.Files[current - 1];
                    if (next != null)
                        await UpdateReader(next);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar anterior {0}", ex.Message);
            }

            return false;
        }

        public async Task<bool> UpdateReader(ItemInfo replace)
        {

            App.ViewModel.IsWorking = true;
            App.ViewModel.SelectedFile = replace;
            FvPages.Visibility = Visibility.Collapsed;
            _pages.Clear();
            FvPages.ItemsSource = _pages;
            App.ViewModel.WorkingMsg = String.Format("CARGANDO PAGINAS...");
            await Task.Delay(1);
            List<ComicTemp> pages = new List<ComicTemp>();
            App.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                _pages = MainUtils.CreatePagesComic(replace.Path, replace.CurrentPages);
                DBService.Instance.SaveLastFolder(App.ViewModel.SelectedFolder);
                DBService.Instance.SaveLastComic(replace);
                int moveto = replace.CurrentPages - 1;
                if (moveto == -1)
                    moveto = 0;                
                FvPages.ItemsSource = _pages;
                FvPages.SelectedIndex = moveto;                
                FvPages.Visibility = Visibility.Visible;
            }));
            App.ViewModel.IsWorking = false;
            App.ViewModel.WorkingMsg = String.Empty;
            await Task.Delay(1);
            return true;
        }

        private async void PART_ForwardButton_LAST_Click(object sender, RoutedEventArgs e)
        {
            await LoadNext();
        }

        private async void PART_BackButton_FIRST_Click(object sender, RoutedEventArgs e)
        {
            await LoadPrev();
        }

        #endregion

        private void FvPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           DBService.Instance.UpdateItemFile(App.ViewModel.SelectedFile.Path,FvPages.SelectedIndex+1);
           ItemInfo info = App.ViewModel.Files.Where(f => f.Path == App.ViewModel.SelectedFile.Path).FirstOrDefault();
           if (info != null)
                info.CurrentPages = FvPages.SelectedIndex + 1;             
        }

        #region Reset scroll when load page

        private void ResetScroll(bool top)
        {
            _currentScroll = FindChild<ScrollViewer>(FvPages);
            if (_currentScroll != null)
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (_currentScroll != null && _currentScroll.VerticalOffset != 0)
                    {
                        if (top)
                            _currentScroll.ScrollToVerticalOffset(0);
                        else
                            _currentScroll.ScrollToVerticalOffset(Double.PositiveInfinity);
                    }
                }));
            }
        }

        private void PART_BackButton_Click(object sender, RoutedEventArgs e)
        {
            ResetScroll(false);

        }

        private void PART_ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            ResetScroll(true);
        }

        /// <summary>
        /// Looks for a child control within a parent by name
        /// </summary>
        public DependencyObject FindChild(DependencyObject parent, string name)
        {
            // confirm parent and name are valid.
            if (parent == null || string.IsNullOrEmpty(name)) return null;

            if (parent is FrameworkElement && (parent as FrameworkElement).Name == name) return parent;

            DependencyObject result = null;

            if (parent is FrameworkElement) (parent as FrameworkElement).ApplyTemplate();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                result = FindChild(child, name);
                if (result != null) break;
            }

            return result;
        }

        /// <summary>
        /// Looks for a child control within a parent by type
        /// </summary>
        public T FindChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            // confirm parent is valid.
            if (parent == null) return null;
            if (parent is T) return parent as T;

            DependencyObject foundChild = null;

            if (parent is FrameworkElement) (parent as FrameworkElement).ApplyTemplate();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                foundChild = FindChild<T>(child);
                if (foundChild != null) break;
            }

            return foundChild as T;
        }

        #endregion

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {            
            if (((ComicTemp)FvPages.SelectedItem).Image != null)
            {
                String path = MainUtils.SaveFileAndGetPath(((ComicTemp)FvPages.SelectedItem).Image);
                W10Utils.ShowNotification(String.Format("Imagen guardada en Imágenes.", path),path);                
            }
        }

        private void buttonFull_Click(object sender, RoutedEventArgs e)
        {
            UpdateFullScreen();
        }

        private void ButtonThumbnail_Click(object sender, RoutedEventArgs e)
        {
            UpdateTopBar();
        }

        private void BottomMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
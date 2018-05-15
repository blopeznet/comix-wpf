using DirectoryBrowser.Entities;
using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DirectoryBrowser.Views
{
    /// <summary>
    /// Lógica de interacción para ReaderWindow.xaml
    /// </summary>
    public partial class ReaderWindow : MetroWindow, INotifyPropertyChanged
    {

        #region Variables

        /// <summary>
        /// Flag adjust by width or by height
        /// </summary>
        private bool _IsFit = true;
        public bool IsFit
        {
            get => _IsFit;
            set
            {
                _IsFit = value;
                NotifyPropertyChanged("IsFit");
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

        /// <summary>
        /// Page No. for display
        /// </summary>
        private Double _PageNo;
        public double PageNo
        {
            get => _PageNo;
            set
            {
                _PageNo = value;
                NotifyPropertyChanged("PageNo");
            }
        }

        /// <summary>
        /// Total pages for display
        /// </summary>
        private Double _TotalPages;
        public double TotalPages
        {
            get => _TotalPages;
            set
            {
                _TotalPages = value;
                NotifyPropertyChanged("TotalPages");
            }
        }


        #endregion

        public ReaderWindow()
        {
            InitializeComponent();
            this.Loaded += ReaderWindow_Loaded;
        }

        /// <summary>
        /// Event Load initial appearance window reader
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReaderWindow_Loaded(object sender, RoutedEventArgs e)
        {            
            PageNo = 1;
            TotalPages = App.ViewModel.Pages.Count;

            if (IsFit)            
                HeightDisplay = CurrentReaderWindow.ActualHeight - 32;            
            else            
                WidthDisplay = this.CurrentReaderWindow.ActualWidth;            

            if (FvPages.SelectedItem!=null)
                PageNo = App.ViewModel.Pages.IndexOf((ComicTemp)FvPages.SelectedItem) + 1;
        }

        /// <summary>
        /// Event when change element page display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FvPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PageNo = App.ViewModel.Pages.IndexOf((ComicTemp)FvPages.SelectedItem) + 1;
        }

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
        ///Adjust image to screen horizontall or vertical
        /// </summary>
        private void ImageGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                IsFit = !IsFit;
                UpdateAdjust(IsFit);
            }
        }

        /// <summary>
        /// Event when click last page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PART_ForwardButton_LAST_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Event when click first page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PART_BackButton_FIRST_Click(object sender, RoutedEventArgs e)
        {
        }

        #region Reset scroll when load page

        /// <summary>
        /// Scrollviewer
        /// </summary>
        private ScrollViewer _currentScroll;

        /// <summary>
        /// Method reset scroll element
        /// </summary>
        /// <param name="top"></param>
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

        /// <summary>
        /// Reset scroll when back page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_BackButton_Click(object sender, RoutedEventArgs e)
        {
            ResetScroll(false);
        }

        /// <summary>
        /// Reset scroll when go next page
        /// </summary>
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
        /// Flag fullscreen
        /// </summary>
        bool fullscreenactive = false;

        /// <summary>
        /// Method Update Fullscreen
        /// </summary>
        private void UpdateFullScreen()
        {
            fullscreenactive = !fullscreenactive;
            if (fullscreenactive)
            {

                Isfullscreen = true;
                UpdateAdjust(false);
            }
            else
            {
                Isfullscreen = false;
                UpdateAdjust(true);
            }
        }

        /// <summary>
        /// Method update screen to full generatin new window or update actual window
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
                    this.Height = screen.WorkingArea.Height + tb.Size.Height + 3;
                else
                    this.Height = screen.WorkingArea.Height + 4;
                this.Topmost = false;
                this.ResizeMode = ResizeMode.NoResize;
                this.IgnoreTaskbarOnMaximize = true;
                this.WindowStyle = WindowStyle.None;
                this.UseNoneWindowStyle = true;
                this.IsCloseButtonEnabled = false;
                this.popUpBox.IsPopupOpen = true;
                this.FvPages.Focus();
                this.Show();
            }
            else
            {
                this.Hide();
                ReaderWindow r = new ReaderWindow();
                r.Show();
                this.Close();
            }
        }

        /// <summary>
        /// Control with keyboard to fullscreen (F11)
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
                    if (e.Key == Key.F11)
                    {
                        UpdateFullScreen();
                        popUpBox.IsPopupOpen = false;
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

        #region Action Buttons

        /// <summary>
        /// Update fullscreen button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFullScreen_Click(object sender, RoutedEventArgs e)
        {            
            UpdateFullScreen();            
        }

        /// <summary>
        /// Save image button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonSaveImage_Click(object sender, RoutedEventArgs e)
        {
            String path = UtilsApp.SaveFileAndGetPath(((ComicTemp)FvPages.SelectedItem).Image);
            await App.ViewModel.DisplayPopUp(String.Format("Fichero creado en {0}", path), "ACEPTAR", "RootDialogReader");                        
        }

        /// <summary>
        /// About button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            await App.ViewModel.DisplayPopUp(String.Format("Created by blopez 2018"), "ACEPTAR", "RootDialogReader");
        }

        #endregion

    }
}

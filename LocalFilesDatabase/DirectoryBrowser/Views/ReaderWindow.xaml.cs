using DirectoryBrowser.Entities;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DirectoryBrowser.Views
{
    /// <summary>
    /// Lógica de interacción para ReaderWindow.xaml
    /// </summary>
    public partial class ReaderWindow : MetroWindow, INotifyPropertyChanged
    {
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
            //UpdateScreen(true);
            PageNo = 1;
            TotalPages = App.ViewModel.Pages.Count;

            if (IsFit)
            {
                HeightDisplay = CurrentReaderWindow.ActualHeight - 32;
            }
            else
            {
                WidthDisplay = this.CurrentReaderWindow.ActualWidth;
            }

        }

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

        private async void PART_ForwardButton_LAST_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void PART_BackButton_FIRST_Click(object sender, RoutedEventArgs e)
        {
        }

        #region Reset scroll when load page

        /// <summary>
        /// Scrollviewer
        /// </summary>
        private ScrollViewer _currentScroll;

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

        #endregion

        private void FvPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PageNo = App.ViewModel.Pages.IndexOf((ComicTemp)FvPages.SelectedItem)+1;

        }
    }
}

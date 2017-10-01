﻿using LocalFilesDatabase.Entities;
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
    public partial class ReaderWindow : MetroWindow,INotifyPropertyChanged
    {
        public ReaderWindow()
        {
            InitializeComponent();
            this.Loaded += ReaderWindow_Loaded;
        }

        private void ReaderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            showappbar = App.usefullscreen;
            if (IsFit)
                HeightDisplay = this.RowContent.ActualHeight + 80;
            else
                WidthDisplay = this.Width;
            UpdateTopBar();
        }

        private MetroWindow _mainWindowReference;
        public List<ComicTemp> _pages;

        private ScrollBarVisibility _ShowScrollBar= ScrollBarVisibility.Hidden;
        public ScrollBarVisibility ShowScrollBar
        {
            get
            {
                return _ShowScrollBar;
            }

            set
            {
                _ShowScrollBar = value;
                NotifyPropertyChanged("ShowScrollBar");
            }
        }

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
        private void UpdateAdjust(bool isfit)
        {
            if (isfit)
            {
                HeightDisplay = this.RowContent.ActualHeight + 80;
            }
            else
            {
                WidthDisplay = this.ActualWidth;
            }
        }
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

        private Double _VerticalOffsetDisplay;
        public double VerticalOffsetDisplay
        {
            get => _VerticalOffsetDisplay;
            set
            {
                _VerticalOffsetDisplay = value;
                NotifyPropertyChanged("VerticalOffsetDisplay");
            }
        }

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

        public void LoadPages(List<ComicTemp> pages,MetroWindow mainwreference)
        {            
            _mainWindowReference = mainwreference;                  
            _pages = pages;
            Isfullscreen = App.usefullscreen;
            if (!Isfullscreen)            
                this.GridMenu.Background = new SolidColorBrush(Color.FromRgb(187,20,4));            
            IsFit = false;            
            FvPages.ItemsSource = pages;
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
                r.LoadPages(_pages, _mainWindowReference);
                r.Show();
                this.Close();
            }
        }

        #endregion

#region Appbar
        private bool showappbar = false;
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

        private void buttonCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonAdjust_Click(object sender, RoutedEventArgs e)
        {
            IsFit = !IsFit;            
        }

        private ScrollViewer _currentScroll;

        private void buttonHideMenu_Click(object sender, RoutedEventArgs e)
        {
            UpdateTopBar();
        }

        private void GridMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateTopBar();
        }

        private void ImageGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                UpdateTopBar();
        }

        private async void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            await LoadNext();
        }

        private async void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            await LoadPrev();
        }

        private async Task<bool> LoadNext()
        {
            try
            {
                int current = Array.IndexOf(App.ViewModel.Files.ToArray(), App.ViewModel.SelectedFile);
                if (current <= App.ViewModel.Files.Count - 2)
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
                if (current >= 2)
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
            _pages.Clear();
            FvPages.ItemsSource = _pages;
            App.ViewModel.WorkingMsg = String.Format("CARGANDO PAGINAS...");
            await Task.Delay(1);
            List<ComicTemp> pages = new List<ComicTemp>();
            App.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                _pages = MainUtils.CreatePagesComic(replace.Path);
                FvPages.ItemsSource = _pages;
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

        /// <summary>
        /// Looks for a child control within a parent by name
        /// </summary>
        public static DependencyObject FindChild(DependencyObject parent, string name)
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

        private void FvPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void PART_BackButton_Click(object sender, RoutedEventArgs e)
        {
            ResetScroll(false);

        }

        private void PART_ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            ResetScroll(true);
        }
    }
}

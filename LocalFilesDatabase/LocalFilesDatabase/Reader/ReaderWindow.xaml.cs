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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            UpdateTopBar();
        }

        private MetroWindow _mainWindowReference;
        private List<ComicTemp> _pages;

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

        public void LoadPages(List<ComicTemp> pages,MetroWindow mainwreference)
        {            
            _mainWindowReference = mainwreference;
            _pages = pages;
            Isfullscreen = App.usefullscreen;
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

        private void scrollView_Loaded(object sender, RoutedEventArgs e)
        {
            _currentScroll = ((ScrollViewer)sender);
           
        }

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
    }
}
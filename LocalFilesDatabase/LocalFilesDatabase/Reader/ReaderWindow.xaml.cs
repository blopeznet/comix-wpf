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
        }

        private MetroWindow _mainWindowReference;

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

        private bool _Isfullscreen;
        public bool Isfullscreen {
            get => _Isfullscreen;
            set
            {
                _Isfullscreen = value;
                NotifyPropertyChanged("Isfullscreen");
                UpdateScreen(_Isfullscreen);
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

        public void LoadPages(List<BitmapImage> pages,MetroWindow mainwreference)
        {            
            _mainWindowReference = mainwreference;
            Isfullscreen = false;
            IsFit = false;
            FvPages.ItemsSource = pages;
        }

        private void buttonFullScreen_Click(object sender, RoutedEventArgs e)
        {            
            Isfullscreen = !Isfullscreen;
        }

        private void UpdateScreen(bool fullscreen)
        {
            if (fullscreen)
            {
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
                this.GridMenu.Height = 25;
            }
            else
            {
                                
                this.Top = _mainWindowReference.Top;
                this.Left = _mainWindowReference.Left;
                this.Width = _mainWindowReference.Width;
                this.Height = _mainWindowReference.Height;
                this.Topmost = false;
                this.ResizeMode = ResizeMode.CanResize;
                this.IgnoreTaskbarOnMaximize = false;
                this.GridMenu.Height = 80;
            }
        }

        private void UpdateAdjust(bool isfit)
        {
            if (isfit)
            {
                HeightDisplay = this.RowContent.ActualHeight;
            }
            else
            {                      
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
    }
}

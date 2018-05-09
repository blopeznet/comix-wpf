using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace DirectoryBrowser.Views
{
    /// <summary>
    /// Lógica de interacción para FilesView.xaml
    /// </summary>
    public partial class FilesView : UserControl
    {
        public FilesView()
        {
            InitializeComponent();
        }

        bool useexternalapp = false;

        private void GridFile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                String path = ((TextBlock)(e.OriginalSource)).DataContext.ToString();
                if (System.IO.File.Exists(path))
                {                    
                    if (useexternalapp)
                        Process.Start(path);
                    else
                    {
                        App.ViewModel.InitReader(path);
                        ReaderWindow r = new ReaderWindow();
                        r.ShowDialog();
                        
                    }
                }                    
            }catch(Exception ex) { App.ViewModel.StatusMsg = ex.Message; }
            
        }
    }
}

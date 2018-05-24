using DirectoryBrowser.WebServer.Utils;
using DirectoryBrowser.WebServer.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectoryBrowser.WebServerServer
{
    static class Program
    {
        /// <summary>
        /// View Model
        /// </summary>
        private static MainViewModel _ViewModel;
        public static MainViewModel ViewModel { get { if (_ViewModel == null) _ViewModel = new MainViewModel(); return _ViewModel; } }

        private static SimpleLogger logger = new SimpleLogger(); // Will create a fresh new log file if it doesn't exist.

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {                        
            //Instantiate the logger
            string[] args = Environment.GetCommandLineArgs();
            
            //Para no abrir la app dos veces
            bool alreadyopened = IsAppAlreadyOpenedKill();
            if (alreadyopened)
            {
                logger.Error("App is already open...");
                //app is already running! Exiting the application   
                Application.Exit();
                return;
            }
            
            if (args == null || args.Count() != 2)
            {
                logger.Error("No parameters.");
                return;
            }


            if (System.IO.File.Exists(args[0]) && (System.IO.Path.GetExtension(args[0]).EndsWith("cdb"))) { 
                logger.Error(String.Format("Error file not found or extension error (Filename: {0} ).",args[0]));
                return;
            }

            Console.WriteLine(args[1]);
            ExecuteServer(args[1]);
            logger.Trace(String.Format("--> Starting server at {0}...", GetLocalAddress()));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(CreateSetupForm());                        
        }

        /// <summary>
        /// Launch and execute server
        /// </summary>
        /// <param name="path"></param>
        private static void ExecuteServer(String path)
        {            
          ViewModel.LoadDataBase(path);
          ViewModel.InitServer();  
            
        }

        #region Form setup


        public static NotifyIcon notifyIcon1 = new NotifyIcon();
        public static ContextMenu contextMenu1 = new ContextMenu();

        public static Form CreateSetupForm()
        {
            // Add menu items to shortcut menu.  
            contextMenu1.MenuItems.Add(String.Format("V&iew {0}", GetLocalAddress()));
            contextMenu1.MenuItems.Add("E&xit");
            contextMenu1.MenuItems[0].Click += View_Click;
            contextMenu1.MenuItems[1].Click += Exit_Click;
            notifyIcon1.Icon = DirectoryBrowser.WebServer.Properties.Resources.Mattahan_Ultrabuuf_Comics_Ironman_Folder;
            notifyIcon1.Visible = true;
            notifyIcon1.Text = "Comic Web Server";
            notifyIcon1.Visible = true;
            notifyIcon1.ContextMenu = contextMenu1;

            Form form = new Form();
            form.ContextMenu = contextMenu1;
            form.Icon = notifyIcon1.Icon;            
            form.ShowInTaskbar = false;            
            form.WindowState = FormWindowState.Minimized;
            return form;
        }

        private static void Exit_Click(object sender, EventArgs e)
        {            
            logger.Trace("--> Exit server...");
            Application.Exit();
        }

        private static void View_Click(object sender, EventArgs e)
        {
            logger.Trace("--> View server...");
            Process.Start("explorer.exe", GetLocalAddress());
        }

        public static string GetLocalAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {                        
                        return String.Format("http://{0}:9664",ip.ToString());
                    }
                }
            }
            catch
            {
                return "http://localhost:9664";
            }
            return "http://localhost:9664";
        }

        #endregion

        #region Do not open if already open, bring to front

        private static Mutex _mutex = null;

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
        private const int SW_RESTORE = 9;
        private const int SW_NORMAL = 5;
        private const int SW_SHOWMAXIMIZED = 3;


        private static bool IsAppAlreadyOpenedKill()
        {
            string appName = Process.GetCurrentProcess().ProcessName.Replace(".exe", "");
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {

                List<Process> ps = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).ToList();
                Process proc = ps.Where(p => p.ProcessName.ToString() == appName).FirstOrDefault();
                proc.Kill();
                return false;
            }

            return false;
        }

        #endregion
    }
}

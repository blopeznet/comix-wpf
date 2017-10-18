using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Notifications;

namespace LocalFilesDatabase
{
    public enum DeviceMode
    {
        None,
        PC,
        Tablet
    }

    public static class W10Utils
    {

        public static void CheckDeviceMode()
        {
            try
            {
                var tabletMode = (int)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ImmersiveShell", "TabletMode", 0);
                if (tabletMode == 1)
                {
                    if (!App.ViewModel.usefullscreen)
                    {
                        App.ViewModel.usefullscreen = true;
                        RaiseModeChangeEvent(DeviceMode.Tablet);
                    }

                }
                else
                {
                    if (App.ViewModel.usefullscreen)
                    {
                        App.ViewModel.usefullscreen = false;
                        RaiseModeChangeEvent(DeviceMode.PC);
                    }
                }
            }
            catch { }

        }

        public static event Action<DeviceMode> ModeChangeEvent;

        private static void RaiseModeChangeEvent(DeviceMode newmode)
        {
            // Your logic
            if (ModeChangeEvent != null)
                ModeChangeEvent(newmode);
        }

        /// <summary>
        /// Devuelve la URI del archivo pasado por parámetro formada con el nombre del proyecto.
        /// 
        /// Ejemplo de entrada: "/Assets/Images/logo.png"        
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static String GetAssetsUriForFile(String file)
        {
            string appFolderPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string resourcesFolderPath = System.IO.Path.Combine(System.IO.Directory.GetParent(appFolderPath).Parent.FullName, "Assets\\Images");
            string path = "file:///" + resourcesFolderPath + "\\" + file;
            return path;
        }

        private static ToastNotification toast;

        public static void ShowNotification(String content="",String path="")
        {

            try
            {

                String imageUri = W10Utils.GetAssetsUriForFile("toast_logo.png");

                // Get a toast XML template
                var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

                // Fill in the text element
                var stringElements = toastXml.GetElementsByTagName("text");
                stringElements[1].AppendChild(toastXml.CreateTextNode(content));

                // Set image
                var toastImageAttribute = toastXml.GetElementsByTagName("image").Select(s => (Windows.Data.Xml.Dom.XmlElement)s).First();
                toastImageAttribute.SetAttribute("src", imageUri);
                toastImageAttribute.SetAttribute("alt", "logo");
                // Create the toast and attach event listeners
                toast = new ToastNotification(toastXml);
                toast.Tag = path;
                toast.Activated += Toast_Activated;
                toast.Dismissed += Toast_Dismissed;
                toast.Failed += Toast_Failed;

                // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
                ToastNotificationManager.CreateToastNotifier("COMIX").Show(toast);
            }
            catch { }
        }

        private static void Toast_Failed(ToastNotification sender, ToastFailedEventArgs args)
        {
            
        }

        private static void Toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {

           
        }

        private static void Toast_Activated(ToastNotification sender, object args)
        {
            if (!String.IsNullOrEmpty(toast.Tag))
                if (System.IO.File.Exists(toast.Tag))
                    OpenImage(toast.Tag);
        }

        public static async Task<StorageFile> GetAsStorageFile(String path)
        {
            return await StorageFile.GetFileFromPathAsync(path);
        }

        /// <summary>
        /// NOT WORKING
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <returns></returns>
        public static async Task ChangeStartScreenBackground(String ImagePath)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {                
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("file://"+ImagePath));
                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                var isSuccess = await settings.TrySetWallpaperImageAsync(file);
                if (isSuccess)                
                    W10Utils.ShowNotification("Set as background and lock screen successfully.");                
                else                
                    W10Utils.ShowNotification("Fail to set image. #API ERROR.");                
            }
        }

        public static void OpenImage(String path)
        {            
            Process p = new Process();
            p.StartInfo.FileName = path;            
            p.Start();            
        }
    }
}

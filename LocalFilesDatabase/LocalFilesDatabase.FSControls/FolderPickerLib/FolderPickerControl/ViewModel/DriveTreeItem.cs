namespace LocalFilesDatabase.FSControls.FolderPickerLib
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Globalization;
  using System.IO;
  using System.Linq.Expressions;
  using System.Windows.Data;
  using System.Windows.Media.Imaging;

  /// <summary>
  /// Class to display different types of drives (CD-ROM, Hard Disk etc)
  /// </summary>
  public class DriveTreeItem : TreeItem
  {
    public DriveTreeItem(string name, DriveType driveType, TreeItem parent)
      : base(name, parent)
    {
      DriveType = driveType;
    }

    public DriveType DriveType { get; set; }
  }

  /// <summary>
  /// Get an icon for each type of drive there is
  /// </summary>
  public class DriveIconConverter : IValueConverter
  {
    private static BitmapImage removable;
    private static BitmapImage drive;
    private static BitmapImage netDrive;
    private static BitmapImage cdrom;
    private static BitmapImage ram;
    private static BitmapImage folder;

    public DriveIconConverter()
    {
      if (removable == null)
        removable = this.CreateImage("pack://application:,,,/LocalFilesDatabase.FSControls;component/FolderPickerLib/FolderPickerControl/Images/shell32_8.ico");
      ////removable = CreateImage("pack://application:,,,/FolderPickerLib;component/Images/shell32_8.ico");

      if (drive == null)
        drive = this.CreateImage("pack://application:,,,/LocalFilesDatabase.FSControls;component/FolderPickerLib/FolderPickerControl/Images/shell32_9.ico");

      if (netDrive == null)
        netDrive = this.CreateImage("pack://application:,,,/LocalFilesDatabase.FSControls;component/FolderPickerLib/FolderPickerControl/Images/shell32_10.ico");

      if (cdrom == null)
        cdrom = this.CreateImage("pack://application:,,,/LocalFilesDatabase.FSControls;component/FolderPickerLib/FolderPickerControl/Images/shell32_12.ico");

      if (ram == null)
        ram = this.CreateImage("pack://application:,,,/LocalFilesDatabase.FSControls;component/FolderPickerLib/FolderPickerControl/Images/shell32_303.ico");

      if (folder == null)
        folder = this.CreateImage("pack://application:,,,/LocalFilesDatabase.FSControls;component/FolderPickerLib/FolderPickerControl/Images/shell32_264.ico");
    }

    #region IValueConverter Members
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var treeItem = value as TreeItem;
      if (treeItem == null)
      {
        if (value != null)
          throw new ArgumentException("Illegal item type: '{0}'", value.GetType().FullName);
        else
          return null;
        ////throw new ArgumentException("Illegal item type: '(null)'");
      }

      if (treeItem is DriveTreeItem)
      {
        DriveTreeItem driveItem = treeItem as DriveTreeItem;
        switch (driveItem.DriveType)
        {
          case DriveType.CDRom:
            return cdrom;
          case DriveType.Fixed:
            return drive;
          case DriveType.Network:
            return netDrive;
          case DriveType.NoRootDirectory:
            return drive;
          case DriveType.Ram:
            return ram;
          case DriveType.Removable:
            return removable;
          case DriveType.Unknown:
            return drive;
        }
      }
      else
      {
        return folder;
      }

      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion

    private BitmapImage CreateImage(string uri)
    {
      BitmapImage img = new BitmapImage();
      img.BeginInit();
      img.UriSource = new Uri(uri);
      img.EndInit();
      return img;
    }
  }
}

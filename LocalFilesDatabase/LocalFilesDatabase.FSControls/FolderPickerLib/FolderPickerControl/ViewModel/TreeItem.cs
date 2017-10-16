namespace LocalFilesDatabase.FSControls.FolderPickerLib
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.IO;
  using System.Linq.Expressions;

  /// <summary>
  /// One item in the treeview of of items
  /// </summary>
  public class TreeItem : NotifiableObject
  {
    #region Private fields
    private string mName;
    private TreeItem mParent;
    private ObservableCollection<TreeItem> mChildren;
    #endregion

    #region Constructor
    /// <summary>
    /// Standard constructor from itemName and link to parent
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="parent"></param>
    public TreeItem(string itemName, TreeItem parent)
    {
      this.Name = itemName;
      this.IsFullyLoaded = false;
      this.Parent = parent;
      this.Children = new ObservableCollection<TreeItem>();
    }
    #endregion Constructor

    #region Properties

    public bool IsFullyLoaded { get; set; }

    public string Name
    {
      get
      {
        return this.mName;
      }

      set
      {
        this.mName = value;
        this.NotifyPropertyChanged(() => this.Name);
      }
    }

    public TreeItem Parent
    {
      get
      {
        return this.mParent;
      }

      set
      {
        this.mParent = value;
        this.NotifyPropertyChanged(() => this.Parent);
      }
    }

    public ObservableCollection<TreeItem> Children
    {
      get
      {
        return this.mChildren;
      }

      set
      {
        this.mChildren = value;
        this.NotifyPropertyChanged(() => this.Children);
      }
    }
    #endregion

    public string GetFullPath()
    {
      Stack<string> stack = new Stack<string>();

      var ti = this;

      while (ti.Parent != null)
      {
        stack.Push(ti.Name);
        ti = ti.Parent;
      }

      string path = stack.Pop();

      while (stack.Count > 0)
      {
        path = Path.Combine(path, stack.Pop());
      }

      return path;
    }
  }
}

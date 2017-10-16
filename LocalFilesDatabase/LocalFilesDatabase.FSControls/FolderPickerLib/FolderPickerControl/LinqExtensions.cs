namespace LocalFilesDatabase.FSControls.FolderPickerLib
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Linq.Expressions;

  public static class LinqExtensions
  {
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
      var result = new ObservableCollection<T>();

      foreach (var ci in source)
      {
        result.Add(ci);
      }

      return result;
    }
  }
}

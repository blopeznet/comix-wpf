namespace LocalFilesDatabase.FSControls.AutoComplete
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Data;

  [ValueConversion(typeof(int), typeof(int))]
  public class InvertSignConverter : IValueConverter
  {
    private static InvertSignConverter invertSignConverterInstance = new InvertSignConverter();

    public static InvertSignConverter InvertSignConverterInstance
    {
      get
      {
        return InvertSignConverter.invertSignConverterInstance;
      }
    }

    #region IValueConverter Members
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double val = (double)value;
      return val * -1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double val = (double)value;
      return val * -1;
    }

    #endregion
  }
}

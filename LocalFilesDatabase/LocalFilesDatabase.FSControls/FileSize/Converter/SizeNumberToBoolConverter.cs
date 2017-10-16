/****
 *
 * Source: http://www.codeproject.com/KB/WPF/MarkupExtensionsConverter.aspx
 * 
 ****/
namespace LocalFilesDatabase.FSControls.FileSize.Converter
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Windows.Data;
  using System.Windows.Markup;

  using FileSize;

  [MarkupExtensionReturnType(typeof(IValueConverter))]
  public class SizeNumberToBoolConverter : MarkupExtension, IValueConverter
  {
    private static SizeNumberToBoolConverter converter;

    public SizeNumberToBoolConverter()
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (converter == null)
      {
        converter = new SizeNumberToBoolConverter();
      }

      return converter;
    }
    #region IValueConverter Members

    /// <summary>
    /// Input unparsed string (such as "100Kb") plus an array of MinManx values
    /// of type ulong in parameter object and output whether the input can be
    /// converted OK and is within the MinMax range (true) or not (false).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter">Can contain an array of 2 UInt64 numbers
    /// which are interpreted as Min Max values for resulting number of bytes</param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
      {
        return false;
      }

      string stringInput = value as string;

      if (stringInput != null)
      {
        if (!string.IsNullOrEmpty(value.ToString()))
        {
          ulong numberOfBytes;
          string[] inputComponents;
          if (FileSizeContext.ConvertUnparsedSizeToBytesInt(value, out numberOfBytes, out inputComponents) == true)
          {
            ulong[] iMinMax = parameter as ulong[];

            if (iMinMax != null)
            {
              if (iMinMax.Length == 2)
              {
                ulong iMin = iMinMax[0];
                ulong iMax = iMinMax[1];

                if (numberOfBytes < iMin || numberOfBytes > iMax)
                {
                  // Number is out of bounds
                  return false;
                }
              }
            }

            // Number should be OK
            return true;
          }
        }
      }

      // Syntax error
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion
  }

  /// <summary>
  /// Input unparsed string array (such as ["100Kb"] or ["100"], ["Kb"])
  /// plus an array of MinManx values of type ulong in parameter object
  /// and output whether the input can be converted OK and is within the
  /// MinMax range (true) or not (false).
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter">Can contain an array of 2 UInt64 numbers
  /// which are interpreted as Min Max values for resulting number of bytes</param>
  /// </summary>
  [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
  public class SizeNumberToBoolConverter2 : MarkupExtension, IMultiValueConverter
  {
    private static SizeNumberToBoolConverter2 converter;

    public SizeNumberToBoolConverter2()
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (converter == null)
      {
        converter = new SizeNumberToBoolConverter2();
      }

      return converter;
    }

    #region IMultiValueConverter Members

    /// <summary>
    /// Input unparsed string (such as "100Kb") and
    /// output number in bytes as string ("102400 bytes")
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter">Can contain an array of 2 UInt64 numbers
    /// which are interpreted as Min Max values for resulting number of bytes</param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values == null)
      {
        return Binding.DoNothing;
      }

      string stringInput = string.Empty;

      if (values.Length == 1)
      {
        if (values[0] != null)
          stringInput = string.Format("{0}", values[0]);
      }
      else
      {
        if (values.Length == 2)
        {
          if (values[0] != null && values[1] != null)
            stringInput = string.Format("{0} {1}", values[0], values[1]);
        }
      }

      if (stringInput.Length == 0)
      {
        return Binding.DoNothing;
        ////throw new NotImplementedException(string.Format("One or Two input values are required for conversion, but there are; {0}", values.Length));
      }

      ulong numberOfBytes;
      string[] inputComponents;
      if (FileSizeContext.ConvertUnparsedSizeToBytesInt(stringInput, out numberOfBytes, out inputComponents) == true)
      {
        ulong[] iMinMax = parameter as ulong[];

        if (iMinMax != null)
        {
          if (iMinMax.Length == 2)
          {
            ulong iMin = iMinMax[0];
            ulong iMax = iMinMax[1];

            if (numberOfBytes < iMin || numberOfBytes > iMax)
              return false;  // Number is out of bounds
          }
        }

        // Number should be OK
        return true;
      }

      // Syntax error
      return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      return new object[] { value, value };
    }

    #endregion
  }
}

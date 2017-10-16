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

  /// <summary>
  /// Implement a Converter that converts an input string, such as:
  /// "100Gb"
  /// 
  /// into a correctly converted output string, such as: "10 bytes"
  /// </summary>
  [MarkupExtensionReturnType(typeof(IValueConverter))]
  public class NumberToSizeConverter : MarkupExtension, IValueConverter
  {
    private static NumberToSizeConverter converter;

    public NumberToSizeConverter()
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (converter == null)
      {
        converter = new NumberToSizeConverter();
      }

      return converter;
    }
    #region IValueConverter Members

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
    public object Convert(object value,
                          Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
      {
        return Binding.DoNothing;
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
                  string sMin, sMax;
                  MemUnit sizeUnit;
                  double d = NumberToBestSizeConverter.GetHumanReadableSize(iMin, out sizeUnit);
                  sMin = string.Format("{0} {1}", Math.Round(d, 2).ToString(), sizeUnit);

                  d = NumberToBestSizeConverter.GetHumanReadableSize(iMax, out sizeUnit);
                  sMax = string.Format("{0} {1}", Math.Round(d, 2).ToString(), sizeUnit);

                  return string.Format("(Out of range [{0}, {1}])", sMin, sMax);
                }
              }
            }

            return string.Format("{0} bytes", numberOfBytes);
          }
          else
          {
            return string.Format("Syntax Error: '{0}'", stringInput);
          }
        }
      }

      return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion
  }

  /// <summary>
  /// Implement a MULTIVALUE Converter that converts an array of input strings, such as:
  /// ["100Gb"] or ["100", "100Gb"]
  /// 
  /// into a correctly converted output string, such as: "10 bytes"
  /// 
  /// http://stackoverflow.com/questions/848946/is-it-possible-to-bind-wpf-combobox-selectedvalue-to-multiple-objectdataprovider
  /// </summary>
  [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
  public class NumberToSizeConverter2 : MarkupExtension, IMultiValueConverter
  {
    private static NumberToSizeConverter2 converter;

    public NumberToSizeConverter2()
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (converter == null)
      {
        converter = new NumberToSizeConverter2();
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

      // Convert an iinput string such as, "100 Mb" into a byte string represantation, such as, "1000 bytes"
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
            {
              string sMin, sMax;
              MemUnit sizeUnit;
              double d = NumberToBestSizeConverter.GetHumanReadableSize(iMin, out sizeUnit);
              sMin = string.Format("{0} {1}", Math.Round(d, 2).ToString(), sizeUnit);

              d = NumberToBestSizeConverter.GetHumanReadableSize(iMax, out sizeUnit);
              sMax = string.Format("{0} {1}", Math.Round(d, 2).ToString(), sizeUnit);

              return string.Format("(Out of range [{0}, {1}])", sMin, sMax);
            }
          }
        }

        return string.Format("{0} bytes", numberOfBytes);
      }

      return string.Format("Syntax Error: '{0}'", stringInput);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      return new object[] { value, value };
    }

    #endregion
  }
}

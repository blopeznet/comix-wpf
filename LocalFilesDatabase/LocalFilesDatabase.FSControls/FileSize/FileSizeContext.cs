namespace LocalFilesDatabase.FSControls.FileSize
{
  /***
   * 
   * Source: http://rextester.com/rundotnet?code=WMGOQ13650
   * 
   ****/
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class FileSizeContext
  {
    public FileSizeContext()
    {
      this.Input = string.Empty;
      this.InterpretOK = false;
    }

    public FileSizeContext(string input)
    {
      this.Input = input;
      this.InterpretOK = false;
    }

    public string Input { get; set; }

    /// <summary>
    /// Convert from input string to UInt64 for bytesize conversion
    /// </summary>
    public ulong Output { get; set; }

    /// <summary>
    /// Get/set property to monitor whether conversion is OK or Not.
    /// </summary>
    public bool InterpretOK { get; set; }

    /// <summary>
    /// Application Logic to convert unparsed string (such as "100Kb") and
    /// output number in bytes as UInt64
    /// </summary>
    /// <param name="value"></param>
    /// <param name="numberOfBytes"></param>
    /// <returns></returns>
    public static bool ConvertUnparsedSizeToBytesInt(object value,
                                                     out ulong numberOfBytes,
                                                     out string[] sInputComponents)
    {
      string stringInput = value as string;
      sInputComponents = null;
      numberOfBytes = 0;

      if (stringInput != null)
      {
        if (!string.IsNullOrEmpty(value.ToString()))
        {
          var ctx = new FileSizeContext((string)value);   // "10Mb"
          var parser = new FileSizeParser();

          parser.Interpret(ctx, out sInputComponents);

          numberOfBytes = ctx.Output;

          return parser.InterpretOK;
        }
      }

      return false;
    }

    internal class BytesFileSizeExpression : TerminalFileSizeExpression
    {
      protected override string ThisPattern()
      {
        return "bytes";
      }

      protected override string NextPattern()
      {
        return string.Empty;
      }
    }

    internal class KbFileSizeExpression : TerminalFileSizeExpression
    {
      protected override string ThisPattern()
      {
        return "Kb";
      }
      
      protected override string NextPattern()
      {
        return "bytes";
      }
    }

    internal class MbFileSizeExpression : TerminalFileSizeExpression
    {
      protected override string ThisPattern()
      {
        return "Mb";
      }

      protected override string NextPattern()
      {
        return "Kb";
      }
    }

    internal class GbFileSizeExpression : TerminalFileSizeExpression
    {
      protected override string ThisPattern()
      {
        return "Gb";
      }

      protected override string NextPattern()
      {
        return "Mb";
      }
    }

    internal class TbFileSizeExpression : TerminalFileSizeExpression
    {
      protected override string ThisPattern()
      {
        return "Tb";
      }

      protected override string NextPattern()
      {
        return "Gb";
      }
    }
  }
}

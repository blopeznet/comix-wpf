namespace LocalFilesDatabase.FSControls.FileSize
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  internal class FileSizeParser : FileSizeExpression
  {
    private List<FileSizeExpression> expressionTree = new List<FileSizeExpression>()
    {
        new FileSizeContext.TbFileSizeExpression(),
        new FileSizeContext.GbFileSizeExpression(),
        new FileSizeContext.MbFileSizeExpression(),
        new FileSizeContext.KbFileSizeExpression()
    };

    public override void Interpret(FileSizeContext value, out string[] strComponents)
    {
      strComponents = null;

      // Go through each defined expression and see if we can match it with a known unit
      // Compute next smaller unit (eg. convert from Gb to Mb) and keep going until we hit bytes
      foreach (FileSizeExpression exp in this.expressionTree)
      {
        string[] strOutComponents = null;
      
        // Adjust value to next smaller unit (if any)
        exp.Interpret(value, out strOutComponents);

        // Found a match and converted OK -> Return to sender
        if (exp.InterpretOK == true)
        {
          // return recognized and cleaned string components from input
          if (this.InterpretOK == false)
            strComponents = strOutComponents;

          this.InterpretOK = true;
        }
      }

      // Attempt 'bytes conversion' as last resort
      if (this.InterpretOK == false)
        this.InterpretAsBytes(value);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor.DTOs
{
  public class SubtitlePair
  {
    public Subtitle Native { get; set; }
    public Subtitle Translated { get; set; }

    public void Print(int limit = 3)
    {
      Console.WriteLine("-- NATIVE -- ");
      Native.Print(limit);
      Console.WriteLine("\n-- TRANSLATED -- ");
      Translated.Print(limit);
      Console.WriteLine();
    }
  }
}

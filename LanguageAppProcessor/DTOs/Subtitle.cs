using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor
{
  public class Subtitle
  {
    public string MovieName { get; set; }
    public IEnumerable<SubtitleInterval> Intervals { get; set; }

    public void Print(int limit = 3)
    {
      Console.WriteLine($"Subtitles for: {MovieName}");
      int i = 0;
      foreach (var interval in Intervals)
      {
        if (i == limit) break;
        Console.WriteLine($"[{interval.TimeFrame.Start} -> {interval.TimeFrame.End}]");
        foreach (var line in interval.Lines)
        {
          Console.WriteLine($"\t{line}");
        }
        i++;
      }
    }
  }
}

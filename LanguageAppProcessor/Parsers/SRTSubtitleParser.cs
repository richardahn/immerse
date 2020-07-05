using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageAppProcessor
{
  public class SRTSubtitleParser : SubtitleParser
  {
    public override Subtitle Parse(string filePath)
    {
      Subtitle subtitle = new Subtitle
      {
        MovieName = Regex.Match(filePath.Split('\\').Last(), @".*(?=_.*?\.)").Value.Replace('_', ' '),
        Intervals = GetIntervals(filePath)
      };
      return subtitle;
    }

    public IEnumerable<SubtitleInterval> GetIntervals(string filePath)
    {
      using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
      {
        var interval = GetInterval(sr);
        while (interval != null)
        {
          yield return interval;
          interval = GetInterval(sr);
        }
      }
    }
    public SubtitleInterval GetInterval(StreamReader sr)
    {
      // Process initial padding
      string index;
      do
      {
        index = sr.ReadLine();
      } while (index != null && index == ""); // Skip empty lines at beginning
      if (index == null)
      {
        return null;
      }

      try
      {
        string timeFrame = sr.ReadLine();
        // Process all lines of dialogue
        List<string> lines = new List<string>();
        string line;
        while(true)
        {
          line = sr.ReadLine();
          if (line == null || line == "")
          {
            break;
          }
          lines.Add(CleanLine(line));
        }
        return new SubtitleInterval
        {
          Index = int.Parse(index),
          TimeFrame = TimeFrame.Parse(timeFrame),
          Lines = lines,
        };
      } 
      catch(Exception ex)
      {
        Console.WriteLine(ex);
        throw;
      }
    }

    private string CleanLine(string line)
    {
      return Regex.Replace(line, @"(<.*?>)", "");
    }
  }
}

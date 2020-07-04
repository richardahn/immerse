using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageAppProcessor
{
  public class SMISubtitleParser : SubtitleParser
  {
    private int Index { get; set; }
    public override Subtitle Parse(string filePath)
    {
      Index = 0; // Reset index
      Subtitle subtitle = new Subtitle
      {
        MovieName = filePath.Split('\\').Last(),
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
      // Read until find Sync Start
      string start;
      do
      {
        start = sr.ReadLine();
      } while (start != null && !IsStart(start));
      if (start == null)
      {
        return null;
      }

      // Read lines until no lines found
      List<string> lines = new List<string>();
      string line;
      while (true)
      {
        line = sr.ReadLine();
        if (!IsLine(line))
        {
          break;
        }
        lines.AddRange(GetLines(line));
      }

      // Can be already at sync start at this point, or not
      string end = line;
      while (!IsStart(end))
      {
        end = sr.ReadLine();
      }

      // Assert that the following line is a blank 
      string blank = sr.ReadLine();
      if (!IsBlank(blank))
      {
        throw new Exception("Found a nonblank, consider changing approach");
      }

      return new SubtitleInterval
      {
        Index = Index++,
        TimeFrame = new TimeFrame(GetMilliseconds(start), GetMilliseconds(end)),
        Lines = lines,
      };
    }
    private bool IsBlank(string line)
    {
      return line.Contains("<P CLASS=SUBTTL>&nbsp;");
    }
    private int GetMilliseconds(string line)
    {
      return int.Parse(Regex.Match(line, @"(?<=START=)\d+").Value);
    }
    private string[] GetLines(string line)
    {
      string lineRaw = Regex.Match(line, @"(?<=<P CLASS=SUBTTL>).+").Value;
      return Regex.Split(lineRaw, @"<br>");
    }
    private bool IsStart(string line)
    {
      return line.Contains("<SYNC");
    }
    private bool IsLine(string line)
    {
      return line.Contains("<P");
    }
  }
}

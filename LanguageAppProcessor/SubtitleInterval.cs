using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageAppProcessor
{
  public class SubtitleInterval
  {
    public int? Index { get; set; }
    public TimeFrame TimeFrame { get; set; }
    public List<string> Lines { get; set; }
  }
  public class TimeFrame
  {
    public static TimeFrame Parse(string input, string format = @"hh\:mm\:ss\,fff")
    {
      var split = Regex.Split(input, @" --> ");
      return new TimeFrame
      {
        Start = TimeSpan.ParseExact(split[0], format, null),
        End = TimeSpan.ParseExact(split[1], format, null),
      };
    }
    public TimeFrame() { }
    public TimeFrame(int startMs, int endMs)
    {
      Start = TimeSpan.FromMilliseconds(startMs);
      End = TimeSpan.FromMilliseconds(endMs);
    }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
  }
}

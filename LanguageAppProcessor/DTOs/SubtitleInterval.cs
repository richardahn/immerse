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
    public string JoinedLines => string.Join(". ", Lines);


    public double Error(SubtitleInterval b)
    {
      return Error(this, b);
    }
    public double StartError(SubtitleInterval b)
    {
      return StartError(this, b);
    }
    public double EndError(SubtitleInterval b)
    {
      return EndError(this, b);
    }
    public static double Error(SubtitleInterval a, SubtitleInterval b)
    {
      return StartError(a, b) + EndError(a, b);
    }
    public static double StartError(SubtitleInterval a, SubtitleInterval b)
    {
      return Math.Abs((a.TimeFrame.Start - b.TimeFrame.Start).TotalSeconds);
    }
    public static double EndError(SubtitleInterval a, SubtitleInterval b)
    {
      return Math.Abs((a.TimeFrame.End - b.TimeFrame.End).TotalSeconds);
    }
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

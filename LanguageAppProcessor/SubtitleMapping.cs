using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageAppProcessor
{
  public class SubtitleIntervalMapping
  {
    public SubtitleIntervalMapping(SubtitleInterval input, SubtitleInterval target)
    {
      Input = input;
      Target = target;
    }
    public SubtitleInterval Input { get; set; }
    public SubtitleInterval Target { get; set; }
    public double Error => (Input.TimeFrame.Start - Target.TimeFrame.Start).TotalSeconds;
  }
  public class SubtitleMapping
  {
    public List<SubtitleIntervalMapping> Intervals { get; set; }

    public SubtitleMapping(Subtitle input, Subtitle target)
    {
      Intervals = new List<SubtitleIntervalMapping>();
      int currentStartIndex = 0;
      List<SubtitleInterval> searchRange = target.Intervals.ToList();
      foreach (var interval in input.Intervals)
      {
        int closestIntervalIndex = Search.Look(interval.TimeFrame.Start, currentStartIndex, searchRange);
        Intervals.Add(new SubtitleIntervalMapping(interval, searchRange[closestIntervalIndex]));
        currentStartIndex = closestIntervalIndex;
      }
    }

    public void Filter(Func<SubtitleIntervalMapping, bool> condition)
    {
      Intervals = Intervals.Where(condition).ToList();
    }

    public void PrintErrorHistogram(double bucketSize = 0.5)
    {
      Dictionary<int, List<SubtitleIntervalMapping>> histogram = new Dictionary<int, List<SubtitleIntervalMapping>>();
      foreach (var interval in Intervals)
      {
        int key = (int)(interval.Error / bucketSize);
        if (!histogram.ContainsKey(key))
        {
          histogram.Add(key, new List<SubtitleIntervalMapping>());
        }
        histogram[key].Add(interval);
      }
      var list = histogram.Keys.ToList();
      list.Sort();
      foreach (var key in list)
      {
        Console.WriteLine($"Bucket {key}: {histogram[key].Count}");
      }
    }
    public void Print()
    {
      foreach (var interval in Intervals)
      {
        string lineJoiner = ". ";
        Console.WriteLine($"{string.Join(lineJoiner, interval.Input.Lines)}  -->  {string.Join(lineJoiner, interval.Target.Lines)}, {interval.Error}");
      }
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageAppProcessor.DTOs
{
  public class SubtitleMapping
  {
    public string MovieName { get; set; }
    public List<SubtitleIntervalMapping> Intervals { get; set; }


    public void PrintMismatches()
    {
      int numMatching = 0;
      foreach (var interval in Intervals)
      {
        if (interval.Input.Lines.Count == interval.Target.Lines.Count)
        {
          numMatching++;
        }
      }
      Console.WriteLine($"Matches: {numMatching} / {Intervals.Count} = {numMatching / Intervals.Count}");
    }
    public void PrintErrorHistogram(double bucketSize = 0.5)
    {
      Dictionary<int, List<SubtitleIntervalMapping>> histogram = new Dictionary<int, List<SubtitleIntervalMapping>>();
      double sum = 0;
      foreach (var interval in Intervals)
      {
        sum += interval.Error;
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
      Console.WriteLine($"Average Error: {sum / Intervals.Count}");
    }
    public void Print(int limit = 5)
    {
      int i = 0;
      foreach (var interval in Intervals)
      {
        if (i == limit) break;
        Console.WriteLine($"[Error = {interval.Error:0.00}]\n\t{interval.Input.JoinedLines}\n\n\t  --> \n\n\t{interval.Target.JoinedLines}");
        i++;
      }
    }
  }
}

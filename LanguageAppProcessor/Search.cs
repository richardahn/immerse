using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor
{
  public static class Search
  {
    public static int Look(TimeSpan value, int startIndex, List<SubtitleInterval> list)
    {
      if (startIndex < 0 || startIndex >= list.Count)
      {
        return -1;
      }

      int i = startIndex;
      int direction = value < list[i].TimeFrame.Start ? -1 : 1;
      //Console.WriteLine($"Look direction: {direction}");
      double min = double.MaxValue;
      while (i >= 0 && i < list.Count)
      {
        double currMin = Math.Abs((list[i].TimeFrame.Start - value).TotalSeconds);
        //Console.Write($"Looking for {value}, found list[{i}] = {list[i].TimeFrame.Start}, ");
        if (currMin < min)
        {
          //Console.WriteLine($"diff is {currMin}, attempting to move {direction}");
          min = currMin;
          i += direction;
        }
        else
        {
          i -= direction;
          //Console.WriteLine($"Min {currMin} is not bigger, closest is {list[i].TimeFrame.Start}");
          return i;
        }
      }
      i -= direction;
      //Console.WriteLine($"Reached edge, closest is {list[i].TimeFrame.Start}");
      return i;
    }
    public static int Look(double value, int startIndex, double[] list)
    {
      int i = startIndex;
      if (startIndex < 0 || startIndex >= list.Length)
      {
        return -1;
      }

      int direction = value < list[i] ? -1 : 1;
      Console.WriteLine($"Look direction: {direction}");
      double min = double.MaxValue;
      while (i >= 0 && i < list.Length)
      {
        double curr = list[i];
        double currMin = Math.Abs(list[i] - value);
        Console.Write($"Comparing {value} and list[{i}] = {list[i]}, diff is {currMin}, ");
        if (currMin < min)
        {
          Console.WriteLine($"Found min {currMin}");
          min = currMin;
          i += direction;
        }
        else
        {
          i -= direction;
          Console.WriteLine($"Min {currMin} is not bigger, closest is {list[i]}");
          break;
        }
      }
      return i;
    }
  }
}

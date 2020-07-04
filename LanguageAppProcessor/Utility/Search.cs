using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor
{
  public static class Search
  {
    public static int Look(TimeSpan target, int searchStartingPoint, List<SubtitleInterval> list, Func<TimeFrame, TimeSpan> selector)
    {
      if (searchStartingPoint < 0 || searchStartingPoint >= list.Count)
      {
        return -1;
      }

      int i = searchStartingPoint;
      int direction = target < selector(list[i].TimeFrame) ? -1 : 1;
      double runningDistance = double.MaxValue;
      while (i >= 0 && i < list.Count)
      {
        double currentDistance = Math.Abs((selector(list[i].TimeFrame) - target).TotalSeconds);
        if (currentDistance < runningDistance)
        {
          runningDistance = currentDistance;
          i += direction;
        }
        else
        {
          i -= direction;
          return i;
        }
      }
      i -= direction;
      return i;
    }
  }
}

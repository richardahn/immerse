using LanguageAppProcessor.DTOs;
using LanguageAppProcessor.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageAppProcessor
{
  public class SubtitleMapper : IPipelineProcessor<SubtitlePair, SubtitleMapping>
  {
    public string Source { get; set; }

    public Func<SubtitleIntervalMapping, bool> IntervalsFilter { get; set; }
    public event Action<SubtitlePair> Started;
    public event Action<SubtitlePair, SubtitleMapping> Finished;
    public SubtitleMapper WithIntervalsFilter(Func<SubtitleIntervalMapping, bool> condition)
    {
      IntervalsFilter = condition;
      return this;
    }
    public SubtitleMapping Process(SubtitlePair inputPair)
    {
      Started?.Invoke(inputPair);
      var input = inputPair.Native;
      var target = inputPair.Translated;

      var mapping = new SubtitleMapping
      {
        MovieName = input.MovieName,
        Intervals = new List<SubtitleIntervalMapping>(),
      };
      Source = input.MovieName;
      int searchStartingPoint = 0;
      List<SubtitleInterval> searchRange = target.Intervals.ToList();

      SubtitleIntervalMapping aggregate = null;
      int aggregateStart = 0;
      int aggregateEnd = 0;
      var intervals = input.Intervals.ToList();
      for (int i = 0; i < intervals.Count; i++)
      {
        var interval = intervals[i];

        // Get current interval
        int closestStartIntervalIndex = Search.Look(interval.TimeFrame.Start, searchStartingPoint, searchRange, tf => tf.Start);
        int closestEndIntervalIndex = Math.Max(closestStartIntervalIndex, Search.Look(interval.TimeFrame.End, searchStartingPoint, searchRange, tf => tf.End)); // Your end point accuracy can be bad because merges haven't come in yet, so always go higher
        double intervalStartError = interval.StartError(searchRange[closestStartIntervalIndex]);
        double intervalEndError = interval.EndError(searchRange[closestEndIntervalIndex]);
        double intervalError = intervalStartError + intervalEndError;

        if (aggregate == null)
        {
          aggregate = new SubtitleIntervalMapping(interval, null);
          aggregateStart = closestStartIntervalIndex;
          aggregateEnd = closestEndIntervalIndex;
        }
        else
        {
          // Previous aggregate exists
          double aggregateEndError = aggregate.Input.EndError(searchRange[aggregateEnd]);
          double nextStartError = ((i + 1) >= intervals.Count) ? 0 : intervals[i + 1].StartError(searchRange[Search.Look(intervals[i + 1].TimeFrame.Start, searchStartingPoint, searchRange, tf => tf.Start)]);

          double errorWithoutAggregation = Math.Sqrt(Math.Pow(aggregateEndError, 2) + Math.Pow(intervalStartError, 2));
          double errorWithAggregation = Math.Sqrt(Math.Pow(intervalEndError, 2) + Math.Pow(nextStartError, 2) + Math.Pow(0.2 * (interval.TimeFrame.End - aggregate.Input.TimeFrame.End).TotalSeconds, 2));
          if (errorWithAggregation < errorWithoutAggregation)
          {
            // Add to aggregate
            aggregateEnd = closestEndIntervalIndex;
            aggregate.Input = MergeInterval(aggregate.Input, interval);
          }
          else
          {
            // Finish aggregate 
            aggregate.Target = AggregateInterval(searchRange, aggregateStart, aggregateEnd);
            mapping.Intervals.Add(aggregate);

            // Start a new one starting from interval
            aggregate = new SubtitleIntervalMapping(interval, null);
            aggregateStart = closestStartIntervalIndex;
            aggregateEnd = closestEndIntervalIndex;
          }
        }
        searchStartingPoint = closestEndIntervalIndex; // Boosts performance by giving a better starting point
      }
      aggregate.Target = AggregateInterval(searchRange, aggregateStart, aggregateEnd);
      mapping.Intervals.Add(aggregate);
      if (IntervalsFilter != null)
      {
        mapping.Intervals = mapping.Intervals.Where(IntervalsFilter).ToList();
      }
      Finished?.Invoke(inputPair, mapping);
      return mapping;
    }
    private SubtitleInterval MergeInterval(SubtitleInterval a, SubtitleInterval b)
    {
      return new SubtitleInterval
      {
        Index = a.Index,
        Lines = a.Lines.Concat(b.Lines).ToList(),
        TimeFrame = new TimeFrame
        {
          Start = a.TimeFrame.Start,
          End = b.TimeFrame.End,
        }
      };
    }
    private SubtitleInterval AggregateInterval(List<SubtitleInterval> list, int start, int end)
    {
      SubtitleInterval aggregated = list[start];
      for (int i = start + 1; i <= end; i++)
      {
        aggregated = MergeInterval(aggregated, list[i]);
      }
      return aggregated;
    }

  }
}

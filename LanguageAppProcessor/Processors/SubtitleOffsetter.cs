using LanguageAppProcessor.DTOs;
using LanguageAppProcessor.Interfaces;
using LanguageAppProcessor.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LanguageAppProcessor
{
  public class SubtitleOffsetter : IPipelineProcessor<SubtitlePair, SubtitlePair>
  {
    // -- Hyperparameters --
    public double OffsetStart { get; set; }
    public double LearningRate { get; set; }
    public double Eta { get; set; }
    public int Iterations { get; set; }


    // -- History --
    public class Iteration
    {
      /// <summary>
      /// The input offset for this iteration
      /// </summary>
      public double Offset { get; set; }
      /// <summary>
      /// Mean Absolute Error
      /// </summary>
      public double Error { get; set; }
      /// <summary>
      /// Mean Absolute Error Gradient
      /// </summary>
      public double Gradient { get; set; }
    }
    public List<Iteration> History { get; set; }
    public void PrintHistory()
    {
      if (History == null)
        return;

      foreach (var item in History)
      {
        Console.WriteLine($"Offset = {item.Offset}\t Error = {item.Error}\t Gradient = {item.Gradient}");
      }
    }


    public event Action<SubtitlePair> Started;
    public event Action<SubtitlePair, SubtitlePair> Finished;
    public SubtitleOffsetter(double offsetStart = 0, double learningRate = 0.1, double eta = 0.01, int iterations = 1000)
    {
      OffsetStart = offsetStart;
      LearningRate = learningRate;
      Eta = eta;
      Iterations = iterations;
    }
    public Subtitle Transform(Subtitle input, Subtitle target)
    {
      History = new List<Iteration>();
      double offset = OffsetStart;
      for (int i = 0; i < Iterations; i++)
      {
        Iteration iteration = Iterate(input, target, offset);
        History.Add(iteration);
        if (Math.Abs(iteration.Gradient) < Eta)
        {
          break;
        }
        offset -= LearningRate * iteration.Gradient;
      }

      return new Subtitle
      {
        MovieName = input.MovieName,
        Intervals = input.Intervals.Select(i => new SubtitleInterval
        {
          Index = i.Index,
          Lines = new List<string>(i.Lines),
          TimeFrame = new TimeFrame()
          {
            Start = i.TimeFrame.Start.Add(TimeSpan.FromSeconds(offset)),
            End = i.TimeFrame.End.Add(TimeSpan.FromSeconds(offset)),
          }
        })
      };
    }
    public SubtitlePair Process(SubtitlePair input) // warning: impure
    {
      Started?.Invoke(input);
      Subtitle offsettedNativeInput = Transform(input.Native, input.Translated);
      input.Native = offsettedNativeInput;
      Finished?.Invoke(input, input);
      return input;
    }

    /// <summary>
    /// Calculates the gradient(and other information) for the input w/ an offset relative to a target
    /// </summary>
    private Iteration Iterate(Subtitle input, Subtitle target, double inputOffset)
    {
      double errorSum = 0;
      double gradientSum = 0;
      int currentStartIndex = 0;
      List<SubtitleInterval> searchRange = target.Intervals.ToList();
      foreach (var interval in input.Intervals)
      {
        var yPred = interval.TimeFrame.Start.Add(TimeSpan.FromSeconds(inputOffset));
        int closestIntervalIndex = Search.Look(yPred, currentStartIndex, searchRange, tf => tf.Start);
        var y = searchRange[closestIntervalIndex].TimeFrame.Start;

        double error = (yPred - y).TotalSeconds; 
        errorSum += Math.Abs(error); // MAE error
        gradientSum += error >= 0 ? 1 : -1; // MAE error gradient
        currentStartIndex = closestIntervalIndex;
      }
      int M = input.Intervals.Count();
      return new Iteration
      {
        Offset = inputOffset,
        Error = errorSum / M,
        Gradient = gradientSum / M,
      };
    }
  }
}

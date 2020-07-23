using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAppProcessor.Pipeline
{
  public class Pipeline<TPipelineIn, TPipelineOut> : IStoppable
  {
    // An interfaced step is necessary to reference the next step's buffer
    interface IBufferable<T>
    {
      BlockingCollection<Item<T>> Buffer { get; set; }
    }
    class PipelineStep<TStepIn, TStepOut> : IBufferable<TStepIn>, IStoppable
    {
      public IPipelineProcessor<TStepIn, TStepOut> Processor { get; }
      public BlockingCollection<Item<TStepIn>> Buffer { get; set; } = new BlockingCollection<Item<TStepIn>>();

      public PipelineStep(IPipelineProcessor<TStepIn, TStepOut> processor) { Processor = processor; }

      public TStepOut Process(TStepIn input)
      {
        return Processor.Process(input);
      }
      public void Stop()
      {
        Buffer.CompleteAdding();
      }
    }
    /// <summary>
    /// Represents a step in the pipeline. Is composed of the actual PipelineStep
    /// </summary>
    class PipelineStepContainer : IStoppable
    {
      public object Value { get; set; } // Represents an object of type PipelineStep<,>

      public void Stop()
      {
        (Value as IStoppable).Stop();
      }
    }
    /// <summary>
    /// Represents an item in the pipeline. 
    /// </summary>
    class Item<T>
    {
      public T Value { get; set; } // Represents some data being transferred throughout the pipeline
      public TaskCompletionSource<TPipelineOut> TaskCompletionSource { get; set; } // Represents the 
    }
    List<PipelineStepContainer> _pipelineSteps = new List<PipelineStepContainer>(); // Reference to all steps

    public Pipeline<TPipelineIn, TPipelineOut> AddStepAndStart<TIn, TOut>(IPipelineProcessor<TIn, TOut> processor)
    {
      var step = new PipelineStep<TIn, TOut>(processor);
      int stepIndex = _pipelineSteps.Count;

      // Alternatively, I can store a list of the Task.Run() handlers here and then run them at a later point
      // Start the step
      Task.Run(() =>
      {
        // Cached next step
        IBufferable<TOut> nextBuffer = null;

        foreach (Item<TIn> input in step.Buffer.GetConsumingEnumerable())
        {
          // Give us the most up-to-date status on our current position in the pipeline
          bool isLastStep = stepIndex == _pipelineSteps.Count - 1;

          TOut outputValue;

          // Attempt to process value
          try
          {
            outputValue = processor.Process(input.Value);
          }
          catch (Exception e)
          {
            input.TaskCompletionSource.SetException(e);
            continue;
          }

          // Move to next step
          if (isLastStep)
          {
            input.TaskCompletionSource.SetResult((TPipelineOut)(object)outputValue);
          }
          else
          {
            nextBuffer = nextBuffer ?? _pipelineSteps[stepIndex + 1].Value as IBufferable<TOut>;
            try // In the case of the pipeline closing prematurely, catch the error and throw an exception to the task
            {
              nextBuffer.Buffer.Add(new Item<TOut>
              {
                Value = outputValue,
                TaskCompletionSource = input.TaskCompletionSource
              });
            }
            catch (InvalidOperationException e)
            {
              input.TaskCompletionSource.SetException(e);
            }
          }
        }
      });
      _pipelineSteps.Add(new PipelineStepContainer
      {
        Value = step,
      });
      return this;
    }
    public Task<TPipelineOut> Execute(TPipelineIn input)
    {
      TaskCompletionSource<TPipelineOut> tcs = new TaskCompletionSource<TPipelineOut>();
      (_pipelineSteps.First().Value as IBufferable<TPipelineIn>).Buffer.Add(new Item<TPipelineIn>
      {
        Value = input,
        TaskCompletionSource = tcs,
      });
      return tcs.Task;
    }

    public void Stop()
    {
      foreach (var step in _pipelineSteps)
      {
        step.Stop();
      }
    }
  }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAppProcessor.Processors
{
  // An interfaced step is necessary to reference the next step's buffer
  public interface IBufferable<T>
  {
    BlockingCollection<T> Buffer { get; set; }
  }
  public abstract class PipelineStep<TStepIn, TStepOut> : IBufferable<TStepIn>
  {
    public BlockingCollection<TStepIn> Buffer { get; set; } = new BlockingCollection<TStepIn>();
    public abstract TStepOut Process(TStepIn input);
  }
  //public static class PipelineExtensions
  //{
  //  public static TOut Add<TIn, TOut, TPipelineIn, TPipelineOut>
  //    (this TIn inputType, 
  //    Pipeline<TPipelineIn, TPipelineOut> pipeline,
  //    PipelineStep<TIn, TOut> step)
  //  {
  //    pipeline.AddStepAndStart(step);
  //    return default;
  //  }
  //}
  public class Pipeline<TPipelineIn, TPipelineOut> 
  {
    List<object> _pipelineSteps = new List<object>(); // Reference to all steps
    public event Action<TPipelineOut> Finished; 

    public Pipeline() { }
    //public Pipeline(Func<Pipeline<TPipelineIn, TPipelineOut>, TPipelineIn, TPipelineOut> buildPipeline)
    //{
    //  buildPipeline.Invoke(this, default);
    //}
    public Pipeline<TPipelineIn, TPipelineOut> AddStepAndStart<TIn, TOut>(PipelineStep<TIn, TOut> step)
    {
      int stepIndex = _pipelineSteps.Count;
      // Start the step
      Task.Run(() =>
      {
        // Cached next step
        IBufferable<TOut> nextBuffer = null;

        foreach (var input in step.Buffer.GetConsumingEnumerable())
        {
          // Give us the most up-to-date status on our current position in the pipeline
          bool isLastStep = stepIndex == _pipelineSteps.Count - 1;
          TOut output = step.Process(input);

          if (isLastStep)
          {
            Finished?.Invoke((TPipelineOut)(object)output);
          }
          else
          {
            nextBuffer = nextBuffer ?? _pipelineSteps[stepIndex + 1] as IBufferable<TOut>;
            nextBuffer.Buffer.Add(output);
          }
        }
      });
      _pipelineSteps.Add(step);
      return this;
    }
    public void Execute(TPipelineIn input)
    {
      (_pipelineSteps.First() as IBufferable<TPipelineIn>).Buffer.Add(input);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor.Interfaces
{
  public interface IPipelineProcessorLifecycle<TIn, TOut>
  {
    public event Action<TIn> Started;
    public event Action<TIn, TOut> Finished;
  }
}

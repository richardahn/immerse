using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageAppProcessor.Pipeline
{
  /// <summary>
  /// Base interface for implementing a processor into the pipeline
  /// </summary>
  public interface IPipelineProcessor<TStepIn, TStepOut>
  {
    TStepOut Process(TStepIn input);
  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor.DTOs
{
  public class SubtitleConversation
  {
    public List<SubtitleIntervalMapping> Intervals { get; set; } = new List<SubtitleIntervalMapping>();
    public SubtitleConversation Add(SubtitleIntervalMapping interval)
    {
      Intervals.Add(interval);
      return this;
    }
  }
}

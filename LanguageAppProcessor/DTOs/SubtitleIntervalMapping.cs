using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor.DTOs
{
  public class SubtitleIntervalMapping
  {
    public SubtitleIntervalMapping(SubtitleInterval input, SubtitleInterval target)
    {
      Input = input;
      Target = target;
    }
    public SubtitleInterval Input { get; set; }
    public SubtitleInterval Target { get; set; }
    public double Error => Input.Error(Target);
  }
}

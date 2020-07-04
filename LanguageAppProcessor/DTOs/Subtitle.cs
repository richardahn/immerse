using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor
{
  public class Subtitle
  {
    public string MovieName { get; set; }
    public IEnumerable<SubtitleInterval> Intervals { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor
{
  public abstract class SubtitleParser
  {
    public abstract Subtitle Parse(string filepath);
  }
}

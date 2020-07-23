using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor.Parsers
{
  public abstract class SubtitleFileParser
  {
    public abstract Subtitle Parse(string filepath);
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageAppProcessor.Parsers
{
  public abstract class SubtitleFileParser
  {
    protected static string GetMovieName(string filePath) => Regex.Match(filePath.Split('\\').Last(), @".*(?=_.*?\.)").Value.Replace('_', ' ');
    public abstract Subtitle Parse(string filePath);
  }
}

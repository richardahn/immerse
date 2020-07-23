using LanguageAppProcessor.DTOs;
using LanguageAppProcessor.Interfaces;
using LanguageAppProcessor.Parsers;
using LanguageAppProcessor.Pipeline;
using System;
using System.Linq;

namespace LanguageAppProcessor.Processors
{
  public class SubtitleParser : IPipelineProcessor<SubtitleFilePathPair, SubtitlePair>
  {
    public event Action<SubtitleFilePathPair> Started;
    public event Action<SubtitleFilePathPair, SubtitlePair> Finished;

    static string GetExtension(string path)
    {
      return path.Split('.').Last();
    }
    static Subtitle Parse(string filePath)
    {
      string extension = GetExtension(filePath);
      SubtitleFileParser parser;
      if (extension == "srt")
      {
        parser = new SRTSubtitleFileParser();
      }
      else
      {
        parser = new SMISubtitleFileParser();
      }
      return parser.Parse(filePath);
    }

    public SubtitlePair Process(SubtitleFilePathPair input)
    {
      Started?.Invoke(input);
      var output = new SubtitlePair
      {
        Native = Parse(input.Native),
        Translated = Parse(input.Translated),
      };
      Finished?.Invoke(input, output);
      return output;
    }
  }
}

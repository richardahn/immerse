using LanguageAppProcessor.DTOs;
using LanguageAppProcessor.Parsers;
using LanguageAppProcessor.Pipeline;
using System.Linq;

namespace LanguageAppProcessor.Processors
{
  public class SubtitleParser : IPipelineProcessor<SubtitleFilePathPair, SubtitlePair>
  {
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
      return new SubtitlePair
      {
        Native = Parse(input.Native),
        Translated = Parse(input.Translated),
      };
    }
  }
}

using LanguageAppProcessor.DTOs;
using LanguageAppProcessor.Parsers;
using LanguageAppProcessor.Pipeline;
using System.Linq;

namespace LanguageAppProcessor.Processors
{
  public class SubtitleParser : IPipelineProcessor<SubtitleFilePathPair, SubtitlePair>
  {
    static string GetExtension(string fileName)
    {
      return fileName.Split('.').Last();
    }
    static SubtitleFileParser GetParserFromExtension(string extension)
    {
      if (extension == "srt")
      {
        return new SRTSubtitleFileParser();
      }
      else
      {
        return new SMISubtitleFileParser();
      }
    }
    public SubtitlePair Process(SubtitleFilePathPair input)
    {
      return new SubtitlePair
      {
        Native = GetParserFromExtension(GetExtension(input.Native))
         .Parse(input.Native),
        Translated = GetParserFromExtension(GetExtension(input.Translated))
        .Parse(input.Translated),
      };
    }
  }
}

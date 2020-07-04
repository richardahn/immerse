using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LanguageAppProcessor
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.OutputEncoding = Encoding.UTF8;
      string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
      string filePath = @$"{rootDir}\Data\knk_eng.srt";
      string filePathKor = $@"{rootDir}\Data\knk_kor.srt";
      var parser = new SRTSubtitleParser();
      var parserKor = new SRTSubtitleParser();
      var subtitle = parser.Parse(filePath);
      var subtitleKor = parserKor.Parse(filePathKor);

      // Aggregator


      // Offset
      var offsetter = new SubtitleOffsetter();
      var transformed = offsetter.Transform(subtitle, subtitleKor);
      offsetter.PrintHistory();

      // Map
      var mapping = new SubtitleMapping(transformed, subtitleKor);
      mapping.Print();
    }
  }
}

using LanguageAppProcessor.DTOs;
using LanguageAppProcessor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageAppProcessor
{
  public class TranslationProcessor
  {
    public class ProcessedTranslation
    {
      public SubtitleConversations Conversations { get; set; }
      public void Save(TranslationContext context)
      {
        var conversations = Conversations.Conversations.Select(c => new Conversation
        {
          Source = Conversations.Source,
          Lines = c.Intervals.Select(i => new ConversationLine
          {
            NativeText = i.Input.JoinedLines,
            TranslatedText = i.Target.JoinedLines,
            Start = i.Input.TimeFrame.Start,
            End = i.Input.TimeFrame.End,
            Error = i.Error,
            Index = i.Input.Index.GetValueOrDefault(),
          }).ToList()
        });
        context.Conversations.AddRange(conversations);
        context.SaveChanges();
      }
    }
    static SubtitleParser ParserFromExtension(string extension)
    {
      if (extension == "srt")
      {
        return new SRTSubtitleParser();
      }
      else
      {
        return new SMISubtitleParser();
      }
    }
    public ProcessedTranslation Process(string nativeFilePath, string translatedFilePath)
    {
      var nativeParser = ParserFromExtension(nativeFilePath.Split('.').Last());
      var translatedParser = ParserFromExtension(translatedFilePath.Split('.').Last());
      var nativeSubtitle = nativeParser.Parse(nativeFilePath);
      var translatedSubtitle = translatedParser.Parse(translatedFilePath);

      // Offset
      var offsetter = new SubtitleOffsetter();
      var transformed = offsetter.Transform(nativeSubtitle, translatedSubtitle);
      //offsetter.PrintHistory();

      // Map
      var mapping = new SubtitleMapping(transformed, translatedSubtitle);
      mapping.Filter(i => i.Error < 2.5);
      //mapping.Print();
      //mapping.PrintMismatches();

      var conversations = new SubtitleConversationAggregator().Aggregate(mapping);
      //conversations.Print();
      mapping.PrintErrorHistogram();

      return new ProcessedTranslation
      {
        Conversations = conversations
      };
    }
  }
}

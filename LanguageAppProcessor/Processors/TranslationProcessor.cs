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
    static SubtitleParser GetParserFromExtension(string extension)
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
    public static string GetExtension(string fileName)
    {
      return fileName.Split('.').Last();
    }

    // Hyperparameters
    public double MaxErrorAllowed { get; set; }
    public double ConversationSizeMinimum { get; set; }
    public double SilenceMinimum { get; set; }
    public TranslationProcessor(double maxErrorAllowed = 3, int conversationSizeMinimum = 3, double silenceMinimum = 1.5)
    {
      MaxErrorAllowed = maxErrorAllowed;
      ConversationSizeMinimum = conversationSizeMinimum;
      SilenceMinimum = silenceMinimum;
    }
    public ProcessedTranslation Process(string nativeFilePath, string translatedFilePath)
    {
      var nativeSubtitle = GetParserFromExtension(GetExtension(nativeFilePath))
        .Parse(nativeFilePath);
      var translatedSubtitle = GetParserFromExtension(GetExtension(translatedFilePath))
        .Parse(translatedFilePath);

      // Offset
      var transformed = new SubtitleOffsetter()
        .Transform(nativeSubtitle, translatedSubtitle);
      //offsetter.PrintHistory();

      // Map
      var mapping = new SubtitleMapping(transformed, translatedSubtitle);
      mapping.Filter(i => i.Error < MaxErrorAllowed);
      //mapping.Print();
      //mapping.PrintMismatches();

      var conversations = new SubtitleConversationAggregator(SilenceMinimum)
        .Aggregate(mapping);
      conversations.Filter(i => i.Intervals.Count >= ConversationSizeMinimum);
      //conversations.Print();
      mapping.PrintErrorHistogram();

      return new ProcessedTranslation
      {
        Conversations = conversations
      };
    }
  }
}

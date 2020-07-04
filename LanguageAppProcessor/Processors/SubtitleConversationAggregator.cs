using LanguageAppProcessor.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageAppProcessor
{
  public class SubtitleConversationAggregator
  {
    public SubtitleConversations Aggregate(SubtitleMapping mapping)
    {
      double silenceMinimum = 2.5;
      double silenceStart = 0;

      List<SubtitleConversation> conversations = new List<SubtitleConversation>();
      SubtitleConversation currentConversation = new SubtitleConversation();
      foreach (var interval in mapping.Intervals)
      {
        if (interval.Input.TimeFrame.Start.TotalSeconds - silenceStart >= silenceMinimum) // Ended conversation
        {
          if (currentConversation.Intervals.Any())
            conversations.Add(currentConversation);
          currentConversation = new SubtitleConversation();
        }
        currentConversation.Add(interval);
        silenceStart = interval.Input.TimeFrame.End.TotalSeconds;
      }
      if (currentConversation.Intervals.Any())
        conversations.Add(currentConversation);
      return new SubtitleConversations
      {
        Source = mapping.Source,
        Conversations = conversations,
      };
    }
  }
}

using LanguageAppProcessor.DTOs;
using LanguageAppProcessor.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageAppProcessor
{
  public class SubtitleMappingAggregator : IPipelineProcessor<SubtitleMapping, SubtitleConversations>
  {
    public double SilenceMinimum { get; set; }
    public event Action<SubtitleMapping> Started;
    public event Action<SubtitleMapping, SubtitleConversations> Finished;
    private Func<SubtitleConversation, bool> ConversationFilter { get; set; }
    public SubtitleMappingAggregator(double silenceMinimum = 1.5)
    {
      SilenceMinimum = silenceMinimum;
    }

    public SubtitleMappingAggregator WithFilter(Func<SubtitleConversation, bool> conversationFilter)
    {
      ConversationFilter = conversationFilter;
      return this;
    }

    public SubtitleConversations Aggregate(SubtitleMapping mapping)
    {
      double silenceStart = 0;

      List<SubtitleConversation> conversations = new List<SubtitleConversation>();
      SubtitleConversation currentConversation = new SubtitleConversation();
      foreach (var interval in mapping.Intervals)
      {
        if (interval.Input.TimeFrame.Start.TotalSeconds - silenceStart >= SilenceMinimum) // Ended conversation
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
        MovieName = mapping.MovieName,
        Conversations = conversations,
      };
    }

    public SubtitleConversations Process(SubtitleMapping input)
    {
      Started?.Invoke(input);
      var output = Aggregate(input);
      if (ConversationFilter != null)
      {
        output.Conversations = output.Conversations.Where(ConversationFilter).ToList();
      }
      Finished?.Invoke(input, output);
      return output;
    }
  }
}

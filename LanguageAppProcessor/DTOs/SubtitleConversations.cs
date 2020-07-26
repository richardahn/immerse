using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageAppProcessor.DTOs
{
  public class SubtitleConversations
  {
    public string MovieName { get; set; }
    public List<SubtitleConversation> Conversations { get; set; }

    public void Print(int limit = 3)
    {
      int i = 0;
      foreach (var conversation in Conversations)
      {
        if (i == limit) break;
        Console.WriteLine("-- Conversation --");
        foreach (var interval in conversation.Intervals)
        {
          Console.WriteLine($"[{interval.Input.TimeFrame.Start}, {interval.Error:0.00}] {interval.Input.JoinedLines} --> {interval.Target.JoinedLines}");
        }
        i++;
      }
    }

    public void PrintAverageError()
    {
      Console.WriteLine($"Average error = {Conversations.Average(c => c.Intervals.Average(i => i.Error))} for {Conversations.Count} conversations");
    }
  }
}

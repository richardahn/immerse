using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageAppProcessor.DTOs
{
  public class SubtitleConversations
  {
    public string Source { get; set; }
    public List<SubtitleConversation> Conversations { get; set; }

    public void Filter(Func<SubtitleConversation, bool> condition)
    {
      Conversations = Conversations.Where(condition).ToList();
    }
    public void Print()
    {
      foreach (var conversation in Conversations)
      {
        Console.WriteLine("-- Conversation --");
        foreach (var interval in conversation.Intervals)
        {
          Console.WriteLine($"[{interval.Input.TimeFrame.Start}] {interval.Input.JoinedLines} --> {interval.Target.JoinedLines}");
        }
      }
    }
  }
}

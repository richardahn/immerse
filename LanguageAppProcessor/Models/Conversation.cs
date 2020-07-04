using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor.Models
{
  public class Conversation
  {
    public int ID { get; set; }
    public string Source { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }

    public List<ConversationLine> Lines { get; set; } = new List<ConversationLine>();
  }
}

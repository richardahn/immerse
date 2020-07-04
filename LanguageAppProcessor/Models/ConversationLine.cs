using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor.Models
{
  public class ConversationLine
  {
    public int ID { get; set; }
    public int Index { get; set; }
    public string NativeText { get; set; }
    public string TranslatedText { get; set; }
    public double Error { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public int ConversationID { get; set; }
    public Conversation Conversation { get; set; }
  }
}

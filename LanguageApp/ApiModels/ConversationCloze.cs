using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageApp.ApiModels
{
  public class ClozeText
  {
    public bool Cloze { get; set; }
    public string Text { get; set; }
  }
  public class ConversationClozeLine
  {
    public List<ClozeText> NativeWords { get; set; }
    public List<ClozeText> TranslatedWords { get; set; }
  }
  public class ConversationCloze
  {
    public string Source { get; set; }
    public List<ConversationClozeLine> Lines { get; set; }
  }
}

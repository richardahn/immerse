using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageApp.ApiModels
{
  public class QuizBlock : ICloneable
  {
    public string Text { get; set; }
    public object Clone()
    {
      return new QuizBlock
      {
        Text = Text
      };
    }
  }
  public abstract class QuizProblem
  {
    public abstract string Type { get; }
    public string MovieName { get; set; }
    public int ConversationId { get; set; }
  }
  public class ShuffleLine
  {
    public List<QuizBlock> NativeBlocks { get; set; }
    public List<QuizBlock> ShuffledTranslatedBlocks { get; set; }
    public List<QuizBlock> CorrectTranslatedBlocks { get; set; }
  }
  public class ShuffleProblem : QuizProblem
  {
    public override string Type => "shuffle";
    public List<ShuffleLine> Lines { get; set; }
  }
}

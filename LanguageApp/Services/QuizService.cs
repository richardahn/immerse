using LanguageApp.ApiModels;
using LanguageAppProcessor;
using LanguageAppProcessor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageApp.Services
{
  public class QuizService
  {
    private readonly TranslationContext _context;
    public QuizService(TranslationContext context)
    {
      _context = context;
    }
    public List<QuizBlock> ShuffleBlocks(List<QuizBlock> blocksOriginal) // Not in-place
    {
      Random rand = new Random();
      var blocks = blocksOriginal.Select(b => (QuizBlock)b.Clone()).ToList();
      int n = blocks.Count;
      while (n > 1)
      {
        int k = rand.Next(n--); // get some item to swap with
        QuizBlock t = blocks[k];
        blocks[k] = blocks[n];
        blocks[n] = t;
      }
      return blocks;
    }

    private Conversation GetRandomConversation()
    {
      return _context.Conversations
        .OrderBy(c => Guid.NewGuid())
        .Include(c => c.Lines)
        .Take(1)
        .First();
    }
    public QuizProblem GenerateProblem(string type, int? id)
    {
      Conversation conversation = id.HasValue ?
        _context.Conversations.Include(c => c.Lines).Single(c => c.ID == id) :
        GetRandomConversation();
      switch (type)
      {
        case "shuffle":
          return new ShuffleProblem
          {
            ConversationId = conversation.ID,
            MovieName = conversation.Source,
            Lines = conversation.Lines.Select(line => 
            {
              var translatedBlocks = line.TranslatedText.Split(' ').Select(text => new QuizBlock
              {
                Text = text
              }).ToList();
              return new ShuffleLine
              {
                NativeBlocks = line.NativeText.Split(' ').Select(text => new QuizBlock
                {
                  Text = text,
                }).ToList(),
                ShuffledTranslatedBlocks = ShuffleBlocks(translatedBlocks),
                CorrectTranslatedBlocks = translatedBlocks,
              };
            }).ToList(),
          };
        default:
          return null;
      }
    }
  }
}

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
  public class ConversationClozeService
  {
    private readonly TranslationContext _context;
    private readonly Random random = new Random();
    public ConversationClozeService(TranslationContext context)
    {
      _context = context;
    }
    public ConversationCloze GenerateRandom(int clozesPerLine, bool clozeTranslations = true)
    {
      var conversations = _context.Conversations
        .Include(c => c.Lines)
        .AsNoTracking()
        .ToList();
      var randomIndex = random.Next(conversations.Count());
      var randomConversation = conversations[randomIndex];
      return Convert(randomConversation, clozeTranslations, clozesPerLine);
    }

    private static List<ClozeText> GenerateCloze(string text, bool cloze, int clozesPerLine)
    {
      var words = text.Split(' ');
      var clozes = words.Select(word => new ClozeText
      {
        Cloze = false,
        Text = word,
      }).ToList();

      if (!cloze)
      {
        return clozes;
      }

      var randomQueue = RandomWithoutReplacement(words.Length, clozesPerLine);
      while (randomQueue.Any())
      {
        clozes[randomQueue.Dequeue()].Cloze = true;
      }
      return clozes;
    }
    private static Queue<int> RandomWithoutReplacement(int N, int limit)
    {
      limit = Math.Min(N, limit);
      int[] randomIndices = new int[N];
      for (int i = 0; i < N; i++)
      {
        randomIndices[i] = i;
      }
      var rand = new Random();
      var result = new Queue<int>(limit);
      for (int i = N - 1; i >= N - limit; i--)
      {
        int j = rand.Next(i + 1);
        // Swap
        int t = randomIndices[i];
        randomIndices[i] = randomIndices[j];
        randomIndices[j] = t;
        result.Enqueue(randomIndices[i]);
      }
      return result;
    }
    private static ConversationCloze Convert(Conversation conversation, bool clozeTranslations, int clozesPerLine)
    {
      return new ConversationCloze
      {
        Source = conversation.Source,
        Lines = conversation.Lines.Select(line => new ConversationClozeLine
        {
          NativeWords = GenerateCloze(line.NativeText, !clozeTranslations, clozesPerLine),
          TranslatedWords = GenerateCloze(line.TranslatedText, clozeTranslations, clozesPerLine),
        }).ToList()
      };
    }
  }
}

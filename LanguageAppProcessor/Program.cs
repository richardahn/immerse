using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageAppProcessor
{
  class Program
  {

    static void ClearDatabase(TranslationContext context)
    {
      context.Database.ExecuteSqlCommand($"TRUNCATE TABLE [ConversationLines]");
      context.Conversations.RemoveRange(context.Conversations);
      context.SaveChanges();
    }
    static void Main(string[] args)
    {
      Console.OutputEncoding = Encoding.UTF8;
      string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
      var files = Directory.GetFiles(rootDir + @"\Data");

      var context = new TranslationContext();
      ClearDatabase(context);
      for (int i = 0; i < files.Length; i += 2)
      {
        string native;
        string translated;
        if (files[i].Contains("en"))
        {
          native = files[i];
          translated = files[i + 1];
        }
        else
        {
          native = files[i + 1];
          translated = files[i];
        }
        new TranslationProcessor().Process(native, translated).Save(context);
      }
    }
  }
}

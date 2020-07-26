using LanguageAppProcessor.DTOs;
using LanguageAppProcessor.Models;
using LanguageAppProcessor.Pipeline;
using LanguageAppProcessor.Processors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
    static void Save(TranslationContext context, SubtitleConversations conversationsRaw)
    {
      var conversationsModel = conversationsRaw.Conversations.Select(c => new Conversation
      {
        Source = conversationsRaw.MovieName,
        Lines = c.Intervals.Select(i => new ConversationLine
        {
          NativeText = i.Input.JoinedLines,
          TranslatedText = i.Target.JoinedLines,
          Start = i.Input.TimeFrame.Start,
          End = i.Input.TimeFrame.End,
          Error = i.Error,
          Index = i.Input.Index.GetValueOrDefault(),
        }).ToList()
      });
      context.Conversations.AddRange(conversationsModel);
      context.SaveChanges();
    }
    static void PrintHeader(string title)
    {
      Console.WriteLine($"************************************ {title} ******************************************");
    }
    static async Task Main(string[] args)
    {
      Console.OutputEncoding = Encoding.UTF8;
      string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
      var files = Directory.GetFiles(rootDir + @"\Data");
      var context = new TranslationContext();
      ClearDatabase(context);

      // -- Pipeline Components -- 
      var parser = new SubtitleParser();
      parser.Started += (_) => PrintHeader("Step 1 - Parser");
      //parser.Finished += (_, output) => output.Print();
      var offsetter = new SubtitleOffsetter();
      offsetter.Started += (_) => PrintHeader("Step 2 - Offsetter");
      //offsetter.Finished += (_, _) => offsetter.PrintHistory();
      var mapper = new SubtitleMapper();
      mapper.Started += (_) => PrintHeader("Step 3 - Mapper");
      mapper.Finished += (_, output) =>
      {
        //output.Print();
        //output.PrintErrorHistogram();
        //output.PrintMismatches();
      };
      var aggregator = new SubtitleMappingAggregator();
      const double errorThreshold = 1.0;
      aggregator.WithFilter((conversation) => conversation.Intervals.Count < 5 && conversation.Intervals.Average(i => i.Error) < errorThreshold);
      aggregator.Started += (_) => PrintHeader("Step 4 - Aggregation");
      aggregator.Finished += (_, output) => {
        //output.Print(); 
        output.PrintAverageError();
        Save(new TranslationContext(), output); // Create a new context in order to have thread safety
      };

      // -- Pipeline -- 
      var pipeline = new Pipeline<SubtitleFilePathPair, SubtitleConversations>()
        .AddStepAndStart(parser)
        .AddStepAndStart(offsetter)
        .AddStepAndStart(mapper)
        .AddStepAndStart(aggregator);

      List<Task> outputTasks = new List<Task>();
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
        var input = new SubtitleFilePathPair
        {
          Native = native,
          Translated = translated,
        };
        outputTasks.Add(pipeline.Execute(input));
      }
      await Task.WhenAll(outputTasks);
      pipeline.Stop(); 
    }
  }
}

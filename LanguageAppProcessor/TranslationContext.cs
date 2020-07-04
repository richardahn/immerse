using LanguageAppProcessor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageAppProcessor
{
  public class TranslationContext : DbContext
  {
    public DbSet<ConversationLine> ConversationLines { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
      => optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;");
  }
}

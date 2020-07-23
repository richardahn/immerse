using LanguageApp.Services;
using LanguageAppProcessor;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageApp.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  public class LanguageController : Controller
  {
    private readonly ConversationClozeService _conversationClozeService;
    public LanguageController(ConversationClozeService conversationClozeService)
    {
      _conversationClozeService = conversationClozeService;
    }

    // GetRandomConversation /language/random
    [HttpGet("random")]
    public ActionResult RandomConversationCloze(int clozesPerLine)
    {
      return Json(_conversationClozeService.GenerateRandom(clozesPerLine));
    }
  }
}

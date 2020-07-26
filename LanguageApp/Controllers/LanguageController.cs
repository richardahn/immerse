using LanguageApp.Services;
using LanguageAppProcessor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly TranslationContext _context;
    private readonly QuizService _quizService;
    public LanguageController(TranslationContext context, QuizService quizService)
    {
      _context = context;
      _quizService = quizService;
    }

    [HttpGet("problem/{type}/{id?}")]
    public ActionResult GetProblem(string type, int? id)
    {
      return Json(_quizService.GenerateProblem(type, id));
    }
  }
}

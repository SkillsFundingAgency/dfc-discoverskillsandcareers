using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class QuestionController : Controller
    {
        [Route("q")]
        public IActionResult Index()
        {
            return View("Question");
        }

        [Route("q/{questionNumber:int}")]
        public IActionResult AtQuestionNumber(int questionNumber, string assessmentType)
        {
            var model = new QuestionViewModel()
            {
                // TODO:
            };
            return View("Question", model);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.Services;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class QuestionController : Controller
    {
        readonly IUserSessionService UserSessionService;
        readonly IQuestionRepository QuestionRepository;

        public QuestionController(IUserSessionService userSessionService, IQuestionRepository questionRepository)
        {
            UserSessionService = userSessionService;
            QuestionRepository = questionRepository;
        }

        [Route("q")]
        public IActionResult Index()
        {
            return View("Question");
        }

        [Route("q/{questionNumber:int}")]
        public IActionResult AtQuestionNumber(int questionNumber, string assessmentType)
        {
            var resp = UserSessionService.CreateSession("test", 4);
            var model = new QuestionViewModel()
            {
                // TODO:
            };
            return View("Question", model);
        }
    }
}
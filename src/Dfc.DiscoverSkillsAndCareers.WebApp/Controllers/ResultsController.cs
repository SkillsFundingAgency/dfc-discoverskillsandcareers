﻿using Dfc.DiscoverSkillsAndCareers.Services;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("results")]
    public class ResultsController : Controller
    {
        readonly IUserSessionService UserSessionService;
        readonly IResultsService ResultsService;

        public ResultsController(IUserSessionService userSessionService, IResultsService resultsService)
        {
            UserSessionService = userSessionService;
            ResultsService = resultsService;
        }

        public async Task<IActionResult> Index()
        {
            await UserSessionService.Init(Request);

            switch (UserSessionService.Session.AssessmentType)
            {
                case "short":
                    {
                        // Calcualte and save the result of the short assessment
                        await ResultsService.CalculateShortAssessment(UserSessionService.Session);
                        await UserSessionService.UpdateSession();
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException("Assessment type result not implemented");
                    }
            }

            var traits = UserSessionService.Session.ResultData.Traits;
            int traitsTake = (traits.Count > 3 && traits[2].TotalScore == traits[3].TotalScore) ? 4 : 3;
            var jobFamilies = UserSessionService.Session.ResultData.JobFamilies;
            var model = new ResultsViewModel()
            {
                SessionId = UserSessionService.Session.UserSessionId,
                JobFamilyCount = UserSessionService.Session.ResultData.JobFamilies.Count,
                JobFamilyMoreCount = UserSessionService.Session.ResultData.JobFamilies.Count - 3,
                Traits = traits.Take(traitsTake).Select(x => x.TraitText).ToList(),
                JobFamilies = jobFamilies
            };
            return View("Results", model);
        }
    }
}
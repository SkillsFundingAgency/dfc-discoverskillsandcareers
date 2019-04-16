﻿using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public class HomeController : BaseController
    {
        readonly ILogger<HomeController> Log;
        readonly ILoggerHelper LoggerHelper;
        readonly IApiServices ApiServices;

        public HomeController(
            ILogger<HomeController> log,
            ILoggerHelper loggerHelper,
            IApiServices apiServices)
        {
            Log = log;
            LoggerHelper = loggerHelper;
            ApiServices = apiServices;
        }

        public async Task<IActionResult> Index(string e = "")
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);
                var model = await ApiServices.GetContentModel<IndexViewModel>("indexpage", correlationId);
                model.SessionId = sessionId;
                model.HasReloadError = !string.IsNullOrEmpty(e);
                if (e == "1")
                {
                    model.ResumeErrorMessage = model.MissingCodeErrorMessage;
                }
                if (string.IsNullOrEmpty(sessionId) == false)
                {
                    AppendCookie(sessionId);
                }
                return View("Index", model);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(Log, correlationId, ex);
                return StatusCode(500);
            }
            finally
            {
                LoggerHelper.LogMethodExit(Log);
            }
        }

        public class ReloadRequest
        {
            public string Code { get; set; }
        }

        [HttpGet]
        [Route("reload")]
        public async Task<IActionResult> ReloadGet()
        {
            var sessionId = await TryGetSessionId(Request);
            if (!string.IsNullOrEmpty(sessionId))
            {
                return await Reload(new ReloadRequest { Code = sessionId });
            }
            return Redirect("/");
        }

        [HttpPost]
        [Route("reload")]
        public async Task<IActionResult> Reload([FromForm] ReloadRequest reloadRequest)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                if (string.IsNullOrEmpty(reloadRequest?.Code))
                {
                    var model = await ApiServices.GetContentModel<IndexViewModel>("indexpage", correlationId);
                    return Redirect("/?e=1");
                }

                reloadRequest.Code = reloadRequest.Code.Replace(" ", "").ToLower();
                if (reloadRequest.Code != HttpUtility.UrlEncode(reloadRequest.Code))
                {
                    var model = await ApiServices.GetContentModel<IndexViewModel>("indexpage", correlationId);
                    return Redirect("/?e=2");
                }

                if (reloadRequest.Code == "throw500") // TODO: for testing
                {
                    throw new Exception("Test 500 exception!");
                }

                var nextQuestionResponse = await ApiServices.NextQuestion(reloadRequest.Code, correlationId);
                if (nextQuestionResponse == null)
                {
                    var model = await ApiServices.GetContentModel<IndexViewModel>("indexpage", correlationId);
                    return Redirect("/?e=2");
                }

                AppendCookie(nextQuestionResponse.SessionId);

                if (nextQuestionResponse.IsComplete)
                {
                    // Session has complete, redirect to results
                    RedirectResult redirectResult;
                    if (nextQuestionResponse.IsFilterAssessment)
                    {
                        redirectResult = new RedirectResult($"/results/{nextQuestionResponse.JobCategorySafeUrl}");
                    }
                    else
                    {
                        redirectResult = new RedirectResult($"/results");
                    }
                    return redirectResult;
                }

                if (nextQuestionResponse.IsFilterAssessment)
                {
                    // Filter assessment is in progress
                    var redirectResponse = new RedirectResult($"/qf/{nextQuestionResponse.QuestionNumber}");
                    return redirectResponse;
                }
                else
                {
                    // Session is not complete so continue where we was last
                    var redirectResponse = new RedirectResult($"/q/{nextQuestionResponse.QuestionNumber}");
                    return redirectResponse;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(Log, correlationId, ex);
                return StatusCode(500);
            }
            finally
            {
                LoggerHelper.LogMethodExit(Log);
            }
        }
    }
}

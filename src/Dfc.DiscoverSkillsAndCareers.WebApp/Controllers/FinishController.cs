
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("finish")]
    public class FinishController : BaseController
    {
        readonly ILogger<FinishController> Log;
        readonly ILoggerHelper LoggerHelper;
        readonly IApiServices ApiServices;

        public FinishController(
            ILogger<FinishController> log,
            ILoggerHelper loggerHelper,
            IApiServices apiServices)
        {
            Log = log;
            LoggerHelper = loggerHelper;
            ApiServices = apiServices;
        }

        [Route("{jobCategory}")]
        public async Task<IActionResult> FinishWithJobCategory(string jobCategory)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var viewName ="FinishFilteredAssessment";
                var contentName = "finishjobcategorypage";
                var model = await ApiServices.GetContentModel<FinishViewModel>(contentName, correlationId);
                model.JobCategorySafeUrl = jobCategory;
                Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true, HttpOnly = true });
                return View(viewName, model);
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

        public async Task<IActionResult> Index()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);
                
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var viewName = "Finish";
                var contentName = "finishpage";
                var model = await ApiServices.GetContentModel<FinishViewModel>(contentName, correlationId);
                Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true, HttpOnly = true });
                return View(viewName, model);
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
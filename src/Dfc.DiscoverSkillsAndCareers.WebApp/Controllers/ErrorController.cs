using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public interface IErrorController
    {
        Task<IActionResult> Error404();
        Task<IActionResult> Error500();
    }

    public class ErrorController : BaseController, IErrorController
    {
        readonly ILogger<ErrorController> Log;
        readonly ILoggerHelper LoggerHelper;
        readonly IApiServices ApiServices;

        public ErrorController(
            ILogger<ErrorController> log,
            ILoggerHelper loggerHelper,
            IApiServices apiServices)
        {
            Log = log;
            LoggerHelper = loggerHelper;
            ApiServices = apiServices;
        }

        [HttpGet("error/404")]
        public async Task<IActionResult> Error404()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);
                var model = await ApiServices.GetContentModel<IndexViewModel>("indexpage", correlationId);
                model.SessionId = sessionId;
                if (string.IsNullOrEmpty(sessionId) == false)
                {
                    AppendCookie(sessionId);
                }
                return View("404", model);
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

        [HttpGet("error/500")]
        public async Task<IActionResult> Error500()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);
                var model = await ApiServices.GetContentModel<IndexViewModel>("indexpage", correlationId);
                model.SessionId = sessionId;
                if (string.IsNullOrEmpty(sessionId) == false)
                {
                    AppendCookie(sessionId);
                }
                return View("500", model);
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
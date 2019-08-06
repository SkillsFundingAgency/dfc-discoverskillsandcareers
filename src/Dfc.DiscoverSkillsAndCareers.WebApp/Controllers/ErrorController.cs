using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    public interface IErrorController
    {
        Task<IActionResult> Error404();
        Task<IActionResult> Error500();
    }

    public class ErrorController : BaseController, IErrorController
    {
        readonly ILogger<ErrorController> _log;
        readonly IApiServices _apiServices;

        public ErrorController(
            ILogger<ErrorController> log,
            IApiServices apiServices, IDataProtectionProvider dataProtectionProvider) : base(dataProtectionProvider)
        {
            _log = log;
            _apiServices = apiServices;
        }

        [HttpGet("error/404")]
        public async Task<IActionResult> Error404()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId) == false)
                {
                    AppendCookie(sessionId);
                }
                var result = View("404", new IndexViewModel
                {
                    SessionId = sessionId
                });

                result.StatusCode = 404;
                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Error404)}");
                return RedirectToAction("Error500");
            }
        }

        [HttpGet("error/500")]
        public async Task<IActionResult> Error500()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId) == false)
                {
                    AppendCookie(sessionId);
                }
                var result = View("500", new IndexViewModel
                {
                    SessionId = sessionId
                });

                result.StatusCode = 500;
                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Error500)}");
                return StatusCode(500);
            }
        }
    }
}
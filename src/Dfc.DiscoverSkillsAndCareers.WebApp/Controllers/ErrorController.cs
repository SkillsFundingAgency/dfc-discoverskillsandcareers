using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.DataProtection;
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
        private readonly ILogger<ErrorController> _log;
        private readonly ILayoutService _layoutService;

        public ErrorController(ILogger<ErrorController> log, IDataProtectionProvider dataProtectionProvider, ILayoutService layoutService) : base(dataProtectionProvider)
        {
            _log = log;
            _layoutService = layoutService;
        }

        [HttpGet("error/404")]
        public async Task<IActionResult> Error404()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId();
                if (string.IsNullOrEmpty(sessionId) == false)
                {
                    AppendCookie(sessionId);
                }
                var result = View("404", new IndexViewModel
                {
                    SessionId = sessionId,
                    Layout = _layoutService.GetLayout(Request)
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
                var sessionId = await TryGetSessionId();
                if (string.IsNullOrEmpty(sessionId) == false)
                {
                    AppendCookie(sessionId);
                }
                var result = View("500", new IndexViewModel
                {
                    SessionId = sessionId,
                    Layout = _layoutService.GetLayout(Request)
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
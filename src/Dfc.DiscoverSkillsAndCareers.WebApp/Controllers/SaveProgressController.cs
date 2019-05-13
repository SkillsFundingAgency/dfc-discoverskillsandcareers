using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Controllers
{
    [Route("save-my-progress")]
    public class SaveProgressController : BaseController
    {
        readonly ILogger<SaveProgressController> _log;
        readonly IApiServices _apiServices;
        readonly AppSettings _appSettings;

        public SaveProgressController(
            ILogger<SaveProgressController> log,
            IApiServices apiServices,
            IOptions<AppSettings> appSettings)
        {
            _log = log;
            _apiServices = apiServices;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> SaveProgressOption([FromForm]SaveProgressOptionRequest saveProgressOptionRequest)
        {
            switch (saveProgressOptionRequest.SelectedOption)
            {
                case "email":
                    {
                        return RedirectToAction("EmailInput");
                    }
                case "sms":
                    {
                        return RedirectToAction("SmsInput");
                    }
                case "reference":
                    {
                        return RedirectToAction("ReferenceNumber");
                    }
                default:
                    {
                        return await Index(true);
                    }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool withError = false)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);

                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var model = new SaveProgressViewModel();
                model.BackLink = "/reload";
                if (withError)
                {
                    model.ErrorMessage = "Please select an option to continue";
                }
                AppendCookie(sessionId);
                return View("SaveProgress", model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Index)}");
                return StatusCode(500);
            }
        }

        [HttpGet("email", Name = "SaveProgressEmailInput")]
        public async Task<IActionResult> EmailInput()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);

                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var model = new SaveProgressViewModel();
                model.BackLink = "/save-my-progress";
                AppendCookie(sessionId);
                return View("EmailInput", model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(EmailInput)}");
                return StatusCode(500);
            }
        }

        [HttpPost("email")]
        public async Task<IActionResult> SendEmail([FromForm]SendEmailRequest sendEmailRequest)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }
                var model = new SaveProgressViewModel();
                model.BackLink = "/save-my-progress";
                if (string.IsNullOrEmpty(sendEmailRequest.Email?.Trim()))
                {
                    model.ErrorMessage = "You must enter an email address";
                    return View("EmailInput", model);
                }
                NotifyResponse notifyResponse = null;
                try
                {
                    notifyResponse = await _apiServices.SendEmail($"https://{Request.Host.Value}", sendEmailRequest.Email?.ToLower(), _appSettings.NotifyEmailTemplateId, sessionId, correlationId);
                    if (!notifyResponse.IsSuccess)
                    {
                        throw new Exception(notifyResponse?.Message);
                    }
                    model.SentTo = sendEmailRequest.Email?.ToLower();
                    AppendCookie(sessionId);
                    return View("EmailSent", model);

                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"Correlation Id: {correlationId} - Sending email in action {nameof(SendEmail)}");
                    model.ErrorMessage = "Enter a valid email address";
                    return View("EmailInput", model);
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(SendEmail)}");
                return StatusCode(500);
            }
        }

        [HttpGet("sms", Name = "SaveProgressSmsInput")]
        public async Task<IActionResult> SmsInput()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);

                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var model = new SaveProgressViewModel();
                model.BackLink = "/save-my-progress";
                AppendCookie(sessionId);
                return View("SmsInput", model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(SmsInput)}");
                return StatusCode(500);
            }
        }

        [NonAction]
        public static string GetDisplayCode(string code)
        {
            string result = "";
            int i = 0;
            foreach(var c in code.ToUpper().ToCharArray())
            {
                i++;
                if (i % 4 == 1 && i > 1)
                {
                    result += " ";
                }
                result += c.ToString();
            }
            return result;
        }

        [HttpGet("reference", Name = "SaveProgressReference")]
        public async Task<IActionResult> ReferenceNumber()
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);

                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var model = new SaveProgressViewModel();
                model.BackLink = "/save-my-progress";
                await UpdateSessionVarsOnViewModel(model, sessionId, correlationId);
                AppendCookie(sessionId);
                return View("ReferenceNumber", model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(ReferenceNumber)}");
                return StatusCode(500);
            }
        }

        [NonAction]
        public async Task UpdateSessionVarsOnViewModel(SaveProgressViewModel model, string sessionId, Guid correlationId)
        {
            var nextQuestionResponse = await _apiServices.Reload(sessionId, correlationId);
            model.SessionId = sessionId;
            model.Code = GetDisplayCode(nextQuestionResponse.ReloadCode);
            model.SessionDate = nextQuestionResponse.StartedDt.ToString("dd MMMM yyyy");
            model.Status = $"{nextQuestionResponse.RecordedAnswersCount} out of {nextQuestionResponse.MaxQuestionsCount} statements complete";
        }

        [HttpPost("reference")]
        public async Task<IActionResult> SendSms([FromForm]SendSmsRequest sendSmsRequest)
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }
                var model = new SaveProgressViewModel();
                await UpdateSessionVarsOnViewModel(model, sessionId, correlationId);
                model.BackLink = "/save-my-progress";
                if (string.IsNullOrEmpty(sendSmsRequest.MobileNumber?.Trim()) || !System.Text.RegularExpressions.Regex.Match(sendSmsRequest.MobileNumber, @"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$").Success)
                {
                    model.ErrorMessage = "Enter a phone number";
                    return View("ReferenceNumber", model);
                }
                NotifyResponse notifyResponse = null;
                try
                {
                    notifyResponse = await _apiServices.SendSms($"https://{Request.Host.Value}", sendSmsRequest.MobileNumber, _appSettings.NotifySmsTemplateId, sessionId, correlationId);
                    if (!notifyResponse.IsSuccess)
                    {
                        throw new Exception(notifyResponse?.Message);
                    }
                    model.SentTo = sendSmsRequest.MobileNumber;
                    AppendCookie(sessionId);
                    return View("SmsSent", model);

                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred sending an SMS in action {nameof(SendSms)}");
                    model.ErrorMessage = "Enter a valid phone number";
                    return View("ReferenceNumber", model);
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(SendSms)}");
                return StatusCode(500);
            }
        }
    }
}

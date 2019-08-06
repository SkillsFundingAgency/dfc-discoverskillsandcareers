using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            IOptions<AppSettings> appSettings, IDataProtectionProvider dataProtectionProvider) : base(dataProtectionProvider)
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
                    model.ErrorMessage = "Choose how you would like to return to your assessment";
                }
                AppendCookie(sessionId);
                return View("SaveProgress", model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(Index)}");
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet("EmailSent")]
        public IActionResult EmailSent()
        {
            
            var email = (string)TempData["SentEmail"];

            if (!String.IsNullOrWhiteSpace(email))
            {
                TempData["SentEmail"] = null;
                return View("EmailSent", new SaveProgressViewModel() {SentTo = email});
            }
            
            return RedirectToAction("Index");
        }
        
        [HttpGet("SmsSent")]
        public IActionResult SmsSent()
        {
            var sms = (string)TempData["SentSms"];

            if (!String.IsNullOrWhiteSpace(sms))
            {
                TempData["SentSms"] = null;
                return View("SmsSent", new SaveProgressViewModel() {SentTo = sms});
            }

            return RedirectToAction("Index");
        }

        [HttpGet("email", Name = "SaveProgressEmailInput")]
        public async Task<IActionResult> EmailInput(string e = "")
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
                if (e == "1")
                {
                    model.ErrorMessage = "Enter an email address";
                }
                else if (e == "2")
                {
                    model.ErrorMessage = "Enter an email address in the correct format, like name@example.com";
                }
                else if (e == "3")
                {
                    _log.LogError($"Correlation Id: {correlationId} - Unable to send email");
                    return RedirectToAction("Error500", "Error");
                }
                
                return View("EmailInput", model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(EmailInput)}");
                return RedirectToAction("Error500", "Error");
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

                var model = new SaveProgressViewModel {BackLink = "/save-my-progress"};

                if (!sendEmailRequest.ValidEmail)
                {
                    if (string.IsNullOrWhiteSpace(sendEmailRequest.Email))
                    {
                        return Redirect("/save-my-progress/email?e=1");
                    }

                    return Redirect("/save-my-progress/email?e=2");
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

                    TempData["SentEmail"] = model.SentTo;
                    return RedirectToAction("EmailSent");
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"Correlation Id: {correlationId} - Sending email in action {nameof(SendEmail)}");
                    return Redirect("/save-my-progress/email?e=3");
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(SendEmail)}");
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpGet("sms", Name = "SaveProgressSmsInput")]
        public async Task<IActionResult> SmsInput(string e = "")
        {
            var correlationId = Guid.NewGuid();
            try
            {
                var sessionId = await TryGetSessionId(Request);

                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var model = new SaveProgressViewModel {BackLink = "/save-my-progress"};
                if (e == "1")
                {
                    model.ErrorMessage = "Enter a phone number";
                }
                else if (e == "2")
                {
                    model.ErrorMessage = "Enter a mobile phone number, like 07700 900 982.";
                }
                else if (e == "3")
                {
                    _log.LogError($"Correlation Id: {correlationId} - Unable to send SMS");
                    return RedirectToAction("Error500", "Error");
                }
                AppendCookie(sessionId);
                return View("SmsInput", model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(SmsInput)}");
                return RedirectToAction("Error500", "Error");
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
        public async Task<IActionResult> ReferenceNumber(string e = "")
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
                if (e == "1")
                {
                    model.ErrorMessage = "Enter a phone number";
                }
                else if (e == "2")
                {
                    model.ErrorMessage = "Enter a mobile phone number, like 07700 900 982.";
                }
                else if (e == "3")
                {
                    _log.LogError($"Correlation Id: {correlationId} - Unable to send SMS");
                    return RedirectToAction("Error500", "Error");
                }
                
                await UpdateSessionVarsOnViewModel(model, sessionId, correlationId);
                AppendCookie(sessionId);
                return View("ReferenceNumber", model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(ReferenceNumber)}");
                return RedirectToAction("Error500", "Error");
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
                    
                if (!sendSmsRequest.ValidMobileNumber)
                {
                    if (String.IsNullOrWhiteSpace(sendSmsRequest.MobileNumber))
                    {
                        return Redirect("/save-my-progress/reference?e=1");
                    }
                    
                    return Redirect("/save-my-progress/reference?e=2");
                }
                
                var model = new SaveProgressViewModel {BackLink = "/save-my-progress"};
                await UpdateSessionVarsOnViewModel(model, sessionId, correlationId);
                
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

                    TempData["SentSms"] = model.SentTo;
                    return RedirectToAction("SmsSent");

                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred sending an SMS in action {nameof(SendSms)}");
                    return Redirect("/save-my-progress/reference?e=3");
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Correlation Id: {correlationId} - An error occurred rendering action {nameof(SendSms)}");
                return RedirectToAction("Error500", "Error");
            }
        }
    }
}

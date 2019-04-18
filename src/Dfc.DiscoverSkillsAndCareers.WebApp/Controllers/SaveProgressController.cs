using Dfc.DiscoverSkillsAndCareers.WebApp.Config;
using Dfc.DiscoverSkillsAndCareers.WebApp.Models;
using Dfc.DiscoverSkillsAndCareers.WebApp.Services;
using DFC.Common.Standard.Logging;
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
        readonly ILogger<SaveProgressController> Log;
        readonly ILoggerHelper LoggerHelper;
        readonly IApiServices ApiServices;
        readonly AppSettings AppSettings;

        public SaveProgressController(
            ILogger<SaveProgressController> log,
            ILoggerHelper loggerHelper,
            IApiServices apiServices,
            IOptions<AppSettings> appSettings)
        {
            Log = log;
            LoggerHelper = loggerHelper;
            ApiServices = apiServices;
            AppSettings = appSettings.Value;
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
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);

                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var model = await ApiServices.GetContentModel<SaveProgressViewModel>("saveprogresspage", correlationId);
                model.BackLink = "/reload";
                if (withError)
                {
                    model.ErrorMessage = model.SaveProgressNoOptionSelectedMessage;
                }
                AppendCookie(sessionId);
                return View("SaveProgress", model);
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

        [HttpGet("email", Name = "SaveProgressEmailInput")]
        public async Task<IActionResult> EmailInput()
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

                var model = await ApiServices.GetContentModel<SaveProgressViewModel>("saveprogresspage", correlationId);
                model.BackLink = "/save-my-progress";
                AppendCookie(sessionId);
                return View("EmailInput", model);
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

        [HttpPost("email")]
        public async Task<IActionResult> SendEmail([FromForm]SendEmailRequest sendEmailRequest)
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
                var model = await ApiServices.GetContentModel<SaveProgressViewModel>("saveprogresspage", correlationId);
                if (string.IsNullOrEmpty(sendEmailRequest.Email?.Trim()))
                {
                    model.ErrorMessage = $"You must enter an email address";
                    return View("EmailInput", model);
                }
                model.BackLink = "/save-my-progress";
                NotifyResponse notifyResponse = null;
                try
                {
                    notifyResponse = await ApiServices.SendEmail($"https://{Request.Host.Value}", sendEmailRequest.Email?.ToLower(), AppSettings.NotifyEmailTemplateId, sessionId, correlationId);
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
                    LoggerHelper.LogException(Log, correlationId, ex);
                    model.ErrorMessage = $"An error occurred sending an email to {sendEmailRequest.Email}";
                    return View("EmailInput", model);
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

        [HttpGet("sms", Name = "SaveProgressSmsInput")]
        public async Task<IActionResult> SmsInput()
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

                var model = await ApiServices.GetContentModel<SaveProgressViewModel>("saveprogresspage", correlationId);
                model.BackLink = "/save-my-progress";
                AppendCookie(sessionId);
                return View("SmsInput", model);
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
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);

                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }

                var model = await ApiServices.GetContentModel<SaveProgressViewModel>("saveprogresspage", correlationId);
                model.BackLink = "/save-my-progress";
                await UpdateSessionVarsOnViewModel(model, sessionId, correlationId);
                AppendCookie(sessionId);
                return View("ReferenceNumber", model);
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

        [NonAction]
        public async Task UpdateSessionVarsOnViewModel(SaveProgressViewModel model, string sessionId, Guid correlationId)
        {
            var nextQuestionResponse = await ApiServices.NextQuestion(sessionId, correlationId);
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
                LoggerHelper.LogMethodEnter(Log);

                var sessionId = await TryGetSessionId(Request);
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Redirect("/");
                }
                var model = await ApiServices.GetContentModel<SaveProgressViewModel>("saveprogresspage", correlationId);
                await UpdateSessionVarsOnViewModel(model, sessionId, correlationId);
                if (string.IsNullOrEmpty(sendSmsRequest.MobileNumber?.Trim()) || !System.Text.RegularExpressions.Regex.Match(sendSmsRequest.MobileNumber, @"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$").Success)
                {
                    model.ErrorMessage = model.SmsInputInvalidMessage;
                    return View("ReferenceNumber", model);
                }
                model.BackLink = "/save-my-progress";
                NotifyResponse notifyResponse = null;
                try
                {
                    notifyResponse = await ApiServices.SendSms($"https://{Request.Host.Value}", sendSmsRequest.MobileNumber, AppSettings.NotifySmsTemplateId, sessionId, correlationId);
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
                    LoggerHelper.LogException(Log, correlationId, ex);
                    model.ErrorMessage = $"An error occurred sending a text to {sendSmsRequest.MobileNumber}";
                    return View("ReferenceNumber", model);
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

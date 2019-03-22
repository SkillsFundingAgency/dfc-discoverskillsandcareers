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

                var model = await ApiServices.GetContentModel<SaveProgressViewModel>("saveprogresspage", correlationId);

                var nextQuestionResponse = await ApiServices.NextQuestion(sessionId, correlationId);
                model.SessionId = sessionId;
                model.Code = nextQuestionResponse.ReloadCode;
                model.SessionDate = nextQuestionResponse.StartedDt.ToString("dd MMMM yyyy");
                model.Status = $"{nextQuestionResponse.RecordedAnswersCount} out of {nextQuestionResponse.MaxQuestionsCount} statements complete";
                Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true, HttpOnly = true });
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

        [HttpGet("email")]
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

                Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true, HttpOnly = true });
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
                NotifyResponse notifyResponse = null;
                try
                {
                    notifyResponse = await ApiServices.SendEmail($"https://{Request.Host.Value}", sendEmailRequest.Email, AppSettings.NotifyEmailTemplateId, sessionId, correlationId);

                    model.SentTo = sendEmailRequest.Email?.ToLower();
                    Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true, HttpOnly = true });
                    return View("EmailSent", model);

                }
                catch (Exception ex)
                {
                    model.ErrorMessage = ex.Message;
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

        [HttpGet("sms")]
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

                Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true, HttpOnly = true });
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

        [HttpPost("sms")]
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
                NotifyResponse notifyResponse = null;
                try
                {
                    notifyResponse = await ApiServices.SendSms($"https://{Request.Host.Value}", sendSmsRequest.MobileNumber, AppSettings.NotifySmsTemplateId, sessionId, correlationId);

                    model.SentTo = sendSmsRequest.MobileNumber;
                    Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true, HttpOnly = true });
                    return View("SmsSent", model);

                }
                catch (Exception ex)
                {
                    model.ErrorMessage = ex.Message;
                    return View("SmsInput", model);
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

        [HttpGet("reference")]
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

                Response.Cookies.Append("ncs-session-id", sessionId, new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true, HttpOnly = true });
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
    }
}

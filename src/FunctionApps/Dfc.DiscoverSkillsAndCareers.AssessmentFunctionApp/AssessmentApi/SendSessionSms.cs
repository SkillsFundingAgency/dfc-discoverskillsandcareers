using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp
{
    public static class SendSessionSmsTrigger
    {
        [FunctionName("SendSessionSmsTrigger")]
        [ProducesResponseType(typeof(PostAnswerResponse), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "The Sms has been sent", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "No such session exists", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The Sms supplied is not valid or the TemplateId is not supplied or the Domain is not supplied", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "SendSessionSms", Description = "Sends an Sms containing session info to the address supplied.")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "assessment/notify/sms")]HttpRequest req,
            ILogger log,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IUserSessionRepository userSessionRepository,
            [Inject]INotifyClient notifyClient)
        {
            try
            {
                var correlationId = httpRequestHelper.GetDssCorrelationId(req);
                if (string.IsNullOrEmpty(correlationId))
                    log.LogInformation("Unable to locate 'DssCorrelationId' in request header");

                if (!Guid.TryParse(correlationId, out var correlationGuid))
                {
                    log.LogInformation("Unable to parse 'DssCorrelationId' to a Guid");
                    correlationGuid = Guid.NewGuid();
                }

                SendSessionSmsRequest sendSessionSmsRequest;
                using (var streamReader = new StreamReader(req.Body))
                {
                    var body = streamReader.ReadToEnd();
                    sendSessionSmsRequest = JsonConvert.DeserializeObject<SendSessionSmsRequest>(body);
                }

                if (sendSessionSmsRequest == null || string.IsNullOrEmpty(sendSessionSmsRequest.SessionId))
                {
                    log.LogError($"CorrelationId: {correlationGuid} - Session Id not supplied");
                    return httpResponseMessageHelper.BadRequest();
                }

                if (string.IsNullOrEmpty(sendSessionSmsRequest.Domain))
                {
                    log.LogError($"CorrelationId: {correlationGuid} - Domain not supplied");
                    return httpResponseMessageHelper.BadRequest();
                }

                if (string.IsNullOrEmpty(sendSessionSmsRequest.TemplateId))
                {
                    log.LogError($"CorrelationId: {correlationGuid} - TemplateId not supplied");
                    return httpResponseMessageHelper.BadRequest();
                }

                var userSession = await userSessionRepository.GetUserSession(sendSessionSmsRequest.SessionId);
                if (userSession == null)
                {
                    log.LogError($"CorrelationId: {correlationGuid} - Session Id does not exist {sendSessionSmsRequest.SessionId}");
                    return httpResponseMessageHelper.NoContent();
                }

                await notifyClient.SendSms(sendSessionSmsRequest.Domain, sendSessionSmsRequest.MobileNumber, sendSessionSmsRequest.TemplateId, userSession.UserSessionId);

                var result = new SendSessionSmsResponse()
                {
                    IsSuccess = true,
                };
                
                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                var result = new SendSessionSmsResponse()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(result));
            }
        }
    }
}

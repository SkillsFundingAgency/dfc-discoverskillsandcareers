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
    public static class SendSessionEmailTrigger
    {
        [FunctionName("SendSessionEmailTrigger")]
        [ProducesResponseType(typeof(PostAnswerResponse), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "The email has been sent", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "No such session exists", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The email supplied is not valid or the TemplateId is not supplied or the Domain is not supplied", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "SendSessionEmail", Description = "Sends an email containing session info to the address supplied.")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "assessment/notify/email")]HttpRequest req,
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

                SendSessionEmailRequest sendSessionEmailRequest;
                using (var streamReader = new StreamReader(req.Body))
                {
                    var body = streamReader.ReadToEnd();
                    sendSessionEmailRequest = JsonConvert.DeserializeObject<SendSessionEmailRequest>(body);
                }

                if (sendSessionEmailRequest == null || string.IsNullOrEmpty(sendSessionEmailRequest.SessionId))
                {
                    log.LogError($"CorrelationId: {correlationGuid} - Session Id not supplied");
                    return httpResponseMessageHelper.BadRequest();
                }

                if (string.IsNullOrEmpty(sendSessionEmailRequest.Domain))
                {
                    log.LogError($"CorrelationId: {correlationGuid} - Domain not supplied");
                    return httpResponseMessageHelper.BadRequest();
                }

                if (string.IsNullOrEmpty(sendSessionEmailRequest.TemplateId))
                {
                    log.LogError($"CorrelationId: {correlationGuid} - TemplateId not supplied");
                    return httpResponseMessageHelper.BadRequest();
                }

                var userSession = await userSessionRepository.GetUserSession(sendSessionEmailRequest.SessionId);
                if (userSession == null)
                {
                    log.LogError($"CorrelationId: {correlationGuid} - Session Id does not exist {sendSessionEmailRequest.SessionId}");
                    return httpResponseMessageHelper.NoContent();
                }

                await notifyClient.SendEmail(sendSessionEmailRequest.Domain, sendSessionEmailRequest.EmailAddress, sendSessionEmailRequest.TemplateId, userSession.UserSessionId);

                var result = new SendSessionEmailResponse()
                {
                    IsSuccess = true,
                };

                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                var result = new SendSessionEmailResponse()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(result));
            }
        }
    }
}

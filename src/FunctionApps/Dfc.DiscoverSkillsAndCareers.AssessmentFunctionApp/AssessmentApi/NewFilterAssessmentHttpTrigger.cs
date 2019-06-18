using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models;
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
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.AssessmentApi
{
    public class NewFilterAssessmentHttpTrigger
    {
        [FunctionName("NewFilterAssessmentHttpTrigger")]
        [ProducesResponseType(typeof(FilterSessionResponse), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Creates a new filter assessment with the session provided", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Creates a new assessment session and returns the assessment session details")]

        public static async Task<HttpResponseMessage> Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "assessment/filtered/{sessionId}/{jobCategory}")]HttpRequest req,
             string sessionId,
             string jobCategory,
             ILogger log,
             [Inject]IHttpRequestHelper httpRequestHelper,
             [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
             [Inject]IUserSessionRepository userSessionRepository)
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

                if (string.IsNullOrEmpty(sessionId))
                {
                    log.LogInformation($"CorrelationId: {correlationGuid} - Session Id not supplied");
                    return httpResponseMessageHelper.BadRequest();
                }

                var userSession = await userSessionRepository.GetUserSession(sessionId);
                if (userSession == null)
                {
                    log.LogInformation($"CorrelationId: {correlationGuid} - Session Id does not exist {sessionId}");
                    return httpResponseMessageHelper.NoContent();
                }

                if (userSession.ResultData == null)
                {
                    log.LogInformation(
                        $"CorrelationId: {correlationGuid} - Session Id {sessionId} has not completed the short assessment");
                    return httpResponseMessageHelper.BadRequest();
                }
                
                var code = JobCategoryHelper.GetCode(jobCategory);

                if (!userSession.FilteredAssessmentState.JobCategoryStates.Any(j => j.JobCategoryCode.EqualsIgnoreCase(code)))
                {
                    return httpResponseMessageHelper.BadRequest();
                }

                
                userSession.FilteredAssessmentState.CurrentFilterAssessmentCode = code;

                if (userSession.FilteredAssessmentState.IsComplete)
                {
                    userSession.FilteredAssessmentState.RemoveAnswersForCategory(code);
                }

                var questionNumber = userSession.FilteredAssessmentState.MoveToNextQuestion();
                
                await userSessionRepository.UpdateUserSession(userSession);

                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(new FilterSessionResponse()
                {
                    SessionId = userSession.PrimaryKey,
                    QuestionNumber = questionNumber
                }));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Fatal exception {message}", ex.Message);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}

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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.AssessmentApi
{
    public class NewFilterAssessmentHttpTrigger
    {
        [FunctionName("NewFilterAssessmentHttpTrigger")]
        [ProducesResponseType(typeof(DscSession), (int)HttpStatusCode.OK)]
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
             [Inject]IUserSessionRepository userSessionRepository,
             [Inject]IQuestionRepository questionRepository,
             [Inject]IQuestionSetRepository questionSetRepository,
             [Inject]IOptions<AppSettings> appSettings)
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
                    log.LogInformation($"CorrelationId: {correlationGuid} - Session Id {sessionId} has not completed the short assessment");
                    return httpResponseMessageHelper.BadRequest();
                }

                var questionSet = await questionSetRepository.GetCurrentQuestionSet("filtered", jobCategory);
                if (questionSet == null)
                {
                    log.LogInformation($"CorrelationId: {correlationGuid} - Filtered question set does not exist {jobCategory}");
                    return httpResponseMessageHelper.NoContent();
                }

                var jobFamily = userSession.ResultData.JobFamilies.FirstOrDefault(x => x.JobFamilyName.ToLower().Replace(" ", "-") == jobCategory);
                if (jobFamily == null)
                {
                    log.LogInformation($"CorrelationId: {correlationGuid} - Job family {jobCategory} could not be found on session {sessionId}");
                    return httpResponseMessageHelper.NoContent();
                }

                userSession.FilteredAssessmentState = new FilteredAssessmentState
                {
                    CurrentFilterAssessmentCode = jobFamily.JobFamilyCode,
                    JobFamilyName = jobFamily.JobFamilyName,
                    QuestionSetVersion = questionSet.QuestionSetVersion,
                    CurrentQuestion = 1,
                    MaxQuestions = questionSet.MaxQuestions,
                };

                await userSessionRepository.UpdateUserSession(userSession);

                var result = new DscSession()
                {
                    SessionId = userSession.PrimaryKey
                };
                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Fatal exception {message}", ex.Message);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}

using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp.Models;
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp
{
    public static class ResultsForJobCategoryHttpTrigger
    {
        [FunctionName("ResultsForJobCategoryHttpTrigger")]
        [ProducesResponseType(typeof(ResultsJobCategoryResult), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Gets the results for the user session and specified job category if the filtering questions have been completed", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The filtering results have not yet been generated for the job category on the user session", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "The user session could not be found", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "GetResultsForJobCategory", Description = "Gets the results for the user session and specified job category provided.")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "result/{sessionId}/{jobCategory}")]HttpRequest req,
            string sessionId,
            string jobCategory,
            ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IUserSessionRepository userSessionRepository,
            [Inject]IJobProfileRepository jobProfileRepository)
        {
            loggerHelper.LogMethodEnter(log);

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
                loggerHelper.LogInformationMessage(log, correlationGuid, "Session Id not supplied");
                return httpResponseMessageHelper.BadRequest();
            }

            var userSession = await userSessionRepository.GetUserSession(sessionId);
            if (userSession == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Session Id does not exist {0}", sessionId));
                return httpResponseMessageHelper.NoContent();
            }

            if (userSession.ResultData == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Result data does not yet exist for session {0}", userSession.PrimaryKey));
                return httpResponseMessageHelper.BadRequest();
            }

            var filteredAssessment = userSession.ResultData.JobFamilies.FirstOrDefault(x => x.FilterAssessment?.JobFamilyNameUrlSafe == jobCategory)?.FilterAssessment;
            if (filteredAssessment == null)
            {
                loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Filtered assessment data does not yet exist for session {0} and job category {1}", userSession.PrimaryKey, jobCategory));
                return httpResponseMessageHelper.BadRequest();
            }

            // Build the list of job profiles
            var showSuggestedJobProfiles = new List<JobProfileResult>();

            var jobProfiles =
                await jobProfileRepository.JobProfileBySocCodeAndTitle(filteredAssessment.SuggestedJobProfiles);
            
            foreach (var jobProfile in jobProfiles)
            {
                showSuggestedJobProfiles.Add(new JobProfileResult()
                {
                    CareerPathAndProgression = jobProfile.CareerPathAndProgression,
                    Overview = jobProfile.Overview,
                    SalaryExperienced = jobProfile.SalaryExperienced,
                    SalaryStarter = jobProfile.SalaryStarter,
                    JobCategory = jobCategory,
                    SocCode = jobProfile.SocCode,
                    Title = jobProfile.Title,
                    UrlName = jobProfile.UrlName,
                    TypicalHours = jobProfile.TypicalHours,
                    TypicalHoursPeriod = String.Join("/", jobProfile.WorkingHoursDetails),
                    ShiftPattern = String.Join("/",jobProfile.WorkingPattern),
                    ShiftPatternPeriod = String.Join("/",jobProfile.WorkingPatternDetails),
                    WYDDayToDayTasks = jobProfile.WYDDayToDayTasks
                });
            }

            var model = new ResultsJobCategoryResult()
            {
                JobProfiles = showSuggestedJobProfiles.ToArray(),
                SessionId = userSession.UserSessionId
            };

            loggerHelper.LogMethodExit(log);

            return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(model));
        }
    }
}
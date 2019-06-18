using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp.Models;
using DFC.Functions.DI.Standard.Attributes;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp.ResultsApi
{
    public static class ResultsHttpTrigger
    {
        [FunctionName("ResultsHttpTrigger")]
        [ProducesResponseType(typeof(ResultData), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Gets the results for a user session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The results have not yet been generated for the user session", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "The user session could not be found", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "GetResults", Description = "Gets the results for the user session.")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "result/{sessionId}")]HttpRequest req,
            string sessionId,
            ILogger log,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IUserSessionRepository userSessionRepository,
            [Inject]IJobCategoryRepository jobCategoryRepository,
            [Inject]IQuestionSetRepository questionSetRepository,
            [Inject]IQuestionRepository questionRepository,
            [Inject]IJobProfileRepository jobProfileRepository)
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
                    log.LogInformation($"Correlation Id: {correlationId} - Session Id not supplied");
                    return httpResponseMessageHelper.BadRequest();
                }

                var userSession = await userSessionRepository.GetUserSession(sessionId);
                if (userSession == null)
                {
                    log.LogInformation($"Correlation Id: {correlationId} - Session Id does not exist {sessionId}");
                    return httpResponseMessageHelper.NoContent();
                }

                if (userSession.ResultData == null)
                {
                    log.LogInformation($"Correlation Id: {correlationId} - Result data does not yet exist for session {userSession.PrimaryKey}");
                    return httpResponseMessageHelper.BadRequest();
                }

                var questionSet = await questionSetRepository.GetCurrentQuestionSet("filtered");
                if (questionSet == null)
                {
                    log.LogInformation(
                        $"CorrelationId: {correlationGuid} - Unable to find latest filtered question set");
                    return httpResponseMessageHelper.NoContent();
                }


                var questions = await questionRepository.GetQuestions(questionSet.QuestionSetVersion);
                
                var traits = userSession.ResultData.Traits;
                int traitsTake = (traits.Length > 3 && traits[2].TotalScore == traits[3].TotalScore) ? 4 : 3;
                var jobFamilies = userSession.ResultData.JobCategories;
                
                var suggestedJobProfiles = new List<JobProfileResult>();
             //   var rnd = new Random();
                foreach (var jobCategory in jobFamilies)
                {
                    if (jobCategory.FilterAssessmentResult == null)
                    {
                        var category = await jobCategoryRepository.GetJobCategory(jobCategory.JobCategoryCode);
                        userSession.FilteredAssessmentState = userSession.FilteredAssessmentState ?? new FilteredAssessmentState();
                        userSession.FilteredAssessmentState.CreateOrResetCategoryState(questionSet.QuestionSetVersion, questions, category);
                        userSession.FilteredAssessmentState.CurrentFilterAssessmentCode = jobCategory.JobCategoryCode;
                        continue;
                        
                    }
                    // Build the list of job profiles
                    var jobProfiles =
                        //await jobProfileRepository.JobProfilesForJobFamily(jobCategory.JobFamilyName);
                        await jobProfileRepository.JobProfilesTitle(jobCategory.FilterAssessmentResult.SuggestedJobProfiles);

                    
                    //log.LogInformation($"Received the following job profiles for :{Environment.NewLine}{String.Join(Environment.NewLine, jobProfiles.Select(j => j.Title))}{Environment.NewLine} Filter Assessment:{Environment.NewLine} {JsonConvert.SerializeObject(jobCategory.FilterAssessment)}");
                    
                    foreach (var jobProfile in jobProfiles)//.OrderBy(_ => rnd.Next(0,jobProfiles.Length)).Take(20))
                    {
                        suggestedJobProfiles.Add(new JobProfileResult()
                        {
                            CareerPathAndProgression = jobProfile.CareerPathAndProgression,
                            Overview = jobProfile.Overview,
                            SalaryExperienced = jobProfile.SalaryExperienced,
                            SalaryStarter = jobProfile.SalaryStarter,
                            JobCategory = jobCategory.JobCategoryName,
                            SocCode = jobProfile.SocCode,
                            Title = jobProfile.Title,
                            UrlName = jobProfile.UrlName,
                            TypicalHours = jobProfile.TypicalHours,
                            TypicalHoursPeriod = String.Join("/", jobProfile.WorkingHoursDetails),
                            ShiftPattern = String.Join("/", jobProfile.WorkingPattern),
                            ShiftPatternPeriod = String.Join("/", jobProfile.WorkingPatternDetails),
                            WYDDayToDayTasks = jobProfile.WYDDayToDayTasks
                        });
                    }
                }

                var model = new ResultsResponse()
                {
                    AssessmentType = userSession.AssessmentType,
                    SessionId = userSession.UserSessionId,
                    JobFamilyCount = userSession.ResultData.JobCategories.Length,
                    JobFamilyMoreCount = userSession.ResultData.JobCategories.Length - 3,
                    Traits = traits.Take(traitsTake).Select(x => x.TraitText).ToArray(),
                    JobCategories = jobFamilies,
                    JobProfiles = suggestedJobProfiles.ToArray(),
                    WhatYouToldUs = userSession.ResultData?.JobCategories.SelectMany(r => r.FilterAssessmentResult?.WhatYouToldUs ?? new string[] { }).Distinct().ToArray() ?? new string[] { }
                };

                await userSessionRepository.UpdateUserSession(userSession);
                
                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(model));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Fatal exception {message}", ex.Message);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.QuestionsFunctionApp
{
    public static class ImportJobProfilesHttpTrigger
    {
        [FunctionName("ImportJobProfilesHttpTrigger")]
        [ProducesResponseType(typeof(Question), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Import complete", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Question set does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "ImportJobProfiles", Description = "Imports the job profiles")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "import/job-profiles")]HttpRequest req,
            ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IQuestionRepository questionRepository,
            [Inject]IJobProfileRepository jobProfileRepository
            )
        {

            string inputJson = "";
            using (var streamReader = new StreamReader(req.Body))
            {
                inputJson = streamReader.ReadToEnd();
            }

            try
            {
                req.Headers.TryGetValue("import-key", out var headerValue);
                if (headerValue.ToString() != "Hgfy-gYh3") return httpResponseMessageHelper.BadRequest();

                var data = JsonConvert.DeserializeObject<SiteFinityDataFeed<List<DfcJobProfile>>>(inputJson);

                log.LogInformation($"Have {data.Value.Count} job profiles to save");

                foreach (var jobProfile in data.Value)
                {
                    await jobProfileRepository.CreateJobProfile(new JobProfile()
                    {
                        CareerPathAndProgression = jobProfile.CareerPathAndProgression,
                        JobProfileCategories = jobProfile.JobProfileCategories,
                        Overview = jobProfile.Overview,
                        PartitionKey = "jobprofile-cms",
                        SalaryExperienced = jobProfile.SalaryExperienced,
                        SalaryStarter = jobProfile.SalaryStarter,
                        SocCode = jobProfile.SocCode,
                        Title = jobProfile.Title,
                        UrlName = jobProfile.UrlName,
                        WYDDayToDayTasks = jobProfile.WYDDayToDayTasks
                    });
                }

                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(new
                {
                    message = "Ok",
                    importCount = data.Value.Count
                }));
            }

            catch (Exception ex)
            {
                return httpResponseMessageHelper.Ok(JsonConvert.SerializeObject(new
                {
                    message = ex.ToString()
                }));
            }
        }

        public class SiteFinityDataFeed<T> where T : class
        {
            [JsonProperty("value")]
            public T Value { get; set; }
        }

        public class DfcJobProfile
        {
            [JsonProperty("SocCode")]
            public string SocCode { get; set; }
            [JsonProperty("Title")]
            public string Title { get; set; }
            [JsonProperty("Overview")]
            public string Overview { get; set; }
            [JsonProperty("SalaryStarter")]
            public decimal SalaryStarter { get; set; }
            [JsonProperty("SalaryExperienced")]
            public decimal SalaryExperienced { get; set; }
            [JsonProperty("UrlName")]
            public string UrlName { get; set; }
            [JsonProperty("WYDDayToDayTasks")]
            public string WYDDayToDayTasks { get; set; }
            [JsonProperty("CareerPathAndProgression")]
            public string CareerPathAndProgression { get; set; }
            [JsonProperty("JobProfileCategories")]
            public string[] JobProfileCategories { get; set; }
        }

    }
}

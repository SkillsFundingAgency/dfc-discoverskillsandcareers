using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DFC.Swagger.Standard.Annotations;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp
{
    public static class QuestionsHttpTrigger
    {
        [FunctionName("QuestionsHttpTrigger")]
        [ProducesResponseType(typeof(IEnumerable<Models.Question>), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Question Set Not Found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Question set does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Retrieves the questions contained within the requested question set version")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "questionset/{assessmenttype}/{version}/questions")] HttpRequest req, string assessmenttype, string version,
            ILogger log)
        {

            return new OkObjectResult(new List<Models.Question>());
        }
    }
}

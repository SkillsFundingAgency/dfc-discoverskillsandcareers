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
    public static class NextQuestionHttpTrigger
    {
        [FunctionName("NextQuestionHttpTrigger")]
        [ProducesResponseType(typeof(Models.Question), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Gets the next question for a given user session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "No such session exists", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "")]

        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "session/{sessionId}/next")] HttpRequest req, string sessionId, ILogger log)
        {
            if(!String.IsNullOrWhiteSpace(sessionId))
            {
                return new OkObjectResult(new Models.Question());
            } else {
                return new BadRequestResult();
            }
        }
    }

    
}

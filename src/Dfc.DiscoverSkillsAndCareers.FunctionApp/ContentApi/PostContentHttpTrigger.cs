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
    public static class PostContentHttpTrigger
    {
        [FunctionName("PostContentHttpTrigger")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Successfully updated/created the content for a given content type", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "No content for the given content type found", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Gets the content for a given content type.")]

        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "content/{contentType}")] HttpRequest req, string contentType, ILogger log)
        {
            return new OkResult();
        }
    }
}
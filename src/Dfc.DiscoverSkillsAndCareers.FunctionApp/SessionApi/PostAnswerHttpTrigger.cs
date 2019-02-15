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

    public class PostAnswerRequest
    {
        [JsonProperty("questionId")]
        public string QuestionId { get; set; }
        [JsonProperty("questionNumber")]
        public string QuestionNumber { get; set; }
        [JsonProperty("questionText")]
        public string QuestionText { get; set; }

        [JsonProperty("selectedOption")]
        public Models.AnswerOption SelectedOption { get; set; }
    }

    public static class PostAnswerHttpTrigger
    {
        [FunctionName("PostAnswerHttpTrigger")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Stores the answer for the given question against the current session", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "No such session exists", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "The request is malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Post", Description = "Stores an answer for a given question against the current session.")]

        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "session/{sessionId}")] HttpRequest req, string sessionId, ILogger log)
        {
            var body = JsonConvert.DeserializeObject<PostAnswerRequest>("");

            if(body != null)
            {
                return new OkResult();
            } else {
                return new BadRequestResult();
            }
        }
    }
}
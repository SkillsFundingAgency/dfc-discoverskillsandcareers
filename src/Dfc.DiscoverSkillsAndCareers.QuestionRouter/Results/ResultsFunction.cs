using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Collections.Specialized;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.QuestionRouter.Results
{
    public static class ResultsFunction
    {
        [FunctionName("ResultsFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "results")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("ResultsFunction processed a request.");

            var userSessionRepository = new UserSessionRepository();
            UserSession userSession = null;

            if (req.Content.IsFormData())
            {
                NameValueCollection col = req.Content.ReadAsFormDataAsync().Result;
                var sessionId = col.GetValues("sessionId").FirstOrDefault();
                userSession = await userSessionRepository.GetUserSessionAsync(sessionId);
            }

            CalculateResult.Run(userSession);

            string html = $"<body>Results for {userSession.SessionId}</body>";

            // Ok html response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}

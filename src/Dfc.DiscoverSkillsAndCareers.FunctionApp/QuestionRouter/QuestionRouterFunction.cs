using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Dfc.DiscoverSkillsAndCareers.QuestionRouter
{
    public static class QuestionRouterFunction
    {
        [FunctionName("QuestionRouterFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "q/{question?}")]HttpRequestMessage req, string question, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");


            string sessionId = null;
            int questionNumber = 0;

            if (req.Content.IsFormData())
            {
                // Read the form data
                NameValueCollection col = req.Content.ReadAsFormDataAsync().Result;
                sessionId = col.GetValues("sessionId").FirstOrDefault();
                // Question number from the route
                int.TryParse(question, out questionNumber);

                // Check if you have result selected and save that
            }
            else
            {
                if (question == "1")
                {
                    // Start again, no form content on question 1
                    sessionId = System.Guid.NewGuid().ToString();
                    questionNumber = 1;
                }
                else
                {
                    // No form content and not requesting question 1
                    questionNumber = 1;
                }
            }

            if (string.IsNullOrEmpty(sessionId) == true)
            {
                // Session id is missing, redirect to question 1
                var redirectResponse = req.CreateResponse(HttpStatusCode.Redirect);
                var uri = req.RequestUri;
                var host = uri.AbsoluteUri.Replace(uri.AbsolutePath, "");
                redirectResponse.Headers.Location = new System.Uri($"{host}/q/{questionNumber}");
                return redirectResponse;
            }

            // Read the question page html file and replace text strings
            var nextRoute = $"/q/{questionNumber + 1}";
            var buttonText = "Continue";
            if (questionNumber >= 5)
            {
                nextRoute = "/results";
                buttonText = "Finish";
            }
            var html = System.IO.File.ReadAllText("pages/QuestionPage.html");
            html = html.Replace("[question_number]", questionNumber.ToString());
            html = html.Replace("[form_route]", nextRoute);
            html = html.Replace("[session_id]", sessionId);
            html = html.Replace("[button_text]", buttonText);

            // Ok html response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}

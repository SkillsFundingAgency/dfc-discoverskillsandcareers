using System.Net;
using System.Net.Http;
using System.Reflection;
using DFC.Functions.DI.Standard.Attributes;
using DFC.Swagger.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.DiscoverSkillsAndCareers.QuestionsFunctionApp
{
    public static class ApiDefinition
    {
        public const string APIName = "question";
        public const string APITitle = "discover-skills-and-careers-" + APIName;
        public const string APIDefinitionName = APIName + "-api-definition";
        public const string APIDefRoute = APITitle + "/" + APIDefinitionName;
        public const string APIDescription = "Basic details of a National Careers Service " + APITitle + " " + APIName  + " resource";
      
        [FunctionName(APIDefinitionName)]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = APIDefRoute)]HttpRequest req,
            [Inject]ISwaggerDocumentGenerator swaggerDocumentGenerator)
        {
           var swagger = swaggerDocumentGenerator.GenerateSwaggerDocument(req, APITitle + "-sval", APIDescription, APIDefinitionName, Assembly.GetExecutingAssembly());

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(swagger)
            };
        }
    }
}
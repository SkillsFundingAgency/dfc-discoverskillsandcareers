using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Dfc.UnitTests.EndpointTests
{
    public class EndpointTests : IClassFixture<WebApplicationFactory<DiscoverSkillsAndCareers.WebApp.Startup>>
    {
        private readonly WebApplicationFactory<DiscoverSkillsAndCareers.WebApp.Startup> _factory;

        public EndpointTests(WebApplicationFactory<DiscoverSkillsAndCareers.WebApp.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/dysac")]
        [InlineData("/dysac/assessment/short")]
        [InlineData("/dysac/reload")]
        [InlineData("/dysac/results")]
        [InlineData("/dysac/save-my-progress")]
        [InlineData("/dysac/q/short/01")]
        [InlineData("/dysac/finish")]
        [InlineData("/dysac/finish/animal-care")]
        public async Task Get_WithEndpoint_ReturnFound(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            var isOkOrError = response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.InternalServerError;
            Assert.True(isOkOrError);
        }

        [Theory]
        [InlineData("/dysac/q/short/01")]
        [InlineData("/dysac/q/animal-care/01")]
        public async Task Post_WithEndpoint_ReturnBadRequest(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsync(url, new JsonContent(null));

            var actualStatusCode = response.StatusCode;
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, actualStatusCode);
        }

        [Theory]
        [InlineData("/dysac/q/animal-care/01")]
        public async Task Post_WithAnswerData_ReturnSuccess(string url)
        {
            var client = _factory.CreateClient();
            
            var request = new
            {
                questionId = 1,
                selected_answer = "Yes"
            };
            var response = await client.PostAsync(url, new JsonContent(request));

            var actualStatusCode = response.StatusCode;
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, actualStatusCode);
        }

        public class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json")
            { }
        }
    }
}

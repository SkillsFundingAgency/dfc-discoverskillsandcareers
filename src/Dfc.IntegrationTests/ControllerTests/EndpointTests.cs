using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Dfc.IntegrationTests.ControllerTests
{
    public class EndpointTests : IClassFixture<WebApplicationFactory<DiscoverSkillsAndCareers.WebApp.Startup>>
    {
        private readonly WebApplicationFactory<DiscoverSkillsAndCareers.WebApp.Startup> _factory;

        public EndpointTests(WebApplicationFactory<DiscoverSkillsAndCareers.WebApp.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/assessment/short")]
        [InlineData("/reload")]
        [InlineData("/results")]
        [InlineData("/save-my-progress")]
        [InlineData("/q/1")]
        [InlineData("/start")]
        [InlineData("/finish")]
        [InlineData("/finish/animal-care")]
        public async Task Get_WithEndpoint_ReturnSuccess(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            var actual = response.Content.Headers.ContentType.ToString();
            Assert.Equal("text/html; charset=utf-8", actual);
        }

        [Theory]
        [InlineData("/q/1")]
        [InlineData("/qf/1")]
        public async Task Post_WithEndpoint_ReturnBadRequest(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsync(url, new JsonContent(null));

            var actualStatusCode = response.StatusCode;
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, actualStatusCode);
        }

        [Theory]
        [InlineData("/q/1?sessionId=")]
        [InlineData("/qf/1")]
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

using System;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Dfc.IntegrationTests
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
        [InlineData("/reload")]
        [InlineData("/results")]
        [InlineData("/save-my-progress")]
        [InlineData("/q/1")]
        [InlineData("/start")]
        public async Task Get_WithEndpoint_ReturnSuccess(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            var actual = response.Content.Headers.ContentType.ToString();
            Assert.Equal("text/html; charset=utf-8", actual);
        }
    }
}

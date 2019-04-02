using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.IntegrationTests
{
    public class CmsTests
    {
        private ILogger<HttpService> _logger;

        public CmsTests()
        {
            _logger = Substitute.For<ILogger<HttpService>>();
        }

        [Fact]
        public async Task GetJobProfileData_WithLocalData_ShouldContainJobProfiles()
        {
            var httpService = new HttpService(_logger);
            var requester = new GetJobProfileData(httpService);

            var data = await requester.GetData(null);

            Assert.True(data.Count > 0);
            Assert.Equal("Video editor", data.First(x => x.SocCode == "3416I").Title);
        }

        // TODO: others when cms endpoint is available 
    }
}

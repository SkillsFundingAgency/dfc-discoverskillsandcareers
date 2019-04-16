using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Dfc.IntegrationTests.RepositoryTests
{
    public class JobProfileRepositoryTests : IDisposable
    {
        public IConfigurationRoot Configuration { get; set; }

        JobProfileRepository _repository;
        ILogger<JobProfileRepository> _logger;
        IOptions<CosmosSettings> _optsCosmosSettings;

        public JobProfileRepositoryTests()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            Configuration = builder.Build();
            var cosmosSettings = Configuration.GetSection("CosmosSettings").Get<CosmosSettings>();
            _optsCosmosSettings = Options.Create(cosmosSettings);
            _logger = Substitute.For<ILogger<JobProfileRepository>>();
            var documentClient = new DocumentClient(new Uri(cosmosSettings.Endpoint), cosmosSettings.Key);
            _repository = new JobProfileRepository(documentClient, _optsCosmosSettings);
        }

        public void Dispose() { }

        [Fact]
        public async Task GetContent_WithValidKey_ShouldReturnJobProfile()
        {
            var socCode = "startpage";
            var partitionKey = "jobprofiles-cms";

            var jobProfile = await _repository.GetJobProfile(socCode, partitionKey);

            Assert.Null(jobProfile);
        }
    }
}

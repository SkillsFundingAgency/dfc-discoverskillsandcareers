using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Xunit;

namespace Dfc.IntegrationTests.RepositoryTests
{
    public class JobProfileRepositoryTests : IDisposable
    {
        public IConfigurationRoot Configuration { get; set; }

        JobProfileRepository _repository;
        ILogger<JobProfileRepository> _logger;
        IOptions<AzureSearchSettings> _opts;

        public JobProfileRepositoryTests()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            Configuration = builder.Build();

            _opts = Options.Create(Configuration.GetSection("AzureSearchSettings").Get<AzureSearchSettings>());
            _logger = Substitute.For<ILogger<JobProfileRepository>>();

            var settings = _opts.Value;
            var documentClient = new SearchIndexClient(settings.ServiceName, settings.IndexName, new SearchCredentials(settings.ApiKey));
            _repository = new JobProfileRepository(documentClient);
        }

        public void Dispose() { }


        [Fact]
        public async Task GetJobProfilesBySocCodeAndTitle_WithValidKey_ShouldNotBeEmpty()
        {
            var profiles = await _repository.JobProfilesForJobFamily("Law and Legal");

            Assert.NotEmpty(profiles);
            Assert.All(profiles, p => Assert.Contains("Law and legal", p.JobProfileCategories, StringComparer.InvariantCultureIgnoreCase));
        }
    }
}

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
    public class ContentRepositoryTests : IDisposable
    {
        public IConfigurationRoot Configuration { get; set; }

        ContentRepository _repository;
        ILogger<ContentRepository> _logger;
        IOptions<CosmosSettings> _optsCosmosSettings;

        public ContentRepositoryTests()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            Configuration = builder.Build();
            var cosmosSettings = Configuration.GetSection("CosmosSettings").Get<CosmosSettings>();
            _optsCosmosSettings = Options.Create(cosmosSettings);
            _logger = Substitute.For<ILogger<ContentRepository>>();
            var documentClient = new DocumentClient(new Uri(cosmosSettings.Endpoint), cosmosSettings.Key);
            _repository = new ContentRepository(_logger, documentClient, _optsCosmosSettings);
        }

        public void Dispose() { }

        [Fact]
        public async Task GetContent_WithValidKey_ShouldReturnContent()
        {
            var contentType = "startpage";

            var content = await _repository.GetContent(contentType);

            Assert.Equal(contentType, content.ContentType);
        }

        [Fact]
        public async Task GetContent_WithMissingKey_ShouldReturnNull()
        {
            var contentType = "notfoundkey";

            var content = await _repository.GetContent(contentType);

            Assert.Null(content);
        }

        [Fact]
        public async Task GetContent_WithNoKey_ShouldThrowException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                string contentType = null;

                var content = await _repository.GetContent(contentType);
            });
        }

        [Fact]
        public async Task CreateContent_WithValidKey_ShouldReturnContent()
        {
            var content = new Content()
            {
                ContentType = "test",
                ContentData = "{}"
            };

            await _repository.CreateContent(content);

            Assert.True(!string.IsNullOrEmpty(content.Id));
        }
    }
}

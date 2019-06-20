using Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp.Ioc;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Search;
using Microsoft.Extensions.Options;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]
namespace Dfc.DiscoverSkillsAndCareers.ResultsFunctionApp.Ioc
{
    internal class WebJobsExtensionStartup : IWebJobsStartup
    {
        public IConfiguration Configuration { get; private set; }

        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddDependencyInjection();

            ConfigureServices(builder.Services);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            ConfigureOptions(services);

            services.AddSingleton<DocumentClient>(srvs => {
                var cosmosSettings = srvs.GetService<IOptions<CosmosSettings>>();
                return new DocumentClient(new Uri(cosmosSettings?.Value.Endpoint), cosmosSettings?.Value.Key);
            });

            services.AddSingleton<ISearchIndexClient>(srvs =>
            {
                var searchSettings = srvs.GetService<IOptions<AzureSearchSettings>>();
                return new SearchIndexClient(searchSettings.Value.ServiceName, searchSettings.Value.IndexName, new SearchCredentials(searchSettings.Value.ApiKey));
            });


            services.AddSingleton<ILoggerHelper, LoggerHelper>();
            services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
            services.AddSingleton<IHttpResponseMessageHelper, HttpResponseMessageHelper>();
            services.AddSingleton<IJsonHelper, JsonHelper>();

            services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();

            services.AddTransient<IUserSessionRepository, UserSessionRepository>();
            services.AddTransient<IQuestionRepository, QuestionRepository>();
            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<IJobProfileRepository, JobProfileRepository>();
            services.AddTransient<IJobCategoryRepository, JobCategoryRepository>();
            services.AddTransient<IQuestionSetRepository, QuestionSetRepository>();
        }

        private void ConfigureOptions(IServiceCollection services)
        {
            services.AddOptions();

            bool isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
            var configBuilder = new ConfigurationBuilder();
            if (isLocal)
            {
                configBuilder.SetBasePath(System.Environment.CurrentDirectory);
                configBuilder.AddJsonFile(@"local.settings.json", optional: true, reloadOnChange: false);
            }
            Configuration = configBuilder.AddEnvironmentVariables().Build();

            services.Configure<CosmosSettings>(Configuration.GetSection("CosmosSettings"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<AzureSearchSettings>(Configuration.GetSection("AzureSearchSettings"));
        }
    }
}

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
using System.Diagnostics.CodeAnalysis;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Ioc;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using Notify.Client;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]
namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Ioc
{
    [ExcludeFromCodeCoverage]
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
            services.AddSingleton<IUserSessionRepository, UserSessionRepository>();
            services.AddSingleton<IQuestionRepository, QuestionRepository>();
            services.AddSingleton<IContentRepository, ContentRepository>();
            services.AddSingleton<IQuestionSetRepository, QuestionSetRepository>();
            services.AddSingleton<IJobCategoryRepository, JobCategoryRepository>();
            services.AddSingleton<IShortTraitRepository, ShortTraitRepository>();
            services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
            services.AddTransient<IAssessmentCalculationService, AssessmentCalculationService>();
            services.AddTransient<IFilterAssessmentCalculationService, FilterAssessmentCalculationService>();
            services.AddSingleton<INotificationClient>(srvs => {
                var appSettings = srvs.GetService<IOptions<AppSettings>>().Value;
                return new NotificationClient(appSettings.NotifyApiKey);
            });
            services.AddTransient<INotifyClient, NotifyClient>();
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

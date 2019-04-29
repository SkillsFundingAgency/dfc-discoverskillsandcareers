using System;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp;
using Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.Functions.DI.Standard;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Interfaces;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]
namespace Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp
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

            services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
            services.AddSingleton<IHttpResponseMessageHelper, HttpResponseMessageHelper>();
            services.AddSingleton<IJsonHelper, JsonHelper>();
            services.AddSingleton<IUserSessionRepository, UserSessionRepository>();
            services.AddSingleton<IQuestionRepository, QuestionRepository>();
            services.AddSingleton<IContentRepository, ContentRepository>();
            services.AddSingleton<IQuestionSetRepository, QuestionSetRepository>();
            services.AddSingleton<IJobProfileRepository, JobProfileRepository>();
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

            var appSettings = new AppSettings();
            Configuration.Bind("AppSettings", appSettings);

            var cosmosSettings = new CosmosSettings();
            Configuration.Bind("CosmosSettings", cosmosSettings);

            services.Configure<CosmosSettings>(env =>
            {
                env.DatabaseName = cosmosSettings.DatabaseName;
                env.Endpoint = cosmosSettings.Endpoint;
                env.Key = cosmosSettings.Key;
            });
            services.Configure<AppSettings>(env =>
            {
                env.SessionSalt = appSettings.SessionSalt;
                env.NotifyApiKey = appSettings.NotifyApiKey;
            });
        }
    }
}

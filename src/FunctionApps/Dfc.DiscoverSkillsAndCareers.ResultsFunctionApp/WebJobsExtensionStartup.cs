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

            services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
            services.AddSingleton<IHttpResponseMessageHelper, HttpResponseMessageHelper>();
            services.AddSingleton<IJsonHelper, JsonHelper>();

            services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();

            services.AddTransient<IUserSessionRepository, UserSessionRepository>();
            services.AddTransient<IQuestionRepository, QuestionRepository>();
            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<IJobProfileRepository, JobProfileRepository>();
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
            });
        }
    }
}

using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Ioc;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]
namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Ioc
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


            services.AddSingleton<ILoggerHelper, LoggerHelper>();
            services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
            services.AddSingleton<IHttpResponseMessageHelper, HttpResponseMessageHelper>();
            services.AddSingleton<IJsonHelper, JsonHelper>();
            services.AddSingleton<IUserSessionRepository, UserSessionRepository>();
            services.AddSingleton<IQuestionRepository, QuestionRepository>();
            services.AddSingleton<IContentRepository, ContentRepository>();
            services.AddSingleton<IJobProfileRepository, JobProfileRepository>();
            services.AddSingleton<IShortTraitRepository, ShortTraitRepository>();
            services.AddSingleton<IHttpService, HttpService>();
            services.AddSingleton<IQuestionSetRepository, QuestionSetRepository>();

            services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();

            services.AddTransient<IGetShortTraitData, GetShortTraitData>();
            services.AddTransient<IShortTraitDataProcessor, ShortTraitDataProcessor>();
            services.AddTransient<IGetShortQuestionSetData, GetShortQuestionSetData>();
            services.AddTransient<IShortQuestionSetDataProcessor, ShortQuestionSetDataProcessor>();
            services.AddTransient<IGetShortQuestionData, GetShortQuestionData>();
            services.AddTransient<IContentDataProcessor<ContentStartPage>, ContentDataProcessor<ContentStartPage>>();
            services.AddTransient<IGetContentData<List<ContentStartPage>>, GetContentData<List<ContentStartPage>>>();
            services.AddTransient<IContentDataProcessor<ContentQuestionPage>, ContentDataProcessor<ContentQuestionPage>>();
            services.AddTransient<IGetContentData<List<ContentQuestionPage>>, GetContentData<List<ContentQuestionPage>>>();
            services.AddTransient<IContentDataProcessor<ContentFinishPage>, ContentDataProcessor<ContentFinishPage>>();
            services.AddTransient<IGetContentData<List<ContentFinishPage>>, GetContentData<List<ContentFinishPage>>>();
            services.AddTransient<IContentDataProcessor<ContentShortResultsPage>, ContentDataProcessor<ContentShortResultsPage>>();
            services.AddTransient<IGetContentData<List<ContentShortResultsPage>>, GetContentData<List<ContentShortResultsPage>>>();
            services.AddTransient<IGetFilteringQuestionData, GetFilteringQuestionData>();
            services.AddTransient<IGetFilteringQuestionSetData, GetFilteringQuestionSetData>();
            services.AddTransient<IFilteredQuestionSetDataProcessor, FilteredtQuestionSetDataProcessor>();
            services.AddTransient<IContentDataProcessor<ContentIndexPage>, ContentDataProcessor<ContentIndexPage>>();
            services.AddTransient<IGetContentData<List<ContentIndexPage>>, GetContentData<List<ContentIndexPage>>>();
            services.AddTransient<IGetJobProfileData, GetJobProfileData>();
            services.AddTransient<IJobProfileDataProcessor, JobProfileDataProcessor>();
            
            
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
                env.SiteFinityApiUrlbase = appSettings.SiteFinityApiUrlbase;
                env.SiteFinityApiWebService = appSettings.SiteFinityApiWebService;
            });
        }
    }
}

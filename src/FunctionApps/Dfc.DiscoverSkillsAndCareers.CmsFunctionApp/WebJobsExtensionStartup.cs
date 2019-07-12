using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.Common.Standard.Logging;
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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]
namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp
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

            services.AddMemoryCache();
            
            services.AddSingleton<DocumentClient>(srvs => {
                var cosmosSettings = srvs.GetService<IOptions<CosmosSettings>>();
                return new DocumentClient(new Uri(cosmosSettings?.Value.Endpoint), cosmosSettings?.Value.Key);
            });

           
            services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
            services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
            services.AddSingleton<IHttpResponseMessageHelper, HttpResponseMessageHelper>();
            services.AddSingleton<IJsonHelper, JsonHelper>();
            
            services.AddSingleton<IUserSessionRepository, UserSessionRepository>();
            services.AddSingleton<IQuestionRepository, QuestionRepository>();
            services.AddSingleton<IShortTraitRepository, ShortTraitRepository>();
            services.AddSingleton<ISiteFinityHttpService, SiteFinityHttpService>();
            services.AddSingleton<IQuestionSetRepository, QuestionSetRepository>();
            services.AddSingleton<IJobCategoryRepository, JobCategoryRepository>();
            
            services.AddTransient<IContentTypeProcessor<ShortTraitDataProcessor>, ShortTraitDataProcessor>();
            services.AddTransient<IContentTypeProcessor<ShortQuestionSetDataProcessor>, ShortQuestionSetDataProcessor>();
            services.AddTransient<IContentTypeProcessor<JobProfileSkillsProcessor>, JobProfileSkillsProcessor>();
            services.AddTransient<IContentTypeProcessor<FilteredQuestionSetDataProcessor>, FilteredQuestionSetDataProcessor>();
            services.AddTransient<IContentTypeProcessor<JobCategoryDataProcessor>, JobCategoryDataProcessor>();
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
            services.Configure<SiteFinitySettings>(Configuration.GetSection("AppSettings"));
        }
    }
}

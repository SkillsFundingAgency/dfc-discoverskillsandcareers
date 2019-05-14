using DFC.Functions.DI.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using DFC.Common.Standard.Logging;
using Microsoft.Extensions.Options;
using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Common.Blob;
using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data;
using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Processor;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Processor
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

            services.AddScoped<IUnderstandMyselfDbContext>(srvs =>
            {
                var connectionString = System.Environment.GetEnvironmentVariable("SqlConnectionString");
                return new UnderstandMyselfDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<UnderstandMyselfDbContext>(), connectionString);
            });

            services.AddSingleton<ILoggerHelper, LoggerHelper>();
            services.AddSingleton<IBlobStorageService>(srvs =>
            {
                var blobStorageSettings = srvs.GetService<IOptions<BlobStorageSettings>>();
                return new BlobStorageService(blobStorageSettings.Value);
            });
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

            var blobStorageSettings = new BlobStorageSettings();
            Configuration.Bind("BlobStorageSettings", blobStorageSettings);
            services.Configure<BlobStorageSettings>(env =>
            {
                env.StorageConnectionString = blobStorageSettings.StorageConnectionString;
                env.ContainerName = blobStorageSettings.ContainerName;
            });
        }
    }
}

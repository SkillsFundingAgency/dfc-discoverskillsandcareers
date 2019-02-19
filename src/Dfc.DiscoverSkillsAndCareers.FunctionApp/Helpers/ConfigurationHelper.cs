using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Helpers
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// When debugging locally, we gete settings from local.settings.json
        /// When running in Azure, it gets the settings from the Application settings tab.
        /// </summary>
        public static AppSettings ReadConfiguration(ExecutionContext context)
        {
            bool isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory);
            if (isLocal)
            {
                configBuilder.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
            }
            var config = configBuilder.AddEnvironmentVariables()
                .Build();

            var appSettings = new AppSettings();
            config.Bind(appSettings);
            //if (appSettings.StaticSiteDomain?.EndsWith('/') == true)
            //{
            //    appSettings.StaticSiteDomain = appSettings.StaticSiteDomain.TrimEnd('/');
            //}
            return appSettings;
        }
    }
}

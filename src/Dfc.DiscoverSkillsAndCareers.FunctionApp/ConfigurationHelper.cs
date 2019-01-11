using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// When debugging locally, we gete settings from local.settings.json
        /// When running in Azure, it gets the settings from the Application settings tab.
        /// </summary>
        public static AppSettings ReadConfiguration(ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var appSettings = new AppSettings();
            config.Bind(appSettings);
            return appSettings;
        }
    }
}

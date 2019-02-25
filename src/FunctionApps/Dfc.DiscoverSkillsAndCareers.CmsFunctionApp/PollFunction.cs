using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp
{
    public static class PollFunction
    {
        [FunctionName("PollFunction")]
        public static  async Task Run([TimerTrigger("*/10 * * * *")]TimerInfo myTimer,
            ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IShortQuestionSetPoller shortQuestionSetPoller
            )
        {
            log.LogInformation($"PollFunction executed at: {DateTime.UtcNow}");

            // Check the short questions
            await shortQuestionSetPoller.RunOnce();

        }
    }
}

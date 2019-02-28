using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
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
            [Inject]IShortTraitDataProcessor shortTraitDataProcessor,
            [Inject]IShortQuestionSetDataProcessor shortQuestionSetDataProcessor,
            [Inject]IContentDataProcessor<ContentStartPage> startPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentQuestionPage> questionPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentFinishPage> finishPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentShortResultsPage> shortResultPageContentDataProcessor
            )
        {
            log.LogInformation($"PollFunction executed at: {DateTime.UtcNow}");

            await startPageContentDataProcessor.RunOnce("startpages", "startpage");

            await questionPageContentDataProcessor.RunOnce("questionpages", "questionpage");

            await finishPageContentDataProcessor.RunOnce("finishpages", "finishpage");

            await shortResultPageContentDataProcessor.RunOnce("shortresultspages", "shortresultpage");

            await shortTraitDataProcessor.RunOnce();

            await shortQuestionSetDataProcessor.RunOnce();

        }
    }
}

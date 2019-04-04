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
        public static  async Task Run([TimerTrigger("*/4 * * * *")]TimerInfo myTimer,
            ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IShortTraitDataProcessor shortTraitDataProcessor,
            [Inject]IShortQuestionSetDataProcessor shortQuestionSetDataProcessor,
            [Inject]IContentDataProcessor<ContentQuestionPage> questionPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentFinishPage> finishPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentShortResultsPage> shortResultPageContentDataProcessor,
            [Inject]IFilteredQuestionSetDataProcessor filteredQuestionSetDataProcessor,
            [Inject]IContentDataProcessor<ContentIndexPage> indexPageContentDataProcessor,
            [Inject]IJobProfileDataProcessor jobProfileDataProcessor,
            [Inject]IFunctionalCompetencyDataProcessor functionalCompetencyDataProcessor,
            [Inject]IJobCategoryDataProcessor jobCategoryDataProcessor
            )
        {
            log.LogInformation($"PollFunction executed at: {DateTime.UtcNow}");

            await shortTraitDataProcessor.RunOnce();

            await jobCategoryDataProcessor.RunOnce();

            await filteredQuestionSetDataProcessor.RunOnce();

            await functionalCompetencyDataProcessor.RunOnce();

            await indexPageContentDataProcessor.RunOnce("indexpagecontents", "indexpage");

            await questionPageContentDataProcessor.RunOnce("questionpagecontents", "questionpage");

            await finishPageContentDataProcessor.RunOnce("finishpagecontents", "finishpage");

            await shortResultPageContentDataProcessor.RunOnce("resultspagecontents", "shortresultpage");

            await shortQuestionSetDataProcessor.RunOnce();

            await jobProfileDataProcessor.RunOnce();
        }
    }
}

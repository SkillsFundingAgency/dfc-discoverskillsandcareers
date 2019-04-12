using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp
{
    public static class PollFunction
    {
        public const string Schedule = "%PollingSchedule%";

        [FunctionName("PollFunction")]
        public static  async Task Run([TimerTrigger(Schedule)]TimerInfo myTimer,
            ILogger log,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IShortTraitDataProcessor shortTraitDataProcessor,
            [Inject]IShortQuestionSetDataProcessor shortQuestionSetDataProcessor,
            [Inject]IContentDataProcessor<ContentQuestionPage> questionPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentFinishPage> finishPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentResultsPage> resultPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentSaveProgressPage> saveProgressPageContentDataProcessor,
            [Inject]IFilteredQuestionSetDataProcessor filteredQuestionSetDataProcessor,
            [Inject]IContentDataProcessor<ContentIndexPage> indexPageContentDataProcessor,
            [Inject]IJobProfileDataProcessor jobProfileDataProcessor,
            [Inject]IFunctionalCompetencyDataProcessor functionalCompetencyDataProcessor,
            [Inject]IJobCategoryDataProcessor jobCategoryDataProcessor
            )
        {
            var id = Guid.NewGuid();
            try
            {
                log.LogInformation($"PollFunction executed at: {DateTime.UtcNow}");

                await shortTraitDataProcessor.RunOnce();

                await jobCategoryDataProcessor.RunOnce();

                await filteredQuestionSetDataProcessor.RunOnce();

                await functionalCompetencyDataProcessor.RunOnce();

                await indexPageContentDataProcessor.RunOnce("indexpagecontents", "indexpage");

                await questionPageContentDataProcessor.RunOnce("questionpagecontents", "questionpage");

                await finishPageContentDataProcessor.RunOnce("finishpagecontents", "finishpage");

                await resultPageContentDataProcessor.RunOnce("resultspagecontents", "resultspagecontents");

                await saveProgressPageContentDataProcessor.RunOnce("saveprogresscontents", "saveprogresspage");

                await shortQuestionSetDataProcessor.RunOnce();

                await jobProfileDataProcessor.RunOnce();

                log.LogInformation($"PollFunction completed at: {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                loggerHelper.LogException(log, id, ex);
            }
        }
    }
}

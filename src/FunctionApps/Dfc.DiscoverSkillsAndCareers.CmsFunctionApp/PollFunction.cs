using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            [Inject]IShortTraitDataProcessor shortTraitDataProcessor,
            [Inject]IShortQuestionSetDataProcessor shortQuestionSetDataProcessor,
            [Inject]IContentDataProcessor<ContentQuestionPage> questionPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentFinishPage> finishPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentResultsPage> resultPageContentDataProcessor,
            [Inject]IContentDataProcessor<ContentSaveProgressPage> saveProgressPageContentDataProcessor,
            [Inject]IFilteredQuestionSetDataProcessor filteredQuestionSetDataProcessor,
            [Inject]IContentDataProcessor<ContentIndexPage> indexPageContentDataProcessor,
            [Inject]IJobCategoryDataProcessor jobCategoryDataProcessor
            )
        {
            var id = Guid.NewGuid();
            try
            {
                log.LogInformation($"PollFunction executed at: {myTimer.ScheduleStatus.Last:O}");

                await shortTraitDataProcessor.RunOnce(log);

                await jobCategoryDataProcessor.RunOnce(log);

                await filteredQuestionSetDataProcessor.RunOnce(log);
                
                await indexPageContentDataProcessor.RunOnce(log, "indexpagecontents", "indexpage");

                await questionPageContentDataProcessor.RunOnce(log, "questionpagecontents", "questionpage");

                await finishPageContentDataProcessor.RunOnce(log, "finishpagecontents", "finishpage");

                await finishPageContentDataProcessor.RunOnce(log, "finishpagecontents", "finishjobcategorypage");

                await resultPageContentDataProcessor.RunOnce(log, "resultspagecontents", "shortresultpage");

                await resultPageContentDataProcessor.RunOnce(log, "resultspagecontents", "filteredresultpage");

                await saveProgressPageContentDataProcessor.RunOnce(log, "saveprogresscontents", "saveprogresspage");

                await shortQuestionSetDataProcessor.RunOnce(log);

                log.LogInformation($"PollFunction completed at: {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Running CMS extract function failed. {ex.Message}");
            }
        }
    }
}

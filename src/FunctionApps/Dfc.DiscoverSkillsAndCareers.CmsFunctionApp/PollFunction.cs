using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataProcessors;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp
{
    public static class PollFunction
    {
        public const string Schedule = "%PollingSchedule%";

        [FunctionName("PollFunction")]
        public static async Task Run([TimerTrigger(Schedule)]TimerInfo myTimer,
            ILogger log,
            [Inject]ISiteFinityHttpService siteFinityHttpService,
            [Inject]IContentTypeProcessor<ShortTraitDataProcessor> shortTraitDataProcessor,
            [Inject]IContentTypeProcessor<ShortQuestionSetDataProcessor> shortQuestionSetDataProcessor,
            [Inject]IContentTypeProcessor<FilteredQuestionSetDataProcessor> filteredQuestionSetDataProcessor,
            [Inject]IContentTypeProcessor<JobCategoryDataProcessor> jobCategoryDataProcessor,
            [Inject]IContentTypeProcessor<JobProfileSkillsProcessor> jobProfileSkillDataProcessor
            )
        {
            var id = Guid.NewGuid();
            try
            {
                siteFinityHttpService.Logger = log;

                log.LogInformation($"PollFunction executed at: {myTimer.ScheduleStatus.Last:O}");

                await shortTraitDataProcessor.RunOnce(log);

                await jobCategoryDataProcessor.RunOnce(log);

                await shortQuestionSetDataProcessor.RunOnce(log);

                await filteredQuestionSetDataProcessor.RunOnce(log);

                await jobProfileSkillDataProcessor.RunOnce(log);

                log.LogInformation($"PollFunction completed at: {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Running CMS extract function failed. {ex.Message}");
            }
        }
    }
}
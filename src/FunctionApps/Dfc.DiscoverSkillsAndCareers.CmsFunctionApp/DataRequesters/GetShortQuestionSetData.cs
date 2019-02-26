using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetShortQuestionSetData : IGetShortQuestionSetData
    {
        readonly IHttpService HttpService;

        public GetShortQuestionSetData(IHttpService httpService)
        {
            HttpService = httpService;
        }
            
        public async Task<ShortQuestionSet> GetData(string url)
        {
            string json = await HttpService.GetString(url);
            // TODO: dummy data
            var data = new ShortQuestionSet()
            {
                Title = "Test",
                Description = "Test questionset",
                LastUpdated = new DateTime(2019, 2, 1),
                Questions = new List<ShortQuestion>()
                {
                    new ShortQuestion()
                    {
                        Trait = "doer",
                        Order = 1,
                        IsNegative = false,
                        Title = "Has this question loaded again?",
                        Description = "First test question"
                    },
                    new ShortQuestion()
                    {
                        Trait = "doer",
                        Order = 2,
                        IsNegative = false,
                        Title = "Second question?",
                        Description = "First test question"
                    }
                }
            };
            return data;
        }
    }
}

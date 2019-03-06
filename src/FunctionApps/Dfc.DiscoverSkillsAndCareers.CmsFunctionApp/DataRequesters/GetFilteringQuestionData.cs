using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    public class GetFilteringQuestionData : IGetFilteringQuestionData
    {
        readonly IHttpService HttpService;

        public GetFilteringQuestionData(IHttpService httpService)
        {
            HttpService = httpService;
        }

        public Task<List<FilteringQuestion>> GetData(string siteFinityApiUrlbase, string questionSetId)
        {
            if (questionSetId == "AC1")
            {
                var result = new List<FilteringQuestion>()
                {
                   new FilteringQuestion()
                   {
                        Id = System.Guid.NewGuid().ToString(),
                        Order = 1,
                        Title = "Filtering question 1",
                        ExcludesJobProfiles = new List<string>()
                        {
                            "Beekeeper",
                            "Farm worker"
                        }
                   }
                };
                return Task.FromResult(result);
            }

            else if (questionSetId == "SC1")
            {
                var result = new List<FilteringQuestion>()
                {
                   new FilteringQuestion()
                   {
                        Id = System.Guid.NewGuid().ToString(),
                        Order = 1,
                        Title = "Social case filtering question 1",
                        ExcludesJobProfiles = new List<string>()
                        {
                            "Nurse",
                            "Doctor"
                        }
                   }
                };
                return Task.FromResult(result);
            }

            else if (questionSetId == "SL1")
            {
                var result = new List<FilteringQuestion>()
                {
                   new FilteringQuestion()
                   {
                        Id = System.Guid.NewGuid().ToString(),
                        Order = 1,
                        Title = "Sports and leisure case filtering question 1",
                        ExcludesJobProfiles = new List<string>()
                        {
                            "Nurse",
                            "Doctor"
                        }
                   },
                   new FilteringQuestion()
                   {
                        Id = System.Guid.NewGuid().ToString(),
                        Order = 2,
                        Title = "Sports and leisure question 2",
                        ExcludesJobProfiles = new List<string>()
                        {
                            "Nurse",
                            "Doctor"
                        }
                   }
                };
                return Task.FromResult(result);
            }

            return null;
        }
    }
}

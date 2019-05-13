using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{

    public class JobProfileRepository : IJobProfileRepository
    {
        private readonly ISearchIndexClient _client;


        public JobProfileRepository(ISearchIndexClient client)
        {
            _client = client;
        }

        public async Task<JobProfile[]> JobProfileBySocCodeAndTitle(IDictionary<string, string> socCodeTitleMap)
        {
            var queryString = String.Join("||", socCodeTitleMap.Values.Select(s => $"({s})"));
            var searchParameters = new SearchParameters
            {
                SearchMode = SearchMode.All,
                SearchFields = new[] { "Title" },
                QueryType = QueryType.Full
            };

            var results = await _client.Documents.SearchAsync<JobProfile>(queryString, searchParameters);
            var contToken = results.ContinuationToken;

            while (contToken != null)
            {
                var nextResults = await _client.Documents.ContinueSearchAsync<JobProfile>(contToken);
                foreach (var result in nextResults.Results)
                {
                    results.Results.Add(result);
                }

                contToken = nextResults.ContinuationToken;

            }


            return results.Results.Select(jp => jp.Document).Where(x => socCodeTitleMap.ContainsKey(x.SocCode)).ToArray();
        }

        public async Task<JobProfile[]> JobProfilesForJobFamily(string jobFamily)
        {
            var searchResults = await _client.Documents.SearchAsync<JobProfile>(jobFamily, new SearchParameters
            {
                SearchMode = SearchMode.Any,
                SearchFields = new[] { "JobProfileCategories" },
            });

            return searchResults.Results.Select(r => r.Document).ToArray();
        }
    }
}
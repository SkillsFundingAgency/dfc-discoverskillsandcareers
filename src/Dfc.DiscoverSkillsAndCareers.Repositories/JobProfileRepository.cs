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

        private async Task<IList<SearchResult<T>>> RunAzureSearchQuery<T>(string query, params string[] fields)
            where T : class
        {
            var searchParameters = new SearchParameters
            {
                ScoringProfile = "jp",
                SearchMode = SearchMode.All,
                SearchFields = fields,
                QueryType = QueryType.Full
            };

            var results = await _client.Documents.SearchAsync<T>(query, searchParameters);
            var contToken = results.ContinuationToken;

            while (contToken != null)
            {
                var nextResults = await _client.Documents.ContinueSearchAsync<T>(contToken);
                foreach (var result in nextResults.Results)
                {
                    results.Results.Add(result);
                }

                contToken = nextResults.ContinuationToken;

            }


            return results.Results;
        }

        public async Task<JobProfile[]> JobProfileBySocCodeAndTitle(IDictionary<string, string> socCodeTitleMap)
        {
            var queryString = String.Join("||", socCodeTitleMap.Values.Select(s => $"({s})"));

            var results = await RunAzureSearchQuery<JobProfile>(queryString, "Title");
            return results.Select(jp => jp.Document).Where(x => socCodeTitleMap.ContainsKey(x.SocCode)).ToArray();
        }

        public async Task<JobProfile[]> JobProfilesForJobFamily(string jobFamily)
        {
            var results = await RunAzureSearchQuery<JobProfile>($"({jobFamily})", "JobProfileCategories");
            return results.Select(r => r.Document).ToArray();
        }
    }
}
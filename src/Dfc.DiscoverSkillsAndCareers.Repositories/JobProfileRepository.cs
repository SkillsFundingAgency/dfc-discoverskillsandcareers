using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Azure.Search;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;

namespace Dfc.DiscoverSkillsAndCareers.Repositories
{

    [ExcludeFromCodeCoverage]
    public class JobProfileRepository : IJobProfileRepository
    {
        private readonly ISearchIndexClient _client;
        private readonly ISiteFinityHttpService _siteFinityHttpService;

        public JobProfileRepository(ISearchIndexClient client, ISiteFinityHttpService siteFinityHttpService)
        {
            _client = client;
            _siteFinityHttpService = siteFinityHttpService;
        }

        private async Task EnsureClient()
        {
            try
            {
                var searchParameters = new SearchParameters
                {
                    ScoringProfile = "jp",
                    SearchMode = SearchMode.All,
                    QueryType = QueryType.Full,
                    Top = 1
                };

                var _ = await _client.Documents.SearchAsync("*", searchParameters);
            }
            catch (CloudException)
            {
                var index = await _siteFinityHttpService.GetLatestIndex("DFC.Digital.JobProfileSearchIndex");
                _client.IndexName = index.Trim('"');
            }

        }
        
        private async Task<IList<SearchResult<T>>> RunAzureSearchQuery<T>(string query, params string[] fields)
            where T : class
        {
            await EnsureClient();
            
            var searchParameters = new SearchParameters
            {
                ScoringProfile = "jp",
                SearchMode = SearchMode.All,
                SearchFields = fields,
                Filter = $"JobProfileSpecialism/any(s: s ne '_exclude_from_dysac_')",
                QueryType = QueryType.Full
            };
        
            var results = await _client.Documents.SearchAsync<T>(query, searchParameters);
            var data = new List<SearchResult<T>>();

            foreach (var result in results.Results)
            {
                data.Add(result);
            }
            
            var contToken = results.ContinuationToken;
            
            while (contToken != null)
            {
                var nextResults = await _client.Documents.ContinueSearchAsync<T>(contToken);
                foreach (var result in nextResults.Results)
                {
                    data.Add(result);
                }
                
                contToken = nextResults.ContinuationToken;
            }
            
            return data;
        }
        
        public async Task<JobProfile[]> JobProfilesForJobFamily(string jobFamily)
        {
            var results = await RunAzureSearchQuery<JobProfile>($"({jobFamily})", "JobProfileCategories");
            return results.Select(r => r.Document).ToArray();
        }
        
    }
}
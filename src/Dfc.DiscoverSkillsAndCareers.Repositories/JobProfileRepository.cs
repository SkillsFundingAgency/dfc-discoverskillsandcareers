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
            IList<SearchResult<JobProfile>> results = null;

            try
            {
                results = await RunAzureSearchQuery<JobProfile>($"({jobFamily})", "JobProfileCategories");
            }
            catch (CloudException cloudException)
            {
                //Only make calls to Sitefinity if the index returns 404 not found
                if (cloudException.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    //Get the new index name and set it in the client
                    var index = await _siteFinityHttpService.GetLatestIndex("DFC.Digital.JobProfileSearchIndex");
                    _client.IndexName = index.Trim('"');

                    //Try the call again with the new index name
                    results = await RunAzureSearchQuery<JobProfile>($"({jobFamily})", "JobProfileCategories");
                }
                else
                {
                    throw;
                }
            }

            return results.Select(r => r.Document).ToArray();
        }
        
    }
}
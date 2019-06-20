using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services
{
    public interface ISiteFinityHttpService
    {
        Task<T> Get<T>(string contentType) where T : class;
        
        Task<List<T>> GetAll<T>(string contentType) where T : class;

        Task<List<TaxonomyHierarchy>> GetTaxonomyInstances(string taxonomyName);
        Task<string> PostData(string contentType, object data);
        Task<string> PostData(string contentType, string data);
        Task Delete(string contentType);
        string MakeAbsoluteUri(string relativePortion);
        ILogger Logger { get; set; }
    }
}

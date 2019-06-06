using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.SupportApp.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    public static class SiteFinityWorkflowRunner
    {
        public static async Task RunDelete(ISiteFinityHttpService service, string baseUrl, string contentType)
        {
            var contentTypeUrl = $"{baseUrl}/{contentType}"; 
            var instances = await service.GetString(contentTypeUrl).FromJson<SiteFinityDataFeed<JObject[]>>();

            foreach (var instance in instances.Value)
            {
                await service.Delete($"{contentTypeUrl}({instance.Value<string>("Id")})");
            }
        }

        public static async Task<SiteFinityDataFeed<List<T>>> GetAll<T>(ISiteFinityHttpService service, string baseUrl,
            string contentType) where T : class
        {
            var contentTypeUrl = new Uri($"{baseUrl}/{contentType}");

            var isExhusted = false;
            var data = new List<T>();
            var page = 0;
            do
            {
                String url;
                if (String.IsNullOrWhiteSpace(contentTypeUrl.Query))
                {
                    url = $"{contentTypeUrl}?$top=50&$skip={50 * page}";
                }
                else
                {
                    url = $"{contentTypeUrl}&$top=50&$skip={50 * page}";
                }

                var results = await service.GetString(url).FromJson<SiteFinityDataFeed<T[]>>();
                if (results == null || results.Value.Length == 0)
                {
                    isExhusted = true;
                }
                else
                {

                    data.AddRange(results.Value);
                    page++;
                }

            } while (!isExhusted);

            return new SiteFinityDataFeed<List<T>> {Value = data.ToList()};
        }
        
        public static async Task<SiteFinityDataFeed<List<JObject>>> GetContentTypeInstances(ISiteFinityHttpService service, string baseUrl,
            string contentType)
        {
            return await GetAll<JObject>(service, baseUrl, contentType);

        }
        
        public static async Task<SiteFinityDataFeed<List<JObject>>> GetTaxonomyInstances(ISiteFinityHttpService service, string baseUrl,
            string contentType)
        {
            var taxonomies = await GetAll<JObject>(service, baseUrl, "taxonomies");
            
            var taxaId =
                taxonomies.Value
                    .Single(r => String.Equals(r.Value<string>("TaxonName"), contentType, StringComparison.InvariantCultureIgnoreCase))
                    .Value<string>("Id");
            
            var taxonHierarcy = await GetAll<TaxonomyHierarchy>(service, baseUrl, "hierarchy-taxa");
            
            var data = new List<JObject>();
            foreach (var hierarchy in taxonHierarcy.Value.Where(x => String.Equals(x.TaxonomyId, taxaId, StringComparison.InvariantCultureIgnoreCase)))
            {
                data.Add(new JObject(
                        new JProperty("Id", hierarchy.Id),
                        new JProperty("Title", hierarchy.Title)
                    )
                );
            }
            
            return new SiteFinityDataFeed<List<JObject>>
            {
                Value = data
            };
        }
        
        private static async Task RunExtract(ISiteFinityHttpService service, string outputDir, string baseUrl, string contentType)
        {
            var instances = await GetContentTypeInstances(service, baseUrl, contentType);
            File.WriteAllText(Path.Combine(outputDir, $"{contentType.Split('?')[0]}.json"), JsonConvert.SerializeObject(instances, Formatting.Indented));
        }
        
     
        private static async Task<JObject> RunCreate(ISiteFinityHttpService service, ILogger logger, string baseUrl, string contentType, JObject source, Relation[] relations)
        {
            var contentTypeUrl = $"{baseUrl}/{contentType}";
            
            source["PublicationDate"] = DateTime.Now.ToString("O");
            source.InterpolateProperties();
            
            var result = await service.PostData(contentTypeUrl, source).FromJson<JObject>();
            
            if(result.ContainsKey("error"))
                throw new Exception($"Error creating {contentType}: {result["error"].Value<string>("message")}");

            if (!_relatedTypesCache.TryGetValue(contentType.ToLower(), out var cachedValues))
            {
                cachedValues = new List<JObject>();
                _relatedTypesCache[contentType.ToLower()] = cachedValues;
            }
            
            cachedValues.Add(result);
            
            if (relations != null && relations.Length > 0)
            {
                await CreateRelations(service, logger, baseUrl, contentType, result.Value<String>("Id"), relations);
            }
            
            return result;
        }
        
        private static IDictionary<string,List<JObject>> _relatedTypesCache = new Dictionary<string, List<JObject>>();
        
        private static async Task CreateRelations(ISiteFinityHttpService service, ILogger logger, string baseUrl, string sourceContentType, string sourceId, Relation[] relations)
        {
            foreach (var relation in relations)
            {
                if (!_relatedTypesCache.TryGetValue(relation.ContentType.ToLower(), out var relatedData))
                {
                    SiteFinityDataFeed<List<JObject>> response;
                    if (!relation.IsTaxonomy)
                    {
                        response = await GetContentTypeInstances(service, baseUrl, relation.RelatedType.ContentType);
                    }
                    else
                    {
                        response = await GetTaxonomyInstances(service, baseUrl, relation.ContentType);
                    }
                    
                    relatedData = response.Value;
                    _relatedTypesCache[relation.ContentType.ToLower()] = relatedData;
                }

                foreach (var jObject in relatedData)
                {
                    if(!jObject.ContainsKey(relation.Property))
                        logger.LogError(new KeyNotFoundException($"Expected property {relation.Property} on content type {JObject.FromObject(relation.RelatedType).ToString(Formatting.Indented)}"), "Invalid property on relation");

                    var key = jObject.Value<string>(relation.Property);

                    foreach (var value in relation.Values)
                    {
                        if (String.Equals(key, value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var id = jObject.Value<string>("Id");
                            
                            var navProp = !String.IsNullOrWhiteSpace(relation.RelatedType.NavigationProperty)
                                ? relation.RelatedType.NavigationProperty
                                : relation.RelatedType.Type;
                            
                            var refUrl = $"{baseUrl}/{sourceContentType}({sourceId})/{navProp}/$ref";
                            var relatedType = relation.IsTaxonomy ? "taxonomies" : relation.RelatedType.ContentType;
                            var oDataId = $"{baseUrl}/{relatedType}({id})";
                            var response = await service.PostData(refUrl,$"{{ \"@odata.id\": \"{oDataId}\"  }}");

                            try
                            {
                                var result = response.FromJson<JObject>();
                        
                                if(result != null && result.ContainsKey("error"))
                                    throw new Exception($"Error creating relation {value} -> {id} : {relation.Property} : {relation.ContentType} {result["error"].Value<string>("message")}");
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e, "Unable to create relation.");
                            }
    
                        }
                    }
                    
                }
            }
        }
        
        private static Workflow ReadWorkflow(string path)
        {
            var fullPath = Path.GetFullPath(path);

            var data = File.ReadAllText(fullPath);

            return JsonConvert.DeserializeObject<Workflow>(data);
        }

        public static async Task RunWorkflow(ISiteFinityHttpService service, ILogger logger, string baseUrl,
            Workflow workflow, string outputDirectory)
        {
            foreach (var step in workflow.Steps)
            {
                logger.LogInformation($"Running step {step.Action} {step.ContentType}");

                switch (step.Action)
                {
                    case Action.Create:
                    {
                        var created = await RunCreate(service, logger, baseUrl, step.ContentType, step.Data,
                            step.Relates);
                        logger.LogInformation(
                            $"Created {step.ContentType}{Environment.NewLine}{created.ToString(Formatting.Indented)}");
                        break;
                    }

                    case Action.Delete:
                    {
                        await RunDelete(service, baseUrl, step.ContentType);
                        break;
                    }

                    case Action.Extract:
                    {
                        var dir = Directory.CreateDirectory(Path.GetFullPath(outputDirectory));
                        await RunExtract(service, dir.FullName, baseUrl, step.ContentType);
                        logger.LogInformation($"Extracted {step.ContentType}");
                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException($"Unknown action: {step.Action}");
                }

                logger.LogInformation($"Completed step {step.Action} {step.ContentType}");
            }
        }
        
        public static async Task RunWorkflowFromFile(ISiteFinityHttpService service, ILogger logger, string siteFinityBaseUrl, string workflowFile, string outputDirectory)
        {
            var workflow = ReadWorkflow(workflowFile);

            logger.LogInformation($"Read workflow file {workflowFile}");

            await RunWorkflow(service, logger, siteFinityBaseUrl, workflow, outputDirectory);
        }
    }
}
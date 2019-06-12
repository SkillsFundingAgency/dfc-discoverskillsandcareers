using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.SupportApp.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    public static class SiteFinityWorkflowRunner
    {
        public static async Task RunDelete(ISiteFinityHttpService service, string contentType)
        {  
            var instances = await service.GetAll<JObject>(contentType);

            foreach (var instance in instances)
            {
                await service.Delete($"{contentType}({instance.Value<string>("Id")})");
            }
        }

        
        private static async Task RunExtract(ISiteFinityHttpService service, string outputDir, string contentType)
        {
            var instances = await service.GetAll<JObject>(contentType);
            File.WriteAllText(Path.Combine(outputDir, $"{contentType.Split('?')[0]}.json"), JsonConvert.SerializeObject(instances, Formatting.Indented));
        }
        
     
        private static async Task<JObject> RunCreate(ISiteFinityHttpService service, ILogger logger, string contentType, JObject source, Relation[] relations)
        {
            var contentTypeUrl = $"{contentType}";
            
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
                await CreateRelations(service, logger, contentType, result.Value<String>("Id"), relations);
            }
            
            return result;
        }
        
        private static IDictionary<string,List<JObject>> _relatedTypesCache = new Dictionary<string, List<JObject>>();
        
        private static async Task CreateRelations(ISiteFinityHttpService service, ILogger logger, string sourceContentType, string sourceId, Relation[] relations)
        {
            foreach (var relation in relations)
            {
                if (!_relatedTypesCache.TryGetValue(relation.ContentType.ToLower(), out var relatedData))
                {
                    List<JObject> response;
                    if (!relation.IsTaxonomy)
                    {
                        response = await service.GetAll<JObject>(relation.RelatedType.ContentType);
                    }
                    else
                    {
                        var data = await service.GetTaxonomyInstances(relation.ContentType);
                        response = data.Select(JObject.FromObject).ToList();
                    }
                    
                    relatedData = response;
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
                            
                            var refUrl = $"{sourceContentType}({sourceId})/{navProp}/$ref";
                            var relatedType = relation.IsTaxonomy ? "taxonomies" : relation.RelatedType.ContentType;
                            var oDataId = $"{relatedType}({id})";
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

        public static async Task RunWorkflow(ISiteFinityHttpService service, ILogger logger,
            Workflow workflow, string outputDirectory)
        {
            foreach (var step in workflow.Steps)
            {
                logger.LogInformation($"Running step {step.Action} {step.ContentType}");

                switch (step.Action)
                {
                    case Action.Create:
                    {
                        var created = await RunCreate(service, logger, step.ContentType, step.Data, step.Relates);
                        logger.LogInformation($"Created {step.ContentType}{Environment.NewLine}{created.ToString(Formatting.Indented)}");
                        break;
                    }

                    case Action.Delete:
                    {
                        await RunDelete(service, step.ContentType);
                        break;
                    }

                    case Action.Extract:
                    {
                        var dir = Directory.CreateDirectory(Path.GetFullPath(outputDirectory));
                        await RunExtract(service, dir.FullName, step.ContentType);
                        logger.LogInformation($"Extracted {step.ContentType}");
                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException($"Unknown action: {step.Action}");
                }

                logger.LogInformation($"Completed step {step.Action} {step.ContentType}");
            }
        }
        
        public static async Task RunWorkflowFromFile(ISiteFinityHttpService service, ILogger logger, string workflowFile, string outputDirectory)
        {
            var workflow = ReadWorkflow(workflowFile);

            logger.LogInformation($"Read workflow file {workflowFile}");

            await RunWorkflow(service, logger, workflow, outputDirectory);
        }
    }
}
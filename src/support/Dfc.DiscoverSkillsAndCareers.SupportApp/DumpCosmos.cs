using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.SupportApp.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    public class DumpCosmos
    {
        [Verb("dump-cosmos", HelpText = "Extracts the cosmos DB DYSAC.")]
        public class Options : AppSettings
        {
            public CosmosSettings Cosmos { get; set; } = new CosmosSettings();

            [Option('o', "outputDir", Required = false, HelpText = "The output directory of any extracted data.")]
            public string OutputDirectory { get; set; }
        }

        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            var logger = services.GetService<ILogger<DumpCosmos>>();

            try
            {
                var configuration = services.GetService<IConfiguration>();
                configuration.Bind(opts);

                var dir = Directory.CreateDirectory(opts.OutputDirectory);

                var client = new DocumentClient(new Uri(opts.Cosmos.Endpoint), opts.Cosmos.Key);

                var shortQuestionSets = 
                    GetAll<QuestionSet>(logger, client, opts.Cosmos.DatabaseName, "QuestionSets", "short_questionset", dir.FullName,
                    qb => qb.Where(q => q.QuestionSetVersion == "short-201901-11" && q.IsCurrent).OrderByDescending(q => q.Version))
                        .GetAwaiter()
                        .GetResult();
                
                var filterQuestionSets = 
                    GetAll<QuestionSet>(logger, client, opts.Cosmos.DatabaseName, "QuestionSets", "filtered_questionset", dir.FullName,
                            qb => qb.Where(q => q.PartitionKey == "filtered-default" && q.IsCurrent).OrderByDescending(q => q.Version))
                        .GetAwaiter()
                        .GetResult();
                
                foreach (var questionSet in filterQuestionSets.Union(shortQuestionSets))
                {
                    GetAll<Question>(logger, client, opts.Cosmos.DatabaseName, "Questions", questionSet.QuestionSetVersion, dir.FullName, 
                        qb => qb.Where(q => q.PartitionKey == questionSet.QuestionSetVersion))
                        .GetAwaiter()
                        .GetResult();
                }

                //var traits = 
                    GetAll<Trait>(logger, client, opts.Cosmos.DatabaseName, "QuestionSets", "traits", dir.FullName,
                            qb => qb.Where(q => q.PartitionKey == "traits"))
                        .GetAwaiter()
                        .GetResult();
                
                //var jobCategories = 
                    GetAll<JobCategory>(logger, client, opts.Cosmos.DatabaseName, "QuestionSets", "job_categories", dir.FullName,
                            qb => qb.Where(q => q.PartitionKey == "job-categories"))
                        .GetAwaiter()
                        .GetResult();
                
                return SuccessFailCode.Succeed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured ");
                return SuccessFailCode.Fail;
            }

        }

        public static async Task<List<T>> GetAll<T>(ILogger logger, DocumentClient client, string dbName, string collection, string fileId,
            string outputDirectory, Func<IQueryable<T>,IQueryable<T>> queryBuilder)
        {
            IQueryable<T> collectionQuery = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(dbName, collection),
                new FeedOptions
                {
                    EnableCrossPartitionQuery = true,
                    MaxItemCount = 100
                }
            );

            collectionQuery = queryBuilder(collectionQuery);
            
            var result = new List<T>();
            using (var query = collectionQuery.AsDocumentQuery())
            {
                while (query.HasMoreResults)
                {
                    var docs = await query.ExecuteNextAsync<T>();
                    result.AddRange(docs.ToArray());
                    
                }
            }

            var path = Path.Combine(outputDirectory, $"{collection}_{fileId}_extract.json");
            File.WriteAllText(path,JsonConvert.SerializeObject(result));
            logger.LogInformation($"Wrote {collection} {fileId}");

            return result;
        }
    }
}
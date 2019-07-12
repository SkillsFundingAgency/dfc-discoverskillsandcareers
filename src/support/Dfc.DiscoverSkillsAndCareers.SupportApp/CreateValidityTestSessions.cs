using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using CsvHelper;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.SupportApp.Models;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    public static class CreateValidityTestSessions
    {
        [Verb("validity-sessions", HelpText = "Creates the validity test sessions in the Cosmos DB instance.")]
        public class Options 
        {
            public CosmosSettings Cosmos { get; set; } = new CosmosSettings();

            [Option('f', "csvFile", Required = false, HelpText = "The output CSV file")]
            public string FileName { get; set; } = "userSessions";
            

            [Option('n', "sessionCount", Required = false, HelpText = "Number of Sessions to create")]
            public int NumberOfSessions { get; set; } = 300;

            [Option('v', "version", Required = false, HelpText = "Question Version Key typically of the format {type}-{YYYYMM}")]
            public string QuestionVersionKey { get; set; }
            
            [Option('r', "read", Default = true, HelpText = "Output the completed sessions")]
            public bool ShouldRead { get; set; }
        
        }

        public class UserSessionEnty
        {
            public string SessionId { get; set; }
            public string UserName { get; set; }
        }

        private static UserSession CreateSession(string questionSetVersion, int maxQuestions, string languageCode = "en")
        {
            string partitionKey = DateTime.Now.ToString("yyyyMM");
            string salt = Guid.NewGuid().ToString();
            return new UserSession()
            {
                UserSessionId = SessionIdHelper.GenerateSessionId(salt) + "_shl",
                Salt = salt,
                StartedDt = DateTime.Now,
                LanguageCode = languageCode,
                PartitionKey = partitionKey,
                AssessmentState = new AssessmentState(questionSetVersion, maxQuestions)
            };
        }

        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            try
            {
                var configuration = services.GetService<IConfiguration>();
                configuration.Bind(opts);

                return !opts.ShouldRead ? WriteSessions(opts) : ReadSessions(opts);
            }
            catch(Exception ex)
            {
                 Console.ForegroundColor = ConsoleColor.Red;
                 Console.WriteLine($"An Error ocurred loading while creating validity test sessions: {opts.Cosmos.Endpoint} - {opts.Cosmos.DatabaseName} -- {ex.Message}");   
                 Console.ForegroundColor = ConsoleColor.White;
                 return SuccessFailCode.Fail;
            }
        }

        private static SuccessFailCode ReadSessions(Options opts)
        {
            var client = new DocumentClient(new Uri(opts.Cosmos.Endpoint), opts.Cosmos.Key);
            var uri = UriFactory.CreateDocumentCollectionUri(opts.Cosmos.DatabaseName, "UserSessions");
            FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true, MaxItemCount = 100 };

            var result = new List<dynamic>();
            using (var query = client.CreateDocumentQuery(uri, feedOptions, new SqlQuerySpec
                {
                    QueryText = "select * from c where CONTAINS(c.id, \"_shl\") and c.isComplete",
                    Parameters = new SqlParameterCollection()
                }).AsDocumentQuery())
            {
                while (query.HasMoreResults)
                {
                    var results = query.ExecuteNextAsync().GetAwaiter().GetResult();
                    result.AddRange(results.ToList());
                }
            }
            
            File.WriteAllText(opts.FileName + ".json", JsonConvert.SerializeObject(result, Formatting.Indented));

            return SuccessFailCode.Succeed;
        }

        private static SuccessFailCode WriteSessions(Options opts)
        {
            var client = new DocumentClient(new Uri(opts.Cosmos.Endpoint), opts.Cosmos.Key);

            var title = opts.QuestionVersionKey.Split('-').Last();
            var questionSetRepository =
                new QuestionSetRepository(client, new OptionsWrapper<CosmosSettings>(opts.Cosmos), new MemoryCache(new MemoryCacheOptions()));
            var sessionRepository =
                new UserSessionRepository(client, new OptionsWrapper<CosmosSettings>(opts.Cosmos), new MemoryCache(new MemoryCacheOptions()));

            var questionSet = questionSetRepository.GetLatestQuestionSetByTypeAndKey("short", title)
                .GetAwaiter().GetResult();

            using (var fs = File.OpenWrite(opts.FileName + ".csv"))
            using (var sw = new StreamWriter(fs))
            using (var csv = new CsvWriter(sw))
            {
                csv.WriteHeader<UserSessionEnty>();
                csv.NextRecord();

                for (var i = 0; i < opts.NumberOfSessions; i++)
                {
                    var session = CreateSession(questionSet.QuestionSetVersion, questionSet.MaxQuestions);
                    Console.WriteLine($"Creating User Session: {i} {session.UserSessionId}");

                    sessionRepository.CreateUserSession(session).GetAwaiter().GetResult();

                    csv.WriteRecord(new UserSessionEnty {SessionId = session.UserSessionId, UserName = ""});
                    csv.NextRecord();
                }
            }

            return SuccessFailCode.Succeed;
        }
    }
}
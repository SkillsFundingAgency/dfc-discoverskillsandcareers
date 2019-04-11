using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using CsvHelper;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Dfc.DiscoverSkillsAndCareers.SupportApp.Program;
using Microsoft.Azure.Documents.Client;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    

    public static class CreateValidityTestSessions
    {
        [Verb("create-validity-sessions", HelpText = "Creates the validity test sessions in the Cosmos DB instance.")]
        public class Options 
        {
            public CosmosSettings Cosmos { get; set; } = new CosmosSettings();

            [Option('f', "csvFile", Required = false, HelpText = "The output CSV file")]
            public string CsvFile { get; set; } = "userSessions.csv";
            

            [Option('n', "sessionCount", Required = true, HelpText = "Number of Sessions to create")]
            public int NumberOfSessions { get; set; } = 300;

            [Option('v', "version", Required = true, HelpText = "Question Version Key typically of the format {type}-{YYYYMM}")]
            public string QuestionVersionKey { get; set; }
        
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
                QuestionSetVersion = questionSetVersion,
                MaxQuestions = maxQuestions,
                CurrentQuestion = 1
            };
        }

        public static SuccessFailCode Execute(IConfiguration configuration, Options opts)
        {
            try
            {
                configuration.Bind(opts);

                var client = new DocumentClient(new Uri(opts.Cosmos.Endpoint), opts.Cosmos.Key);

                var title = opts.QuestionVersionKey.Split('-').Last();
                var questionRepository = new QuestionRepository(client, new OptionsWrapper<CosmosSettings>(opts.Cosmos));
                var questionSetRepository = new QuestionSetRepository(client, new OptionsWrapper<CosmosSettings>(opts.Cosmos));
                var sessionRepository = new UserSessionRepository(client, new OptionsWrapper<CosmosSettings>(opts.Cosmos));
                
                var questionSet = questionSetRepository.GetCurrentQuestionSet("short", title).GetAwaiter().GetResult();
                
                using(var fs = File.OpenWrite(opts.CsvFile))
                using(var sw = new StreamWriter(fs))
                using(var csv = new CsvWriter(sw))
                {
                    csv.WriteHeader<UserSessionEnty>();
                    csv.NextRecord();

                    for(var i = 0; i < opts.NumberOfSessions; i++)
                    {
                        
                        var session = CreateSession(questionSet.QuestionSetVersion, questionSet.MaxQuestions);
                        Console.WriteLine($"Creating User Session: {i} {session.UserSessionId}");
                        
                        sessionRepository.CreateUserSession(session).GetAwaiter().GetResult();

                        csv.WriteRecord(new UserSessionEnty { SessionId = session.UserSessionId, UserName = "" });
                        csv.NextRecord();
                    }
                }


                return SuccessFailCode.Succeed;
            }
            catch(Exception ex)
            {
                 Console.ForegroundColor = ConsoleColor.Red;
                 Console.WriteLine($"An Error ocurred loading while creating validity test sessions: {opts.Cosmos.Endpoint} - {opts.Cosmos.DatabaseName} -- {ex.Message}");   
                 Console.ForegroundColor = ConsoleColor.White;
                 return SuccessFailCode.Fail;
            }
        }
    }
}
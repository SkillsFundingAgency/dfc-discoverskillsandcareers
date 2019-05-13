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
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    

    public static class LoadQuestions
    {
        [Verb("load-questions", HelpText = "Loads questions to the Cosmos DB instance.")]
        public class Options
        {
            public CosmosSettings Cosmos { get; set; } = new CosmosSettings();

            [Option('f', "csvFile", Required = true, HelpText = "The source CSV file")]
            public string CsvFile { get; set; }

            [Option('v', "version", Required = true, HelpText = "Question Version Key typically of the format {type}-{YYYYMM}")]
            public string QuestionVersionKey { get; set; }
        }

        private class QuestionStatement
        {
            public string Statement {get; set;}
            public string Trait { get; set; }

            public int IsFlipped { get; set; }
        }


        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            try
            {
                var configuration = services.GetService<IConfiguration>();
                configuration.Bind(opts);

                var client = new DocumentClient(new Uri(opts.Cosmos.Endpoint), opts.Cosmos.Key);

                var title = opts.QuestionVersionKey.Split('-').Last();
                var questionSetRepository = new QuestionSetRepository(client, new OptionsWrapper<CosmosSettings>(opts.Cosmos));
                var questionSet = new QuestionSet()
                {
                    AssessmentType = "short",
                    Version = 1,
                    Title = title,
                    QuestionSetKey = title.ToLower(),
                    MaxQuestions = 40,
                    LastUpdated = DateTime.Now,
                    PartitionKey = "ncs",
                    IsCurrent = true,
                    QuestionSetVersion = opts.QuestionVersionKey + "-1"
                };
                questionSetRepository.CreateOrUpdateQuestionSet(questionSet).GetAwaiter().GetResult();

                var questionRepository = new QuestionRepository(client, new OptionsWrapper<CosmosSettings>(opts.Cosmos));
                using(var fileStream = File.OpenRead(opts.CsvFile))
                using(var streamReader = new StreamReader(fileStream))
                using(var reader = new CsvReader(streamReader))
                {
                    var questionPartitionKey = questionSet.QuestionSetVersion;
                    var questionNumber = 1;

                    foreach(var question in reader.GetRecords<QuestionStatement>())
                    {
                        var questionId = $"{questionPartitionKey}-{questionNumber}";
                        Console.WriteLine($"Creating question id: {questionId}");    

                        var doc = questionRepository.CreateQuestion(new Question { 
                            IsNegative = question.IsFlipped == 1,  
                            Order = questionNumber,
                            QuestionId = questionId,
                            TraitCode = question.Trait.ToUpper(),
                            PartitionKey = questionPartitionKey,
                            Texts = new [] {  
                                new QuestionText { LanguageCode = "EN", Text = question.Statement }
                            }
                        }).GetAwaiter().GetResult();

                        questionNumber++;
                    }
                }
                return SuccessFailCode.Succeed;
            }
            catch(Exception ex)
            {
                 Console.ForegroundColor = ConsoleColor.Red;
                 Console.WriteLine($"An Error ocurred loading while loading questions to: {opts.Cosmos.Endpoint} - {opts.Cosmos.DatabaseName} -- {ex.Message}");   
                 Console.ForegroundColor = ConsoleColor.White;
                 return SuccessFailCode.Fail;
            }
        }
    }
}
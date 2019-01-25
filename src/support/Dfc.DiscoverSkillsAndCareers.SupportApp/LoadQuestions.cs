using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using CsvHelper;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Configuration;

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

            [Option('v', "version", Required = true, HelpText = "Question Version Key typically of the format YYYYMM")]
            public string QuestionVersionKey { get; set; }
        }

        private class QuestionStatement
        {
            public string Statement {get; set;}
            public string Trait { get; set; }

            public int IsFlipped { get; set; }
        }


        public static SuccessFailCode Execute(IConfiguration configuration, Options opts)
        {
            try
            {
                configuration.Bind(opts);

                var questionRepository = new QuestionRepository(opts.Cosmos);
                using(var fileStream = File.OpenRead(opts.CsvFile))
                using(var streamReader = new StreamReader(fileStream))
                using(var reader = new CsvReader(streamReader))
                {
                    var questionPartitionKey = opts.QuestionVersionKey;
                    var questionNumber = 1;

                    foreach(var question in reader.GetRecords<QuestionStatement>())
                    {
                        Console.WriteLine($"Creating question {questionPartitionKey} - {questionNumber}");    

                        var doc = questionRepository.CreateQuestion(new Question { 
                            IsNegative = question.IsFlipped == 1,  
                            Order = questionNumber,
                            QuestionId = questionPartitionKey + "-" + questionNumber,
                            TraitCode = question.Trait.ToUpper(),
                            PartitionKey = questionPartitionKey,
                            Texts = new List<QuestionText> {  
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
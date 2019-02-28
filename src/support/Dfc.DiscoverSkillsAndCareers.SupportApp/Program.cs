using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CommandLine;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Configuration;
using CsvHelper;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    [Verb("load-questions", HelpText = "Loads questions to the Cosmos DB instance.")]
    public class QuestionLoadingOptions
    {
        public CosmosSettings Cosmos { get; set; } = new CosmosSettings();

        [Option('f', "csvFile", Required = true, HelpText = "The source CSV file")]
        public string CsvFile { get; set; }

        [Option('v', "version", Required = true, HelpText = "Question Version Key typically of the format YYYYMM")]
        public string QuestionVersionKey { get; set; }
    }

    public class QuestionStatement
    {
        public string Statement {get; set;}
        public string Trait { get; set; }

        public int IsFlipped { get; set; }
    }


    class Program
    {
        public enum SuccessFailCode {
            Succeed = 0,
            Fail = -1
        }

        private static SuccessFailCode LoadQuestions(IConfiguration configuration, QuestionLoadingOptions opts)
        {
            try
            {
                configuration.Bind(opts);

                var title = opts.QuestionVersionKey.Split('-').Last();
                var questionSetRepository = new QuestionSetRepository(Options.Create(opts.Cosmos));
                var questionSet = new QuestionSet()
                {
                    AssessmentType = "short",
                    Version = 1,
                    Title = title,
                    TitleLowercase = title.ToLower(),
                    MaxQuestions = 40,
                    LastUpdated = DateTime.Now,
                    PartitionKey = "ncs",
                    QuestionSetVersion = opts.QuestionVersionKey + "-1"
                };
                questionSetRepository.CreateQuestionSet(questionSet).GetAwaiter().GetResult();

                var questionRepository = new QuestionRepository(Options.Create(opts.Cosmos));
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

        static void Main(string[] args)
        {
            Console.WriteLine($"{Directory.GetCurrentDirectory()}");

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json")
                .Build();

            Parser.Default.ParseArguments<QuestionLoadingOptions>(args)
                         .MapResult(
                             (QuestionLoadingOptions opts) => LoadQuestions(config, opts),
                             errs => {
                                 Console.WriteLine(errs.Aggregate("", (s,e) => s += e.ToString() + Environment.NewLine));
                                 return SuccessFailCode.Fail;
                             });

                                     
        }   
    }
}

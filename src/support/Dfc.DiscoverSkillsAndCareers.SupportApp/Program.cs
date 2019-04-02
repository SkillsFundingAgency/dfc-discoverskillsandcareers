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
using Microsoft.Azure.Documents.Client;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{Directory.GetCurrentDirectory()}");

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json")
                .Build();

            Parser.Default.ParseArguments<LoadQuestions.Options, CreateValidityTestSessions.Options>(args)
                         .MapResult(
                             (LoadQuestions.Options opts) => {
                                 var client = new DocumentClient(new Uri(opts.Cosmos.Endpoint), opts.Cosmos.Key);
                                 return LoadQuestions.Execute(config, client, opts);
                             },
                             (CreateValidityTestSessions.Options opts) => {
                                 var client = new DocumentClient(new Uri(opts.Cosmos.Endpoint), opts.Cosmos.Key);
                                 return CreateValidityTestSessions.Execute(config, client, opts);
                             },
                             errs => {
                                 Console.WriteLine(errs.Aggregate("", (s,e) => s += e.ToString() + Environment.NewLine));
                                 return SuccessFailCode.Fail;
                             });

                                     
        }   
    }
}

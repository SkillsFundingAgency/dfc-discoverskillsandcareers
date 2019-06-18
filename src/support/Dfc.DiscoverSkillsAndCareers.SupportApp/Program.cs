using System;
using System.IO;
using System.Linq;
using CommandLine;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Microsoft.Extensions.Configuration;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.SupportApp.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    
    class Program
    {
        static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log.txt")
                .WriteTo.ColoredConsole()
                .CreateLogger();
            
            Console.WriteLine($"{Directory.GetCurrentDirectory()}");
            
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger>();
            var result = Parser.Default
                .ParseArguments<LoadQuestions.Options, CreateValidityTestSessions.Options, Cms.Options, FilteringQuestionsMappingCreator.Options>(args)
                .MapResult(
                    (LoadQuestions.Options opts) => LoadQuestions.Execute(serviceProvider, opts),
                    (CreateValidityTestSessions.Options opts) => CreateValidityTestSessions.Execute(serviceProvider, opts),
                    (Cms.Options opts) => Cms.Execute(serviceProvider, opts),
                    (FilteringQuestionsMappingCreator.Options opts) => FilteringQuestionsMappingCreator.Execute(serviceProvider, opts),
                             errs => {
                                 logger.LogError(errs.Aggregate("", (s,e) => s += e.ToString() + Environment.NewLine));
                                 return SuccessFailCode.Fail;
                             });

            return (int)result;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json")
                .Build();

            
            services.AddLogging(l => l.AddSerilog())
                .AddSingleton<SiteFinityHttpService>()
                .AddSingleton<Cms>();
                

            services.AddSingleton<IConfiguration>(config);
            services.Configure<AppSettings>(config.GetSection("AppSettings"));
            services.Configure<CosmosSettings>(config.GetSection("Cosmos"));
            
            services.AddTransient<ISiteFinityHttpService, SiteFinityHttpService>();
        }
    }
}

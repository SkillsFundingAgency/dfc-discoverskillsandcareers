using System;
using System.Net;
using System.Reflection;
using System.Transactions;
using CommandLine;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.Repositories;
using Dfc.DiscoverSkillsAndCareers.SupportApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    public class Cms
    {
        
        [Verb("run-cms", HelpText = "Pushes the content to site finity.")]
        public class Options : AppSettings
        {
            [Option('f', "workflowFile", Required = true, HelpText = "The file containing the sitefinity workflow to be executed.")]
            public string WorkflowFile { get; set; }
            
            [Option('o', "outputDir", Required = false, HelpText = "The output directory of any extracted data.")]
            public string OutputDirectory { get; set; }
            
        }
        

        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            var logger = services.GetService<ILogger<Cms>>();
            
            try
            {
                var configuration = services.GetService<IConfiguration>();
                configuration.GetSection("AppSettings").Bind(opts);
                
                var siteFinityService = services.GetService<ISiteFinityHttpService>();

                SiteFinityWorkflowRunner.RunWorkflowFromFile(siteFinityService, logger, opts.WorkflowFile, opts.OutputDirectory).GetAwaiter().GetResult();
                
                return SuccessFailCode.Succeed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured loading cms content");
                return SuccessFailCode.Fail;
            }
        }

    }
}
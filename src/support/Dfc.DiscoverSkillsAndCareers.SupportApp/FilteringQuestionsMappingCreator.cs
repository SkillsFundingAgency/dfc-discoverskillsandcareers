using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Dfc.DiscoverSkillsAndCareers.Models;
using Dfc.DiscoverSkillsAndCareers.SupportApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    
    public class FilteringQuestionsMappingCreator
    {
        [Verb("create-filtering-questions-mapping", HelpText = "Creates the filtering question mappings for DYSAC.")]
        public class Options : AppSettings
        {
            
            [Option('o', "outputDir", Required = false, HelpText = "The output directory of any extracted data.")]
            public string OutputDirectory { get; set; }


        }

        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            var logger = services.GetService<ILogger<FilteringQuestionsMappingCreator>>();

            try
            {
                var configuration = services.GetService<IConfiguration>();
                configuration.GetSection("AppSettings").Bind(opts);
                var sitefinity = services.GetService<ISiteFinityHttpService>();

//                var questionSets =
//                    sitefinity.GetAll<SiteFinityFilteringQuestionSet>("filteringquestionsets").GetAwaiter().GetResult();
//
//                File.WriteAllText(Path.Combine(opts.OutputDirectory, "questionsets.json"), JsonConvert.SerializeObject(questionSets, Formatting.Indented));
//                
//                var questionSets = JsonConvert.DeserializeObject<List<SiteFinityFilteringQuestionSet>>(File.ReadAllText(Path.Combine(opts.OutputDirectory, "questionsets.json")));

//                var jobProfiles = 
//                    sitefinity.GetAll<SiteFinityJobProfile>("jobprofiles?$select=Id,Title,JobProfileCategories&$expand=RelatedSkills&$orderby=Title").GetAwaiter().GetResult();
//                
//                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profiles.json"), JsonConvert.SerializeObject(jobProfiles, Formatting.Indented));

//                var jobProfiles = JsonConvert.DeserializeObject<List<SiteFinityJobProfile>>(File.ReadAllText(Path.Combine(opts.OutputDirectory, "job_profiles.json")));

//                var jobCategories = 
//                    sitefinity.GetTaxonomyInstances("Job Profile Category").GetAwaiter().GetResult();
//
//                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_categories.json"), JsonConvert.SerializeObject(jobCategories, Formatting.Indented));

//                var onetAttributes =
//                    jobProfiles
//                        .SelectMany(o => o.RelatedSkills.Select(s => new
//                            {ONetAttribute = s.Skill, ONetAttributeType = s.ONetAttributeType}))
//                        .Where(s => !onetQuestionLookup.ContainsKey(s.ONetAttribute.ToLower()) && !s.ONetAttributeType.Equals("Knowledge"))
//                        .Distinct();
//
//                File.WriteAllText(Path.Combine(opts.OutputDirectory, "extra_onet_attributes.json"), JsonConvert.SerializeObject(onetAttributes));

//                var jobCategories = JsonConvert.DeserializeObject<List<TaxonomyHierarchy>>(File.ReadAllText(Path.Combine(opts.OutputDirectory, "job_categories.json")));
//
//
//                var categoryQuestions = JobCategorySkillMapper.Map(jobProfiles, jobCategories,
//                    0.75,
//                    0.75);

                var onetQuestionLookup = ReadQuestions(Path.Combine(opts.OutputDirectory, "onet_questions.csv"));
                
                var workflow = new Workflow();
                var steps = new List<WorkflowStep>();
                var questions = new List<string>();
                

                foreach (var attribute in onetQuestionLookup)
                {
                    steps.Add(new WorkflowStep
                    {
                        Action = Action.Create,
                        ContentType = "filteringquestions",
                        Data = new JObject(new object[]
                        {
                            new JProperty("Title", attribute.Key),
                            new JProperty("QuestionText", attribute.Value),
                            new JProperty("Description", ""),
                            new JProperty("IncludeInSitemap", false)
                        }),
                        Relates = new[]
                        {
                            new Relation
                            {
                                RelatedType = new RelationType
                                {
                                    ContentType = "skills", Property = "Title",
                                    Type = "RelatedSkill"
                                },
                                Values = new[] {attribute.Key}
                            },
                        }

                    });

                    questions.Add(attribute.Value);
                }
                

                steps.Add(new WorkflowStep
                {
                    Action = Action.Create,
                    ContentType = "filteringquestionsets",
                    Data = new JObject(new []
                    {
                        new JProperty("Title", "Default"),
                        new JProperty("IncludeInSitemap", false)
                    }),
                    Relates = new []
                    {
                        new Relation
                        {
                            RelatedType = new RelationType
                            {
                                ContentType = "filteringquestions",
                                Type = "Questions",
                                Property = "QuestionText"
                            },
                            Values = questions.ToArray()
                        }
                        
                    }
                });

                workflow.Steps = steps.ToArray();
                
                File.WriteAllText(Path.Combine(opts.OutputDirectory, "cms-filtering-question.json"), JsonConvert.SerializeObject(workflow, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                }));
                
                //SiteFinityWorkflowRunner.RunWorkflow(sitefinity, logger, workflow, opts.OutputDirectory).GetAwaiter().GetResult();
                
                logger.LogInformation("Done");
                
                
                return SuccessFailCode.Succeed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured loading cms content");
                return SuccessFailCode.Fail;
            }
        }

        private static IDictionary<string,string> ReadQuestions(string csvQuestions)
        {    
            var result = new Dictionary<string,string>(StringComparer.InvariantCultureIgnoreCase);
            using(var fs = File.OpenRead(csvQuestions))
            using(var sw = new StreamReader(fs))
            using (var csv = new CsvHelper.CsvParser(sw))
            {
                var row = csv.Read();
                while (row != null)
                {
                    result.Add(row[0].ToLower(), row[1]);
                    row = csv.Read();
                }
            }

            return result;
        }
        
    }
}
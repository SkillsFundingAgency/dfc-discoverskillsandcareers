using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Web;
using CommandLine;
using CsvHelper.Configuration;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp;
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

    public class ONetSkill : IEquatable<ONetSkill>
    {
        public string Title { get; set; }
        
        public string ONetAttributeType { get; set; }
        
        public int Rank { get; set; }
        
        public double ONetRank { get; set; }

        public string Skill => Title?.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1);

        public bool Equals(ONetSkill other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Skill, other.Skill);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ONetSkill) obj);
        }

        public override int GetHashCode()
        {
            return (Skill != null ? Skill.GetHashCode() : 0);
        }
    }

    public class SiteFinityJobProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string WhatyouWillDo { get; set; }

        public string WYDDayToDayTasks { get; set; }
       // public string Skills { get; set; }
        public string Overview { get; set; }

        public string WorkingHoursPatternsAndEnvironment { get; set; }

        public Guid[] JobProfileCategories { get; set; }
        
        public ONetSkill[] RelatedSkills { get; set; }

        public IEnumerable<ONetSkill> Skills =>
            RelatedSkills.Take(8).Where(o =>
                o.Skill != "Critical Thinking"
                && o.Skill != "Active Listening"
                && o.Skill != "Reading Comprehension"
                && o.Skill != "Idea Generation and Reasoning Abilities"
                && o.Skill != "Attentiveness"
                && o.Skill != "Attention to Detail");
    }
    
    public class JobProfileCategory
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
    }
    
    public class Attribute
    {
        public string ONetAttribute { get; set; }
        public string ONetAttributeType { get; set; }
        public double CompositeRank { get; set; }
        public double ONetRank { get; set; }
        public double NcsRank { get; set; }
        public int TotalProfilesWithSkill { get; set; }
        public double PercentageProfileWithSkill { get; set; }
        public HashSet<string> ProfilesWithoutSkill { get; set; }
        
    }
        
    public class JobCategoryAttributes
    {
        public int JobCategoryProfileCount { get; set; }
            
        public Attribute[] Attributes { get; set; }
    }


    
    public class FilteringQuestionsMappingCreator
    {
        [Verb("create-filtering-questions-mapping", HelpText = "Creates the filtering question mappings for DYSAC.")]
        public class Options : AppSettings
        {
            public class JobProfileMappingSheet
            {
                public string Id = "1OrM2UH8eVClFiXH6L3_Geqs-q83yigHKHdBOhOftQuU";
                public string FilteringQuestionsRange = "Filtering Questions!A1:B";
            }

            public class ONetQuestionSheet
            {
                public string Id = "1OrM2UH8eVClFiXH6L3_Geqs-q83yigHKHdBOhOftQuU";
                public string OnetQuestionsRange = "Filtering Questions!A1:B";
            }

            
            [Option('o', "outputDir", Required = false, HelpText = "The output directory of any extracted data.")]
            public string OutputDirectory { get; set; }

            public string SiteFinityApiUrl => $"{SiteFinityApiUrlbase}/api/{SiteFinityApiWebService}";
            public double RemoveBottomPercentage { get; set; } = 0.0;

            public JobProfileMappingSheet JobProfileSheet = new JobProfileMappingSheet();
            
            public ONetQuestionSheet ONetSheet = new ONetQuestionSheet();


        }

        public static SuccessFailCode Execute(IServiceProvider services, Options opts)
        {
            var logger = services.GetService<ILogger<FilteringQuestionsMappingCreator>>();

            try
            {
                var configuration = services.GetService<IConfiguration>();
                configuration.GetSection("AppSettings").Bind(opts);
                var sitefinityService = services.GetService<ISiteFinityHttpService>();

                var jobProfileCategories = SiteFinityWorkflowRunner
                    .GetTaxonomyInstances(sitefinityService, opts.SiteFinityApiUrl, "Job Profile Category")
                    .GetAwaiter()
                    .GetResult()
                    .Value
                    .Select(c => c.ToObject<JobProfileCategory>()).ToDictionary(r => r.Id);
                
                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_categories.json"), JsonConvert.SerializeObject(jobProfileCategories, Formatting.Indented));

                var jobProfiles = SiteFinityWorkflowRunner.GetAll<SiteFinityJobProfile>(sitefinityService,
                        opts.SiteFinityApiUrl,
                        "jobprofiles?$select=Title,Overview,WhatYouWillDo,WYDDayToDayTasks,Skills,JobProfileCategories,WorkingHoursPatternsAndEnvironment&$expand=RelatedSkills&$orderby=Title")
                    .GetAwaiter()
                    .GetResult()
                    .Value;

                File.WriteAllText(Path.Combine(opts.OutputDirectory, "job_profiles.json"),
                    JsonConvert.SerializeObject(jobProfiles, Formatting.Indented));
                
                var jobProfileCategoriesLookup =
                    jobProfiles.ToDictionary(p => p.Title,
                        p => p.JobProfileCategories.Select(c => jobProfileCategories[c]).ToArray(),
                        StringComparer.InvariantCultureIgnoreCase);


                var jobProfileSkills = CreateJobProfileSkillGrouping(jobProfiles);
                
                File.WriteAllText(Path.Combine(opts.OutputDirectory, $"job_profile_onet_complete.json"),
                    JsonConvert.SerializeObject(jobProfileSkills, Formatting.Indented));

                var mostCommon =
                    jobProfileSkills.Where(k => k.Value.ProfileCount / 852.0 >= 0.75)
                        .ToDictionary(r => r.Key, r => r.Value.ProfileCount);

                File.WriteAllText(Path.Combine(opts.OutputDirectory, $"job_profile_onet_in_75pc_of_profiles.json"),
                    JsonConvert.SerializeObject(mostCommon, Formatting.Indented));
                
                var jobCategorySkills = CreateJobCategorySkillGrouping(jobProfiles, jobProfileCategoriesLookup);

                File.WriteAllText(Path.Combine(opts.OutputDirectory, $"job_category_onet_complete.json"),
                    JsonConvert.SerializeObject(jobCategorySkills, Formatting.Indented));
                
                var candidateQuestions = jobCategorySkills.ToDictionary(r => r.Key, r => new JobCategoryAttributes
                {
                    JobCategoryProfileCount = r.Value.JobCategoryProfileCount,
                    Attributes = r.Value.Attributes.Take(4).ToArray()
                });
                
                File.WriteAllText(Path.Combine(opts.OutputDirectory, $"job_category_onet.json"),
                    JsonConvert.SerializeObject(candidateQuestions, Formatting.Indented));
                
                var onetAttributes =
                    jobCategorySkills.Values.SelectMany(v => v.Attributes.Select(r => (r.ONetAttribute, r.ONetAttributeType)))
                        .Select(r => new { ONetAttribute = r.Item1, ONetAttributeType = r.Item2, Question = $"" })
                        .Distinct();
                
                File.WriteAllText(Path.Combine(opts.OutputDirectory, $"active_onet_attributes.json"),
                    JsonConvert.SerializeObject(onetAttributes, Formatting.Indented));
                
                
                var workflow = new Workflow();
                var steps = new List<WorkflowStep>();
                var questions = new List<string>();
                
                var onetQuestionLookup = ReadQuestions(Path.Combine(opts.OutputDirectory, "onet_questions.csv"));

                foreach (var attribute in onetAttributes)
                {
                    var question = onetQuestionLookup[attribute.ONetAttribute];
                    steps.Add(new WorkflowStep
                    {
                        Action = Action.Create,
                        ContentType = "filteringquestions",
                        Data = new JObject(new object[]
                        {
                            new JProperty("Title", attribute.ONetAttribute),
                            new JProperty("QuestionText", question),
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
                                Values = new[] {attribute.ONetAttribute}
                            },
                        }

                    });
                    
                    questions.Add(question);
                }
                
                steps.Add(new WorkflowStep
                {
                    Action = Action.Create,
                    ContentType = "filteringquestionsets",
                    Data = new JObject(new object []
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
                
                logger.LogInformation("Done");
                
                
//                GenerateProfileMappingMatrix(questionVocab, jobProfileVocab, jobProfileCategoriesLookup, opts);
//                
//                logger.LogInformation("Output: Job Profile Mapping Matrix");
                
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
                    result.Add(row[0], row[2]);
                    row = csv.Read();
                }
            }

            return result;
        }

        private static IDictionary<string,JobCategoryAttributes> CreateJobCategorySkillGrouping(List<SiteFinityJobProfile> jobProfiles, 
            IDictionary<string,JobProfileCategory[]> categoryLookup)
        {
            var categorySkillGrouping = 
                jobProfiles
                    .SelectMany(p =>
                        categoryLookup[p.Title].Select(c => new
                        {
                            Category = c.Title,
                            Profile = p
                        }))
                    .GroupBy(x => x.Category)
                    .Select(x =>
                    {
                        var totalProfileCount = (double)x.Count();
                        return new
                        {
                            Category = x.Key,
                            Skills = new JobCategoryAttributes
                            {
                                JobCategoryProfileCount = (int)totalProfileCount,
                                Attributes = x.SelectMany(y => y.Profile.Skills.Select(s => new {Profile = y.Profile, Skill = s}))
                                    .GroupBy(s => s.Skill).Select(s =>
                                    {
                                        var onetRank = s.Average(r => r.Skill.ONetRank);
                                        var ncsRank = s.Average(r => 20 - r.Skill.Rank);
                                        var profileCount = (double) s.Count();
                                        var profilePercentage =
                                            (1.0 - ((totalProfileCount - profileCount) / totalProfileCount)) * 100.0;
                                        return new Attribute
                                        {
                                            ONetAttribute = s.Key.Skill,
                                            ONetAttributeType = s.Key.ONetAttributeType,
                                            CompositeRank =
                                                s.Average(r => ncsRank + (profilePercentage / 5.0)),
                                            ONetRank = onetRank,
                                            NcsRank = ncsRank,
                                            TotalProfilesWithSkill = (int) profileCount,
                                            PercentageProfileWithSkill = profilePercentage,
                                            ProfilesWithoutSkill = new HashSet<string>(
                                                x.Where(p => !p.Profile.Skills.Any(ps => ps.Skill == s.Key.Skill))
                                                    .Select(p => p.Profile.Title))
                                        };

                                    })
                                    .Where(a => a.ProfilesWithoutSkill.Count > 0)
                                    .OrderByDescending(a => a.CompositeRank)
                                    .ToArray()
                            }
                        };
                    })
                    .ToDictionary(x => x.Category, x => x.Skills);

            return categorySkillGrouping;

        }

        public class SkillProfileStats
        {
            public int ProfileCount => Profiles.Count; 
            public IList<string> Profiles { get; set; } = new List<string>();
        }
        
        private static IDictionary<string,SkillProfileStats> CreateJobProfileSkillGrouping(IList<SiteFinityJobProfile> jobProfiles)
        {
            var skills = jobProfiles.SelectMany(p => p.RelatedSkills.Select(s => s.Skill)).Distinct();
            var result = new Dictionary<string,SkillProfileStats>();
            foreach (var skill in skills)
            {
                if (!result.TryGetValue(skill, out var profiles))
                {
                    profiles = new SkillProfileStats();
                    result[skill] = profiles;
                }

                foreach (var jobProfile in jobProfiles)
                {
                    if (jobProfile.RelatedSkills.Any(s =>
                        skill.Equals(s.Skill, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        profiles.Profiles.Add(jobProfile.Title);
                    }
                }
            }

            return result;
        }

        



        
    }
}
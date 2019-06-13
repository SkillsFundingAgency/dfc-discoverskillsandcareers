using System;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.DiscoverSkillsAndCareers.Models;
using Microsoft.Extensions.Options;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.DataRequesters
{
    
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
        
    public class JobCategorySkills
    {
        public int JobCategoryProfileCount { get; set; }
            
        public Attribute[] Attributes { get; set; }
    }
    
    public static class JobProfileAnalyzer
    {
        public static HashSet<string> CalculateCommonSkillsToRemoveByPercentage(IList<SiteFinityJobProfile> jobProfiles, double percentage = 0.75)
        {
            var skills = jobProfiles.SelectMany(p => p.RelatedSkills.Select(s => s.Skill)).Distinct();
            var result = new Dictionary<string,IList<string>>();
            foreach (var skill in skills)
            {
                if (!result.TryGetValue(skill, out var profiles))
                {
                    profiles = new List<String>();
                    result[skill] = profiles;
                }

                foreach (var jobProfile in jobProfiles)
                {
                    if (jobProfile.RelatedSkills.Any(s =>
                        skill.Equals(s.Skill, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        profiles.Add(jobProfile.Title);
                    }
                }
            }

            return new HashSet<string>(result.Where(k => (double) k.Value.Count / (double) jobProfiles.Count >= percentage).Select(r => r.Key));
        }
        
        public static IDictionary<string,JobCategorySkills> CalculateSkillsForJobCategory(List<SiteFinityJobProfile> jobProfiles, 
            IDictionary<string,string[]> categoryLookup, HashSet<string> prominentSkills, int maxAttributeCount = 4)
        {
            var categorySkillGrouping = 
                jobProfiles
                    .SelectMany(p =>
                        categoryLookup[p.Title].Select(c => new
                        {
                            Category = c,
                            Profile = p
                        }))
                    .GroupBy(x => x.Category)
                    .Select(x =>
                    {
                        var totalProfileCount = (double)x.Count();
                        return new
                        {
                            Category = x.Key,
                            Skills = new JobCategorySkills
                            {
                                JobCategoryProfileCount = (int)totalProfileCount,
                                Attributes = x.SelectMany(y => y.Profile.Skills(prominentSkills).Select(s => new {Profile = y.Profile, Skill = s}))
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
                                            CompositeRank = (onetRank + (profilePercentage / 20.0)),
                                            ONetRank = onetRank,
                                            NcsRank = ncsRank,
                                            TotalProfilesWithSkill = (int) profileCount,
                                            PercentageProfileWithSkill = profilePercentage,
                                            ProfilesWithoutSkill = new HashSet<string>(
                                                x.Where(p => !p.Profile.Skills(prominentSkills).Any(ps => ps.Skill == s.Key.Skill))
                                                    .Select(p => p.Profile.Title))
                                        };

                                    })
                                    .Where(a => a.ProfilesWithoutSkill.Count > 0)
                                    .OrderByDescending(a => a.CompositeRank)
                                    .Take(maxAttributeCount)
                                    .ToArray()
                            }
                        };
                    })
                    .ToDictionary(x => x.Category, x => x.Skills);

            return categorySkillGrouping;

        }
    }
    
    public class GetFilteringQuestionData : IGetFilteringQuestionData
    {
        private readonly ISiteFinityHttpService _sitefinity;
        private readonly IOptions<AppSettings> _appSettings;

        public GetFilteringQuestionData(ISiteFinityHttpService sitefinity, IOptions<AppSettings> appSettings)
        {
            _sitefinity = sitefinity;
            _appSettings = appSettings;
        }

        public async Task<List<SiteFinityFilteringQuestion>> GetData(string questionSetId)
        {
            var questions = await _sitefinity.GetAll<SiteFinityFilteringQuestion>($"filteringquestionsets({questionSetId})/Questions");
            var jobCategories = await _sitefinity.GetTaxonomyInstances("Job Profile Category");

            var jobCategoryLookup =
                jobCategories.ToDictionary(r => r.Id, r => r.Title);
            
            var jobProfiles = await _sitefinity.GetAll<SiteFinityJobProfile>("jobprofiles?$select=Id,Title,JobProfileCategories&$expand=RelatedSkills&$orderby=Title");

            var jobProfileCategoriesLookup =
                jobProfiles.ToDictionary(p => p.Title, p => p.JobProfileCategories.Select(c => jobCategoryLookup[c]).ToArray(), StringComparer.InvariantCultureIgnoreCase);
            
            var prominentSkills = JobProfileAnalyzer.CalculateCommonSkillsToRemoveByPercentage(jobProfiles, _appSettings.Value.MaxPercentageOfProfileOccurenceForSkill);

            var skillsByJobCategory =
                JobProfileAnalyzer.CalculateSkillsForJobCategory(jobProfiles, jobProfileCategoriesLookup, prominentSkills, _appSettings.Value.MaxQuestionsPerCategory);
            
            
      
            foreach (var question in questions)
            {
                
            }
            
            return questions;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public static class JobCategoryQuestionBuilder
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

        public static SkillAttribute[] GetSkillAttributes(this IEnumerable<SiteFinityJobProfile> profiles, 
            HashSet<string> prominentSkills, double maxProfileDistributionPercentage)
        {
            var siteFinityJobProfiles = profiles as SiteFinityJobProfile[] ?? profiles.ToArray();
            var totalProfileCount = siteFinityJobProfiles.Length;

            var profilesBySkill =
                siteFinityJobProfiles
                    .SelectMany(y => y.Skills(prominentSkills).Select(s => new {Profile = y, Skill = s}))
                    .GroupBy(s => s.Skill).ToArray();

            return profilesBySkill.Select(s =>
                    {
                        var onetRank = s.Average(r => r.Skill.ONetRank);
                        var ncsRank = s.Average(r => 20 - r.Skill.Rank);
                        var profileCount = (double) s.Count();
                        var profilePercentage =
                            (1.0 - ((totalProfileCount - profileCount) / totalProfileCount)) * 100.0;

                        return new SkillAttribute
                        {
                            ONetAttribute = s.Key.Skill,
                            ONetAttributeType = s.Key.ONetAttributeType,
                            CompositeRank = (onetRank + (profilePercentage / 20.0)),
                            ONetRank = onetRank,
                            NcsRank = ncsRank,
                            TotalProfilesWithSkill = (int) profileCount,
                            PercentageProfileWithSkill = profilePercentage,
                            ProfilesWithSkill = new HashSet<string>(s.Select(p => p.Profile.Title), StringComparer.InvariantCultureIgnoreCase)
                        };

                    })
                    .OrderByDescending(a => a.PercentageProfileWithSkill)
                    .SkipWhile(a => a.PercentageProfileWithSkill < maxProfileDistributionPercentage)
                    .TakeWhile(a => a.PercentageProfileWithSkill > (1 - maxProfileDistributionPercentage))
                    .Take(Math.Min(5, (int) Math.Log(totalProfileCount, 2) - 2))
                    .OrderByDescending(a => a.CompositeRank)
                    .ToArray();
        }
        
        public static IDictionary<Guid,JobCategorySkills> CalculateSkillsForJobCategory(List<SiteFinityJobProfile> jobProfiles, 
            IDictionary<Guid,TaxonomyHierarchy> categoryLookup, HashSet<string> prominentSkills, double maxProfileDistributionPercentage)
        {
            var categorySkillGrouping = 
                jobProfiles
                    .SelectMany(p => p.JobProfileCategories.Select(c => new
                        {
                            Category = c,
                            Profile = p
                        }))
                    .GroupBy(x => x.Category)
                    .Select(profiles =>
                    {
                        var totalProfileCount = (double)profiles.Count();
                        return new
                        {
                            Id = profiles.Key,
                            Skills = new JobCategorySkills
                            {
                                CategoryName = categoryLookup[profiles.Key].Title,
                                JobCategoryProfileCount = (int)totalProfileCount,
                                SkillAttributes = profiles.Select(r => r.Profile).GetSkillAttributes(prominentSkills, maxProfileDistributionPercentage)
                            }
                        };
                    })
                    .ToDictionary(x => x.Id, x => x.Skills);

            return categorySkillGrouping;

        }
        
        public static List<SiteFinityFilteringQuestionJobProfileMapping> GetJobProfileMapping(IList<SiteFinityJobProfile> profiles, SkillAttribute skillAttribute)
        {
            return 
                profiles
                    .Select(p => new SiteFinityFilteringQuestionJobProfileMapping
                    {
                        JobProfile = p.Title, 
                        Included = skillAttribute.ProfilesWithSkill.Contains(p.Title)
                    }).ToList();
        }
        
        public static IList<JobCategoryQuestionsResult> Build(
            List<SiteFinityFilteringQuestion> questions,
            List<SiteFinityJobProfile> jobProfiles,
            List<TaxonomyHierarchy> jobCategories,
            double maxPercentageOfSkillOccurence,
            double maxPercentageOfJobProfileDistribution)
        {

            var jobCategoryIdMap =
                jobCategories.ToDictionary(r => r.Id, r => r);
            
            var jobProfilesByCategory =
                jobProfiles
                    .SelectMany(p => p.JobProfileCategories.Select(c => new {JobCategory = c, Profile = p}))
                    .GroupBy(p => p.JobCategory)
                    .ToDictionary(g => g.Key, g => g.Select(p => p.Profile).ToList());
            
            var prominentSkills = 
                CalculateCommonSkillsToRemoveByPercentage(jobProfiles, maxPercentageOfSkillOccurence);
            
            var skillsByJobCategory =
                CalculateSkillsForJobCategory(jobProfiles, jobCategoryIdMap, prominentSkills, maxPercentageOfJobProfileDistribution);

            var questionLookup =
                questions.ToDictionary(r => r.RelatedSkill.Title, r => r, StringComparer.InvariantCultureIgnoreCase);

            var results = new List<JobCategoryQuestionsResult>();
            foreach (var categorySkills in skillsByJobCategory)
            {
                var result = new JobCategoryQuestionsResult { JobCategory = categorySkills.Value.CategoryName };
                var jobCategoryProfiles = jobProfilesByCategory[categorySkills.Key];
                foreach (var attribute in categorySkills.Value.SkillAttributes)
                {
                    if (!questionLookup.TryGetValue(attribute.ONetAttribute, out var question))
                    {
                        result.AddMessage(LogLevel.Warning, $"No question found for ONetAttribute ({attribute.ONetAttribute})");
                        continue;
                    }

                    var q = new JobCategoryQuestion
                    {
                        Id = question.Id, 
                        Description = question.Description,
                        QuestionText = question.QuestionText,
                        RelatedSkill = question.RelatedSkill,
                        LastUpdated = question.LastUpdated,
                        Title = question.Title,
                        JobProfiles  = GetJobProfileMapping(jobCategoryProfiles, attribute)
                    };
                    
                    result.Questions.Add(q);
                    
                }
                
                results.Add(result);
            }
            
            return results;
        }
    }
}
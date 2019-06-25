using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models;
using Dfc.DiscoverSkillsAndCareers.Models.Extensions;
using Newtonsoft.Json;
using Xunit;

namespace Dfc.UnitTests.CmsTests
{
    public class JobCategorySkillsMappingTests
    {
        private List<SiteFinityJobProfile> JobProfiles { get; }
        private List<TaxonomyHierarchy> JobCategories { get; }
        
        private HashSet<string> ProminentSkills = new HashSet<string>
        {
            "Attention to Detail",
            "Idea Generation and Reasoning Abilities",
            "Attentiveness",
            "Active Listening",
            "Critical Thinking"
        };
        
        public JobCategorySkillsMappingTests()
        {
            JobCategories =
                JsonConvert.DeserializeObject<List<TaxonomyHierarchy>>(File.ReadAllText("Data/job-categories.json"));
            JobProfiles =
                JsonConvert.DeserializeObject<List<SiteFinityJobProfile>>(File.ReadAllText("Data/job-profiles.json"));
        }
        
        [Fact]
        public void CanCalculateSkillsToRemoveByPercentage()
        {
            var result = JobCategorySkillMapper.CalculateCommonSkillsToRemoveByPercentage(JobProfiles);
            Assert.Equal(ProminentSkills, result);
            
        }
        
        [Fact]
        public void CanCalculateTopSkillsForProfiles()
        {
            var category = JobCategories.First(c => c.Title.EqualsIgnoreCase("Animal Care")).Id;
            var profiles = JobProfiles.Where(c => c.JobProfileCategories.Contains(category)).ToArray();
            
            var result = profiles.GetSkillAttributes(ProminentSkills, 0.75);
            
            Assert.Collection(result, 
                s => Assert.Equal("self control",s.ONetAttribute.ToLower()),
                s => Assert.Equal("cooperation",s.ONetAttribute.ToLower()),
                s => Assert.Equal("speaking, verbal abilities",s.ONetAttribute.ToLower()));
        }

        [Theory]
        [InlineData("Animal care", "self control|cooperation|speaking, verbal abilities")]
        public void CanCalculateSkillsForJobCategory(string categoryName, string expected)
        {
            var categoryLookup = JobCategories.ToDictionary(c => c.Id, c => c);
            
            var category = JobCategories.First(c => c.Title.EqualsIgnoreCase(categoryName)).Id;
            var result = 
                JobCategorySkillMapper
                    .CalculateSkillsForJobCategory(JobProfiles, categoryLookup, ProminentSkills, 0.75)[category];
            
            var assertions = expected.Split("|", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new Action<SkillAttribute>(sk => Assert.Equal(s, sk.ONetAttribute.ToLower())))
                .ToArray();
            
            Assert.Collection(result.SkillAttributes, assertions);
        }
        
        [Theory]
        [InlineData("Animal care", "self control|cooperation|speaking, verbal abilities")]
        public void CalculatesSkillsCorrectly(string categoryName, string expected)
        {
            var result = 
                JobCategorySkillMapper
                    .Map(JobProfiles, JobCategories, 0.75, 0.75)
                    .Where(r => r.JobCategory.EqualsIgnoreCase(categoryName))
                    .SelectMany(r => r.SkillMappings.Select(s => s.ONetAttribute));
            
            
            var assertions = expected.Split("|", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new Action<string>(sk => Assert.Equal(s, sk.ToLower())))
                .ToArray();
            
            Assert.Collection(result, assertions);
        }

        [Fact]
        public void ShouldReturnTrueForExistingSkillGetJobProfileSkillMapping()
        {
            var profile = JobProfiles.Where(p => p.Title.EqualsIgnoreCase("Accommodation warden")).ToList();

            var result = JobCategorySkillMapper.GetJobProfileMapping(profile,
                new SkillAttribute {ProfilesWithSkill = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) {"Accommodation warden"}});

            Assert.Single(result, p => p.Included && p.JobProfile == "Accommodation warden");
        }
        
        [Fact]
        public void ShouldReturnFalseForExistingSkillGetJobProfileSkillMapping()
        {
            var profile = JobProfiles.Where(p => p.Title.EqualsIgnoreCase("Accommodation warden")).ToList();

            var result = JobCategorySkillMapper.GetJobProfileMapping(profile,
                new SkillAttribute {ProfilesWithSkill = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) {"Warden"}});

            Assert.Single(result, p => !p.Included && p.JobProfile == "Accommodation warden");
        }
    }
}
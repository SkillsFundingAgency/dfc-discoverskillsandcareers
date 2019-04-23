using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data
{
    public interface IUnderstandMyselfDbContext
    {
        DbSet<UmUserSession> UserSessions { get; set; }
        DbSet<UmAnswer> Answers { get; set; }
        DbSet<UmResultStatement> ResultStatements { get; set; }
        DbSet<UmTraitScore> TraitScores { get; set; }
        DbSet<UmSuggestedJobCategory> SuggestedJobCategories { get; set; }
        DbSet<UmSuggestedJobProfile> SuggestedJobProfiles { get; set; }
        DbSet<UmQuestion> Questions { get; set; }
        DbSet<UmQuestionExcludeJobProfile> QuestionExcludeJobProfiles { get; set; }
        DbSet<UmQuestionSet> QuestionSets { get; set; }
        Task<int> SaveChanges();
    }
}

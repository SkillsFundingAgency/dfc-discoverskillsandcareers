using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data
{
    public class UnderstandMyselfDbContext : DbContext, IUnderstandMyselfDbContext
    {
        public UnderstandMyselfDbContext(DbContextOptions<UnderstandMyselfDbContext> options)
            : base(options)
        {
            Id = System.Guid.NewGuid().ToString();
            
        }

        public string Id { get; set; }

        public DbSet<UmUserSession> UserSessions { get; set; }
        public DbSet<UmAnswer> Answers { get; set; }
        public DbSet<UmResultStatement> ResultStatements { get; set; }
        public DbSet<UmTraitScore> TraitScores { get; set; }
        public DbSet<UmSuggestedJobCategory> SuggestedJobCategories { get; set; }
        public DbSet<UmSuggestedJobProfile> SuggestedJobProfiles { get; set; }
        public DbSet<UmQuestion> Questions { get; set; }
        public DbSet<UmQuestionSet> QuestionSets { get; set; }

        public Task<int> SaveChanges()
        {
            return base.SaveChangesAsync();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UmSuggestedJobProfile>()
                .HasKey(c => new { c.UserSessionId, c.JobCategoryCode });
        }

    }
}

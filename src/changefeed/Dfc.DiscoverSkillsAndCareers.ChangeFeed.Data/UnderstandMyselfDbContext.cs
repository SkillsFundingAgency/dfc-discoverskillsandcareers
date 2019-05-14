using Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.Data
{
    public class UnderstandMyselfDbContext : DbContext, IUnderstandMyselfDbContext
    {
        private readonly string _connectionString;
        public UnderstandMyselfDbContext(DbContextOptions<UnderstandMyselfDbContext> options, string connectionString)
            : base(options)
        {
            Id = System.Guid.NewGuid().ToString();
            _connectionString = connectionString;
        }

        public string Id { get; set; }

        public DbSet<UmUserSession> UserSessions { get; set; }
        public DbSet<UmAnswer> Answers { get; set; }
        public DbSet<UmResultStatement> ResultStatements { get; set; }
        public DbSet<UmTraitScore> TraitScores { get; set; }
        public DbSet<UmSuggestedJobCategory> SuggestedJobCategories { get; set; }
        public DbSet<UmSuggestedJobProfile> SuggestedJobProfiles { get; set; }
        public DbSet<UmQuestion> Questions { get; set; }
        public DbSet<UmQuestionExcludeJobProfile> QuestionExcludeJobProfiles { get; set; }
        public DbSet<UmQuestionSet> QuestionSets { get; set; }

        public Task<int> SaveChanges()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this._connectionString);
            }
        }
    }
}

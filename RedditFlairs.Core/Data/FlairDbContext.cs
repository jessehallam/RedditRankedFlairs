using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using RedditFlairs.Core.Entities;

namespace RedditFlairs.Core.Data
{
    public class FlairDbContext : DbContext
    {
        private static readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        public DbSet<LeaguePosition> LeaguePositions { get; set; }
        public DbSet<RankWeight> RankWeights { get; set; }
        public DbSet<SubReddit> SubReddits { get; set; }
        public DbSet<Summoner> Summoners { get; set; }
        public DbSet<SummonerValidation> SummonerValidations { get; set; }
        public DbSet<TierWeight> TierWeights { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFlair> UserFlairs { get; set; }

        public FlairDbContext(DbContextOptions options) : base(options)
        {
        }

        public Summoner CreateSummoner(int userId, Summoner summoner)
        {
            var user = Users.Single(u => u.Id == userId);
            user.Summoners.Add(summoner);
            SaveChanges();
            return summoner;
        }

        public void DeleteSummoner(Summoner summoner)
        {
            LeaguePositions.RemoveRange(summoner.LeaguePositions);
            SummonerValidations.Remove(summoner.Validation);
            Summoners.Remove(summoner);
            SaveChanges();
        }

        public void RemoveRegisteredSummoner(string summonerId, string region)
        {
            var targets = from summoner in Summoners
                where summoner.Validation.Status == ValidationStatus.Valid
                      && summoner.SummonerId == summonerId
                      && summoner.Region == region
                select summoner;

            Summoners.RemoveRange(targets);
            SaveChanges();
        }

        public bool UserHasSummoner(int userId, string summonerId, string region)
        {
            var query = from summoner in Summoners
                where summoner.User.Id == userId
                      && summoner.SummonerId == summonerId
                      && summoner.Region == region
                select summoner;

            return query.Any();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
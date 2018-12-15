using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using RedditFlairs.Core.Configuration;
using RedditFlairs.Core.Data;
using RedditFlairs.Core.Entities;
using RedditFlairs.Core.Utility;

namespace RedditFlairs.Core.Tasks.Implementations
{
    /// <summary>
    ///     Updates user flairs in the database, corresponding to changes to summoners' league positions.
    /// </summary>
    public class FlairUpdateTask : IAsyncTask
    {
        private readonly FlairUpdateConfig config;
        private readonly FlairDbContext context;
        private readonly RankUtility rankUtility;

        public FlairUpdateTask(FlairDbContext context, RankUtility rankUtility, IOptions<FlairUpdateConfig> config)
        {
            this.context = context;
            this.rankUtility = rankUtility;
            this.config = config.Value;
        }

        public async Task ExecuteAsync()
        {
            if (!config.Enable)
            {
              throw new TaskAbortedException();
            }

            var user = await GetCandidateAsync();

            if (user == null)
            {
                await Task.Delay(config.ExecutionInterval);
                return;
            }

            var leaguePositions = user.Summoners.SelectMany(smn => smn.LeaguePositions).ToList();
            var subReddits = context.SubReddits.ToList();

            foreach (var sub in subReddits)
            {
                await ProcessSubRedditAsync(user, leaguePositions , sub);
            }

            user.FlairsUpdated = DateTimeOffset.Now;
            await context.SaveChangesAsync();
        }

        private async Task<User> GetCandidateAsync()
        {
            var cutoff = DateTimeOffset.Now.AddDays(-1);
            var query =
                from user in context.Users
                where (!user.FlairsUpdated.HasValue || user.FlairsUpdated < cutoff)
                      || user.Summoners.Max(smn => smn.RankUpdatedAt) > user.FlairsUpdated
                orderby user.Id
                select user;

            // var expr = query.ToSql();

            return await query.FirstOrDefaultAsync();
        }

        private async Task ProcessSubRedditAsync(
            User user, 
            ICollection<LeaguePosition> leaguePositions, 
            SubReddit subReddit)
        {
            var userFlair = await context
                .UserFlairs
                .Where(uf => uf.User.Id == user.Id && uf.SubReddit.Name == subReddit.Name)
                .FirstOrDefaultAsync();

            if (userFlair == null)
            {
                userFlair = new UserFlair {SubReddit = subReddit, User = user};
                user.Flairs.Add(userFlair);
            }

            var validPositions = rankUtility.Filter(leaguePositions, subReddit.QueueTypes.Split(','));
            var bestPosition = rankUtility.GetBestPosition(validPositions);

            var cssText = rankUtility.Format(bestPosition, subReddit.CssPattern);
            var flairText = rankUtility.Format(bestPosition, subReddit.FlairPattern);

            if (cssText != userFlair.CssText)
            {
                userFlair.CssText = cssText;
                userFlair.NeedToSend = true;
            }

            if (flairText != userFlair.Text)
            {
                userFlair.Text = flairText;
                userFlair.NeedToSend = true;
            }

            userFlair.UpdatedAt = DateTimeOffset.Now;
        }
    }

    public static class IQueryableExtensions
    {
        private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

        private static readonly FieldInfo QueryModelGeneratorField = QueryCompilerTypeInfo.DeclaredFields.First(x => x.Name == "_queryModelGenerator");

        private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

        private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);
            var modelGenerator = (QueryModelGenerator)QueryModelGeneratorField.GetValue(queryCompiler);
            var queryModel = modelGenerator.ParseQuery(query.Expression);
            var database = (IDatabase)DataBaseField.GetValue(queryCompiler);
            var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
            var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
            var sql = modelVisitor.Queries.First().ToString();

            return sql;
        }
    }
}
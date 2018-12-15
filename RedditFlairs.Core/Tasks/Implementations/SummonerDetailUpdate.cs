using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedditFlairs.Core.Configuration;
using RedditFlairs.Core.Data;
using RedditFlairs.Core.Entities;

namespace RedditFlairs.Core.Tasks.Implementations
{
    public class SummonerDetailUpdate : IAsyncTask
    {
        private readonly SummonerDetailConfig config;
        private readonly FlairDbContext context;

        public SummonerDetailUpdate(FlairDbContext context, IOptions<SummonerDetailConfig> config)
        {
            this.context = context;
            this.config = config.Value;
        }

        public async Task ExecuteAsync()
        {
            if (!config.Enable)
            {
                await Task.Delay(config.ExecutionInterval);
                return;
            }


        }
        
    }
}
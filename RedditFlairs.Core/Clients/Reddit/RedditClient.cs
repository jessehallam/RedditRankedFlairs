using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditFlairs.Core.Clients.Reddit
{
    public class RedditClient : IRedditClient
    {
        private const string BaseUri = "https://oauth.reddit.com";
        private const string ErrorTooManyFlairs = "The API only supports 100 flairs per request.";

        private readonly AccessTokenRequester accessTokenRequester;
        private readonly RedditClientOptions options;
        private readonly RedditApiRequester requester;

        public RedditClient(RedditClientOptions options)
        {
            this.options = options;
            accessTokenRequester = new AccessTokenRequester(options);
            requester = new RedditApiRequester(accessTokenRequester, options);
        }

        public async Task SetFlairsAsync(string sub, ICollection<FlairParameter> flairs)
        {
            if(flairs.Count > 100)
                throw new InvalidOperationException(ErrorTooManyFlairs);

            var url = $"{BaseUri}/r/{sub}/api/flaircsv";
            var data = new[]
            {
                new KeyValuePair<string, string>("flair_csv", CreateBulkFlairCsv(flairs)),
            };

            var result = await requester.PostAsync(url, data);
            var errors = result.SelectMany(token => token["errors"]).ToList();
        }

        private static string CreateBulkFlairCsv(IEnumerable<FlairParameter> flairs)
        {
            return string.Join("\n",flairs.Select(ToFlairLine));
            string ToFlairLine(FlairParameter flair) => $"{flair.Name},{flair.Text ?? ""},{flair.CssClass ?? ""}";
        }
    }
}
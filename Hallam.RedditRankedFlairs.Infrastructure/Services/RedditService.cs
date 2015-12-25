using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Reddit;
using Hallam.RedditRankedFlairs.Services.Reddit;

namespace Hallam.RedditRankedFlairs.Services
{
    public class RedditService : IRedditService
    {
        private const string BaseUri = "https://oauth.reddit.com";
        private readonly RedditWebRequester _requester;

        public RedditService(RedditWebRequester requester)
        {
            _requester = requester;
        }

        public async Task<ICollection<string>> GetSubRedditsAsync(SubRedditKind kind)
        {
            var uri = $"{BaseUri}/subreddits/mine/{kind.ToString().ToLowerInvariant()}";
            var results = new List<string>();
            var after = (string) null;

            while (true)
            {
                var data = after == null
                    ? null
                    : new[]
                    {
                        new KeyValuePair<string, string>("after", after), 
                    };
                var listing = await _requester.GetAsync(uri, data);
                var content = listing["data"];
                after = (string) content["after"];
                results.AddRange(from item in content["children"]
                                 where (string) item["kind"] == "t5"
                                 select (string) item["display_name"]);

                if (after == null) break;
            }
            return results;
        } 

        public async Task<bool> SetUserFlairAsync(string subreddit, string name, string text, string css = null)
        {
            var uri = $"{BaseUri}/r/{subreddit}/api/flair";
            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("api_type", "json"),
                new KeyValuePair<string, string>("name", name),
                new KeyValuePair<string, string>("text", "text")
            };
            if (css != null)
            {
                data.Add(new KeyValuePair<string, string>("css_class", css));
            }

            var result = await _requester.PostAsync(uri, data);
            return !result["errors"].Any();
        }

        public async Task<bool> SetUserFlairsAsync(string subreddit, ICollection<UserFlairParameter> flairs)
        {
            if (flairs.Count > 100)
            {
                throw new InvalidOperationException(
                    "The API does not support assigning more than 100 flairs in one operation");
            }
            var uri = $"{BaseUri}/r/{subreddit}/api/flaircsv";
            var data = new[]
            {
                new KeyValuePair<string, string>("flair_csv", ResolveBulkFlairParameter(flairs))
            };
            var result = await _requester.PostAsync(uri, data);
            return !result.Any(token => token["errors"].Any());
        }

        private static string ResolveBulkFlairParameter(IEnumerable<UserFlairParameter> flairs)
        {
            return string.Join("\n", flairs.Select(f => $"{f.Name},{f.Text ?? ""},{f.CssClass ?? ""}"));
        }
    }
}
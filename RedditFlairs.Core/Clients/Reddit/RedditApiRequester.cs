using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RedditFlairs.Core.Clients.Reddit
{
    public class RedditApiRequester
    {
        private const int MaxRequestsPerMinute = 30;

        private readonly AccessTokenRequester accessTokenRequester;
        private readonly HttpClient httpClient;
        private readonly RedditClientOptions options;
        private readonly object lockObject = new object();

        private DateTimeOffset burstPeriodStart = DateTimeOffset.MinValue;
        private int numRequests = 0;

        public RedditApiRequester(AccessTokenRequester accessTokenRequester, RedditClientOptions options)
        {
            this.accessTokenRequester = accessTokenRequester;
            this.options = options;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
        }

        public async Task<JToken> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            await EnsureRateLimitAsync();

            var request = await CreateRequestAsync(HttpMethod.Post, url);

            if (parameters != null)
            {
                request.Content = new FormUrlEncodedContent(parameters);
            }

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return JToken.Parse(await response.Content.ReadAsStringAsync());
        }

        private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string url)
        {
            var message = new HttpRequestMessage(method, url);
            message.Headers.Add("Authorization", "bearer " + (await accessTokenRequester.GetTokenAsync()).Token);
            return message;
        }

        private async Task EnsureRateLimitAsync()
        {
            Monitor.Enter(lockObject);
            try
            {
                var burstPeriodDur = DateTimeOffset.Now - burstPeriodStart;

                if (burstPeriodDur > TimeSpan.FromMinutes(1))
                {
                    burstPeriodStart = DateTimeOffset.Now;
                    numRequests = 0;
                }

                numRequests++;

                if (numRequests >= MaxRequestsPerMinute)
                {
                    var timeToWait = TimeSpan.FromMinutes(2) - burstPeriodDur;
                    await Task.Delay(timeToWait);
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
        }
    }
}
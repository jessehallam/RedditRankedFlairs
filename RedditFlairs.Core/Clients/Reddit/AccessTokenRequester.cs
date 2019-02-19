using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RedditFlairs.Core.Clients.Reddit
{
    public class AccessTokenRequester
    {
        private const string AccessTokenUrl = "https://www.reddit.com/api/v1/access_token";

        private readonly HttpClient httpClient;
        private readonly SemaphoreSlim lockObject = new SemaphoreSlim(1, 1);
        private readonly RedditClientOptions options;

        private AccessToken accessToken = null;

        public AccessTokenRequester(RedditClientOptions options)
        {
            this.options = options;
            httpClient = new HttpClient(new HttpClientHandler
            {
                Credentials = new NetworkCredential(options.ClientId, options.ClientSecret)
            });
            httpClient.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
        }

        public async Task<AccessToken> GetTokenAsync()
        {
            lockObject.Wait();
            try
            {
                if (accessToken != null && accessToken.Expires > DateTimeOffset.Now.AddMinutes(1))
                    return accessToken;

                var request = new HttpRequestMessage(HttpMethod.Post, AccessTokenUrl);
                request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", options.Username),
                    new KeyValuePair<string, string>("password", options.Password),
                });

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = JObject.Parse(await response.Content.ReadAsStringAsync());
                var newToken = new AccessToken
                {
                    Expires = DateTimeOffset.Now.AddSeconds((double) content["expires_in"]),
                    Token = (string) content["access_token"]
                };
                return accessToken = newToken;
            }
            finally
            {
                lockObject.Release();
            }
        }
    }
}
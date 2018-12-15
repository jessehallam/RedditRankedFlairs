using System;

namespace RedditFlairs.Core.Clients.Reddit
{
    public class AccessToken
    {
        public DateTimeOffset Expires { get; set; }
        public string Token { get; set; }
    }
}
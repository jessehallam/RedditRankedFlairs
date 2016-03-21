using System;
using System.Linq;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Data;
using Hallam.RedditRankedFlairs.Services;

namespace Hallam.RedditRankedFlairs.Jobs
{
    public class ConfirmRegistrationMailJob
    {
        private const string Subject = "Summoner registration successful";

        private readonly ApplicationConfiguration _config;
        private readonly IRedditMessengerService _mailService;
        private readonly IUserService _userService;

        public ConfirmRegistrationMailJob(ApplicationConfiguration config, IRedditMessengerService mailService, IUserService userService)
        {
            _config = config;
            _mailService = mailService;
            _userService = userService;
        }

        public void Execute(int userId, int summonerId)
        {
            ExecuteAsync(userId, summonerId).Wait();
        }

        private async Task ExecuteAsync(int userId, int summonerId)
        {
            var user = await _userService.FindAsync(userId);
            var summoner = user?.Summoners.FirstOrDefault(x => x.Id == summonerId);

            if (user == null) return;
            if (summoner == null) return;

            if (!await _mailService.SendMessageAsync(user.Name, Subject, GetMailMessage(user, summoner)))
            {
                throw new InvalidOperationException("Unable to send confirmation mail message.");
            }
        }

        private string GetMailMessage(User user, Summoner summoner)
        {
            const string messagePattern = @"*I'm a bot whose purpose is to deliver League of Legends flairs.*

----

> **This message is to notify you that your summoner {summonerName} ({region}) has been registered successfully.**

> **Please note that it sometimes takes a little while for me to deliver your flair. Thanks for your patience.**

> **One more thing: Your flair will automatically update when it changes in game. This usually takes about a day.**

----

[Report a problem](https://www.reddit.com/message/compose?to=kivinkujata&subject=Issue with FeralFlair) | 
[Author](https://www.reddit.com/message/compose?to=kivinkujata&subject=Ranked Flairs) |
[GitHub](https://github.com/jessehallam/RedditRankedFlairs) | {version}";

            var message = messagePattern.Replace("{summonerName}", summoner.Name);
            message = messagePattern.Replace("{region}", summoner.Region);
            message = messagePattern.Replace("{version}", _config.FlairBotVersion);
            return message;
        }
    }
}
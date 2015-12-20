using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using Hallam.RedditRankedFlairs.Data;
using Hallam.RedditRankedFlairs.Models;
using Hallam.RedditRankedFlairs.Services;
using Hallam.RedditRankedFlairs.Services.Riot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Summoner = Hallam.RedditRankedFlairs.Riot.Summoner;

namespace Hallam.RedditRankedFlairs.Controllers
{
    [Authorize]
    public class RegistrationController : ApiController
    {
        private static readonly Random Random = new Random();
        private const string ReasonString = "Registration";

        protected IRiotService Riot { get; set; }
        protected ISummonerService Summoners { get; set; }
        protected IUserService Users { get; set; }

        public class ValidationModel
        {
            [Required]
            public string State { get; set; }
        }

        public RegistrationController(IRiotService riotService, ISummonerService summonerService,
            IUserService userService)
        {
            Riot = riotService;
            Summoners = summonerService;
            Users = userService;
        }

        [HttpPost, Route("profile/api/register")]
        public async Task<IHttpActionResult> Register(SummonerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Summoner MUST exist.
                var summoner = await FindSummonerAsync(model.Region, model.SummonerName);

                if (summoner == null)
                    return Conflict("Summoner not found.");

                // Summoner MUST NOT be registered.
                if (await Summoners.IsSummonerRegistered(model.Region, model.SummonerName))
                    return Conflict("Summoner is already registered.");

                var context = new RegistrationContext
                {
                    Region = model.Region,
                    SummonerId = summoner.Id,
                    SummonerName = summoner.Name
                };
                context.GenerateRandomValidationCode();
                return Ok(new
                {
                    code = context.ValidationCode,
                    state = SecurityUtil.Protect(context.ToJson().ToString(), ReasonString)
                });
            }
            catch (RiotHttpException e)
            {
                switch ((int) e.StatusCode)
                {
                    case 500:
                    case 503:
                        return Conflict("Error communicating with Riot (Service unavailable)");
                }
                throw;
            }
        }

        [HttpPost, Route("profile/api/validate")]
        public async Task<IHttpActionResult> Validate(ValidationModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                RegistrationContext context;

                // Try to unpack the state object.
                if (!RegistrationContext.TryGetRegistrationContext(model.State, out context) || !context.IsValid)
                {
                    return BadRequest("Invalid state.");
                }

                // Summoner MUST exist.
                var riotSummoner = await Riot.FindSummonerAsync(context.Region, context.SummonerName);
                if (riotSummoner == null)
                {
                    return Conflict("Summoner not found.");
                }

                // Summoner MUST NOT be registered.
                if (await Summoners.IsSummonerRegistered(context.Region, context.SummonerName))
                {
                    return Conflict("Summoner is already registered.");
                }

                var runePages = await Riot.GetRunePagesAsync(context.Region, context.SummonerId);
                var runePage = runePages.First();

                // Validation Code MUST match.
                if (!string.Equals(runePage.Name, context.ValidationCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    return StatusCode(HttpStatusCode.ExpectationFailed);
                }

                // Create the data entity and associate it with the current user
                var currentUser = await Users.GetUserAsync();
                var currentSummoner = await Summoners.AddSummonerAsync(currentUser, 
                    context.SummonerId, context.Region, context.SummonerName);

                // If the user doesn't have an active summoner, assign the new summoner as active.
                if (currentUser.ActiveSummoner == null)
                    await Summoners.SetActiveSummonerAsync(currentSummoner);
                
                return Ok();
            }
            catch (RiotHttpException e)
            {
                switch ((int) e.StatusCode)
                {
                    case 500:
                    case 503:
                        return Conflict("Error communicating with Riot (Service unavailable)");
                }
                throw;
            }
        }

        private IHttpActionResult Conflict(string message)
        {
            return Content(HttpStatusCode.Conflict, new HttpError(message));
        }

        private Task<Summoner> FindSummonerAsync(string region, string summonerName)
        {
            return CacheUtil.GetItemAsync($"{region}:{summonerName}".ToLowerInvariant(),
                () => Riot.FindSummonerAsync(region, summonerName));
        }

        private class RegistrationContext
        {
            [JsonIgnore]
            public bool IsValid => !string.IsNullOrEmpty(Region) &&
                                   SummonerId != 0 &&
                                   !string.IsNullOrEmpty(SummonerName) &&
                                   !string.IsNullOrEmpty(ValidationCode);

            public string Region { get; set; }
            public int SummonerId { get; set; }
            public string SummonerName { get; set; }
            public string ValidationCode { get; set; }

            public void GenerateRandomValidationCode()
            {
                const int length = 5;
                const string validationChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                char[] chars = new char[length];
                lock (Random)
                {
                    for (int i = 0; i < length; i++)
                    {
                        chars[i] = validationChars[Random.Next(validationChars.Length)];
                    }
                    ValidationCode = new string(chars);
                }
            }

            public JObject ToJson()
            {
                return JObject.FromObject(this);
            }

            public static bool TryGetRegistrationContext(string state, out RegistrationContext context)
            {
                try
                {
                    var contextJson = SecurityUtil.Unprotect(state, ReasonString);
                    context = FromJson(contextJson);
                    return true;
                }
                catch
                {
                    context = null;
                    return false;
                }
            }

            private static RegistrationContext FromJson(string json)
            {
                return JsonConvert.DeserializeObject<RegistrationContext>(json);
            }
        }
    }
}

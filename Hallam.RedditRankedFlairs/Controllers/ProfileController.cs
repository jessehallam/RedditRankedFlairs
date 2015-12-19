using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Hallam.RedditRankedFlairs.Models;
using Hallam.RedditRankedFlairs.Riot;
using Hallam.RedditRankedFlairs.Services;

namespace Hallam.RedditRankedFlairs.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private const string RegistrationContextKey = "registration_context";

        private RegistrationContext Registration
        {
            get
            {
                if (Session[RegistrationContextKey] == null)
                {
                    Session[RegistrationContextKey] = new RegistrationContext();
                }
                return (RegistrationContext)Session[RegistrationContextKey];
            }
        }

        public IRiotService Riot { get; set; }
        public ISummonerService Summoners { get; set; }
        public IUserService Users { get; set; }
        public ProfileViewModel ViewModel { get; set; }

        public ProfileController(IUserService userService, IRiotService riotService, ISummonerService summonerService)
        {
            Users = userService;
            Riot = riotService;
            Summoners = summonerService;
        }

        public async Task<ActionResult> Index()
        {
            ViewModel = await CreateViewModelAsync();
            return View(ViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Register(SummonerModel model)
        {
            ViewModel = await CreateViewModelAsync();
            if (!ModelState.IsValid)
            {
                return Error(ModelState);
            }

            // Rule: Summoner must not be registered to a User.
            if (await Summoners.IsSummonerRegistered(model.Region, model.SummonerName))
            {
                return Error("Summoner is already registered.");
            }

            // Rule: Summoner must exist.
            var cacheKey = string.Concat(model.Region, ":", model.SummonerName).ToLowerInvariant();
            var summoner = await CacheUtil.GetItemAsync(cacheKey,
                () => Riot.FindSummonerAsync(model.Region, model.SummonerName));

            if (summoner == null)
            {
                return Error("Summoner not found.");
            }
            
            // Rule: Summoner must not be awaiting validation.
            if (Registration.Contains(model.Region, summoner.Name))
            {
                return Error("That summoner requires validation.");
            }

            Registration.Add(model.Region, summoner.Name);
            return Success();
        }

        private async Task<ProfileViewModel> CreateViewModelAsync()
        {
            var viewModel = new ProfileViewModel
            {
                Registrations = Session[RegistrationContextKey] != null
                    ? Registration.Items
                    : new List<RegistrationInfo>(0),
                User = await Users.FindAsync(User.Identity.Name),
            };
            return viewModel;
        }


        private ActionResult Error(ModelStateDictionary modelState)
        {
            var errorStates = modelState.SelectMany(kvp => kvp.Value.Errors);
            return Error(errorStates.First().ErrorMessage);
        }

        private ActionResult Error(string errorMessage)
        {
            return RedirectToAction("Index", new {error = errorMessage});
        }

        private ActionResult Success()
        {
            return RedirectToAction("Index");
        }



        private class RegistrationContext
        {
            private static readonly Random Random = new Random();
            public const int MaxRegistrations = 5;

            public List<RegistrationInfo> Items = new List<RegistrationInfo>();

            public RegistrationInfo Add(string region, string summonerName)
            {
                if (Items.Count == MaxRegistrations)
                {
                    Items.RemoveAt(0);
                }
                var item = new RegistrationInfo
                {
                    Region = region,
                    SummonerName = summonerName,
                    ValidationCode = GetValidationCode()
                };
                Items.Add(item);
                return item;
            }

            public bool Contains(string region, string summonerName)
            {
                return Items.Any(GetComparer(region, summonerName));
            }

            public void Remove(string region, string summonerName)
            {
                var comparer = GetComparer(region, summonerName);
                Items.RemoveAll(info => comparer(info));
            }

            private static Func<RegistrationInfo, bool> GetComparer(string region, string summonerName)
            {
                return info => info.Region.Equals(region, StringComparison.OrdinalIgnoreCase)
                               && info.SummonerName.Equals(summonerName, StringComparison.OrdinalIgnoreCase);
            } 

            private static string GetValidationCode()
            {
                const int length = 5;
                const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                lock (Random)
                {
                    char[] chars = new char[length];
                    for (int i = 0; i < length; i++)
                    {
                        chars[i] = validChars[Random.Next(validChars.Length)];
                    }
                    return new string(chars);
                }
            }
        }
    }
}
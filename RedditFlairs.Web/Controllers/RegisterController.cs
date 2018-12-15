using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RedditFlairs.Core.Clients;
using RedditFlairs.Core.Data;
using RedditFlairs.Core.Entities;
using RedditFlairs.Core.Extensions;
using RedditFlairs.Core.Utility;
using RedditFlairs.Web.Models;

namespace RedditFlairs.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        private readonly FlairDbContext context;
        private readonly IConfiguration configuration;
        private readonly RiotClient riotClient;

        public RegisterController(FlairDbContext context, IConfiguration configuration, RiotClient riotClient)
        {
            this.context = context;
            this.configuration = configuration;
            this.riotClient = riotClient;
        }

        [HttpPost("delete/{id}")]
        public ApiResult<bool> Delete(int id)
        {
            // Summoner must exist locally with this id:
            var summoner = context.Summoners.Find(id);

            if(summoner == null) 
                return ApiResult<bool>.FromError("Summoner not found.");

            // Summoner must belong to the current principal:
            if (summoner.User.Id != User.GetUserId())
                return ApiResult<bool>.FromError("Forbidden.");

            context.DeleteSummoner(summoner);
            return ApiResult<bool>.FromResult(true);
        }

        [HttpGet("regions")]
        public IEnumerable<string> GetRegions()
        {
            IDictionary<string, string> regionEndpoints = new Dictionary<string, string>();
            configuration.GetSection("Riot:RegionEndpoints").Bind(regionEndpoints);

            return regionEndpoints.Keys.OrderBy(x => x);
        }

        [HttpPost("")]
        public async Task<ApiResult<Summoner>> Register(RegisterModel model)
        {
            // Summoner must exist at Riot:
            var riotSummoner = await riotClient.GetSummonerAsync(model.Region, model.SummonerName);

            if (riotSummoner == null)
                return ApiResult<Summoner>.FromError("Summoner not found.");

            // Summoner must not be registered to the current principal.
            if (context.UserHasSummoner(User.GetUserId(), riotSummoner.Id, model.Region))
                return ApiResult<Summoner>.FromError("Summoner is already registered.");

            context.RemoveRegisteredSummoner(riotSummoner.Id, model.Region);

            var userSummoner = context.CreateSummoner(User.GetUserId(), new Summoner
            {
                AccountId = riotSummoner.AccountId,
                PUUID = riotSummoner.PuuId,
                Region = model.Region.ToUpperInvariant(),
                SummonerId = riotSummoner.Id,
                SummonerName = riotSummoner.Name,
                Validation = new SummonerValidation
                {
                    Code = ValidationUtility.CreateValidationCode(),
                    Status = ValidationStatus.NotValidated
                }
            });

            return ApiResult<Summoner>.FromResult(userSummoner);
        }

        public class RegisterModel
        {
            [Required] public string Region { get; set; }
            [Required] public string SummonerName { get; set; }
        }
    }
}
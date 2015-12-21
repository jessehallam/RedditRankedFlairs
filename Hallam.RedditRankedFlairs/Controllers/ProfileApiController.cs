using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Hallam.RedditRankedFlairs.Data;
using Hallam.RedditRankedFlairs.Services;

namespace Hallam.RedditRankedFlairs.Controllers
{
    [Authorize]
    public class ProfileApiController : ApiController
    {
        public IUserService Users { get; set; }

        public ProfileApiController(IUserService users)
        {
            Users = users;
        }

        [HttpGet, Route("profile/api/summoners")]
        public async Task<IEnumerable<object>> GetSummoners()
        {
            var user = await Users.GetUserAsync();
            return user.Summoners.Select(summoner => new
            {
                region = summoner.Region.ToUpperInvariant(),
                summonerName = summoner.Name,
                league = "",
                active = summoner.Id == user.ActiveSummoner.Id
            });
        }
    }
}

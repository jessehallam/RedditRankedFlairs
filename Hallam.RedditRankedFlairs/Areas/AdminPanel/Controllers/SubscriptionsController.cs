using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Hallam.RedditRankedFlairs.Services;
using Hallam.RedditRankedFlairs.Services.Reddit;

namespace Hallam.RedditRankedFlairs.Areas.AdminPanel.Controllers
{
    [AdminAuthorize]
    public class SubscriptionsController : ApiController
    {
        private readonly IRedditService _reddit;
        private readonly ISubRedditService _subReddits;

        public SubscriptionsController(ISubRedditService subReddits, IRedditService reddit)
        {
            _subReddits = subReddits;
            _reddit = reddit;
        }

        [HttpGet, Route("adminPanel/api/moderatorOf")]
        public async Task<IHttpActionResult> GetModeratorOf()
        {
            return Ok(await _reddit.GetSubRedditsAsync(SubRedditKind.Moderator));
        }

        [HttpGet, Route("adminPanel/api/subscriptions")]
        public async Task<IHttpActionResult> Get()
        {
            var entries = await _subReddits.GetAllAsync();
            return Ok(from e in entries
                      orderby e.Name
                      select new { name = e.Name, status = "Linked" });
        }
    }
}

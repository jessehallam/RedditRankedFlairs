using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Hallam.RedditRankedFlairs.Services;

namespace Hallam.RedditRankedFlairs.Areas.AdminPanel.Controllers
{
    [AdminAuthorize]
    public class SubscriptionsController : ApiController
    {
        private readonly ISubRedditService _subReddits;

        public SubscriptionsController(ISubRedditService subReddits)
        {
            _subReddits = subReddits;
        }

        [AcceptVerbs("GET", "POST"), Route("adminPanel/api/subscriptions")]
        public async Task<IHttpActionResult> Get(bool? refresh)
        {
            if (Request.Method == HttpMethod.Post && refresh == true)
            {
                // todo
            }

            var entries = await _subReddits.GetAllAsync();
            return Ok(from e in entries
                      orderby e.Name
                      select new { name = e.Name, status = "Linked" });
        }
    }
}

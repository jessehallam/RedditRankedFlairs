using Microsoft.AspNetCore.Mvc;
using RedditFlairs.Core.Data;
using RedditFlairs.Core.Entities;
using RedditFlairs.Core.Extensions;

namespace RedditFlairs.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly FlairDbContext context;

        public ProfileController(FlairDbContext context)
        {
            this.context = context;
        }

        public User Get()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            return context.Users.Find(User.GetUserId());
        }
    }
}

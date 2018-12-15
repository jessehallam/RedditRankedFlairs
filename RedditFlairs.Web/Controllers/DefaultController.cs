using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Reddit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedditFlairs.Core.Data;
using RedditFlairs.Core.Entities;

namespace RedditFlairs.Web.Controllers
{
    [Route("")]
    public class DefaultController : Controller
    {
        private readonly FlairDbContext context;

        public DefaultController(FlairDbContext context)
        {
            this.context = context;
        }

        [Route("sign-in")]
        public IActionResult SignInChallenge()
        {
            return Challenge(new AuthenticationProperties
            {
                AllowRefresh = false,
                IsPersistent = false,
                RedirectUri = "/sign-in-callback"
            });
        }

        [Route("sign-in-callback")]
        public async Task<IActionResult> SignInCallback()
        {
            // Ensure this is an inbound callback from Reddit:
            if (User.Identity.IsAuthenticated && User.Identity.AuthenticationType == RedditAuthenticationDefaults.AuthenticationScheme)
            {
                var identity = CreateInternalIdentity(User.Identity.Name);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(principal);
            }

            return LocalRedirect("/");
        }

        private ClaimsIdentity CreateInternalIdentity(string name)
        {
            var user = context.Users.SingleOrDefault(x => x.Name == name);

            if (user == null)
            {
                user = new User {Name = name};
                context.Users.Add(user);
                context.SaveChanges();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

            return identity;
        }
    }
}
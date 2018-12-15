using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace RedditFlairs.Core.Extensions
{
    public static class PrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var id = principal.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(id.Value);
        }
    }
}
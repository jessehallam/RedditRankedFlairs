using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Hallam.RedditRankedFlairs.Data;
using Hallam.RedditRankedFlairs.Services;

namespace Hallam.RedditRankedFlairs.Controllers
{
    public class ProfileSummonerController : Controller
    {
        public IUserService Users { get; set; }

        public ProfileSummonerController(IUserService users)
        {
            Users = users;
        }
    }
}
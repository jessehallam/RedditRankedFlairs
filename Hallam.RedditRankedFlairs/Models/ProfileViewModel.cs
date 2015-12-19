using System.Collections.Generic;
using Hallam.RedditRankedFlairs.Data;

namespace Hallam.RedditRankedFlairs.Models
{
    public class RegistrationInfo
    {
        public string Region { get; set; }
        public string SummonerName { get; set; }
        public string ValidationCode { get; set; }
    }

    public class ProfileViewModel
    {
        public IList<RegistrationInfo> Registrations { get; set; }
        public User User { get; set; }
    }
}
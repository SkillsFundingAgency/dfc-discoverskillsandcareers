using System;
using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp
{
    public class AppSettings
    {
        public string SessionSalt { get; set; }
        public string SiteFinityApiUrlbase { get; set; }
        public string SiteFinityApiWebService { get; set; }
    }

}

﻿using System;
using System.Diagnostics.CodeAnalysis;
using Dfc.DiscoverSkillsAndCareers.Repositories;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp
{

    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public string SessionSalt { get; set; }
        public string SiteFinityApiUrlBase { get; set; }
        public string SiteFinityApiWebService { get; set; }
        public bool SiteFinityRequiresAuthentication { get; set; }
        public string SiteFinityApiAuthenicationEndpoint { get; set;}
        public string SiteFinityScope { get; set; }
        public string SiteFinityUsername { get; set; }
        public string SiteFinityPassword { get; set; }
        public string SiteFinityClientId { get; set; }
        public string SiteFinityClientSecret { get; set; }
        public string SiteFinityJobCategoriesTaxonomyId { get; set; }
        public double MaxPercentageOfProfileOccurenceForSkill { get; set; } = 0.75;
        public double MaxPercentageDistributionOfJobProfiles { get; set; } = 0.75;
    }

}

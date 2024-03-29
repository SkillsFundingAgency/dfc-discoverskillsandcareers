﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    [ExcludeFromCodeCoverage]
    public class SiteFinityTrait
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Title")]
        public string Name { get; set; }
        [JsonProperty("LastModified")]
        public DateTime LastUpdated { get; set; }
        [JsonProperty("ResultDisplayText")]
        public string ResultDisplayText { get; set; }
        [JsonProperty("jobprofilecategories")]
        public List<Guid> JobProfileCategories { get; set; }
        [JsonProperty("UrlName")]
        public string UrlName { get; set; }
    }
}

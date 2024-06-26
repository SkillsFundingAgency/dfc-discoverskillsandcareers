﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    [ExcludeFromCodeCoverage]
    public class SiteFinityShortQuestionSet
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonIgnore()]
        public List<SiteFinityShortQuestion> Questions { get; set; }
        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdated { get; set; }
    }
}

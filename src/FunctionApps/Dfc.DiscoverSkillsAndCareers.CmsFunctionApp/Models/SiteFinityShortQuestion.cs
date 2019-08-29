﻿using Newtonsoft.Json;
using System;

namespace Dfc.DiscoverSkillsAndCareers.CmsFunctionApp.Models
{
    public class SiteFinityShortQuestion
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("QuestionText")]
        public string Title { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Trait")]
        public SiteFinityTrait Trait { get; set; }

        [JsonProperty("IsNegative")]
        public bool IsNegative { get; set; }

        [JsonProperty("LastModified")]
        public DateTimeOffset LastUpdatedDt { get; set; }
    }
}
using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Models
{
    public class ResultsViewModel
    {
        public string SessionId { get; set; }
        public List<JobFamily> JobFamilies { get; set; }
        public List<Trait> Traits { get; set; }
    }
}

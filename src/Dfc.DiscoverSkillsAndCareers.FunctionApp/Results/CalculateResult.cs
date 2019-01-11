using System;
using System.Collections.Generic;
using Dfc.DiscoverSkillsAndCareers.Models;

namespace Dfc.DiscoverSkillsAndCareers.FunctionApp.Results
{
    public class CalculateResult
    {
        public static void Run(UserSession userSession)
        {
            var resultData = new ResultData()
            {
                TraitCodes = new List<string>()
                 {
                     "TRAIT_CODE1",
                     "TRAIT_CODE2",
                     "TRAIT_CODE3"
                 }
            };
            
           userSession.ResultData = resultData;
        }
    }
}

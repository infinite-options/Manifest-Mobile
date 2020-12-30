using Manifest.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manifest.Services.Rds
{
    class OccuranceResponse
    {
        public string message { get; set; }
        public List<OccuranceDto> result { get; set; }

        public List<Occurance> ToOccurances()
        {
            List<Occurance> occurances = new List<Occurance>();
            if (result == null || result.Count == 0) return occurances;
            foreach (OccuranceDto dto in result)
            {
                if (dto.is_displayed_today == "True"){
                    occurances.Add(dto.ToOccurance());
                }
            }
            return occurances;
        }
    }
}

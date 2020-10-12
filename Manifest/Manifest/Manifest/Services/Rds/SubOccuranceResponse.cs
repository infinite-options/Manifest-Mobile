using System;
using System.Collections.Generic;
using System.Text;
using Manifest.Models;

namespace Manifest.Services.Rds
{
    class SubOccuranceResponse
    {
        public string message { get; set; }
        public List<SubOccuranceDto> result { get; set; }

        public List<SubOccurance> ToSubOccurances()
        {
            List<SubOccurance> occurances = new List<SubOccurance>();
            if (result == null || result.Count == 0) return occurances;
            foreach (SubOccuranceDto dto in result)
            {
                occurances.Add(item: dto.ToSubOccurances());
            }
            return occurances;
        }
    }
}

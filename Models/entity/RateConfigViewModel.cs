using System;
using System.Collections.Generic;

namespace crmcsharp.Models.entity
{
    public class RateConfigViewModel
    {
        public List<RateConfig> RateConfigs { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
        public RateConfig NewRateConfig { get; set; } = new RateConfig();
    }
}

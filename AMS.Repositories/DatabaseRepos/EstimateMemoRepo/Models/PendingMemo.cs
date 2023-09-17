using AMS.Repositories.DatabaseRepos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Repositories.DatabaseRepos.EstimateMemo.Models
{
    public class PendingMemo : CommonProperties
    {
        public int EstimateReferenceId { get; set; }

        public string EstimateType { get; set; }
        public string UniqueIdentifier { get; set; }
        public string EstimateSubject { get; set; }
        public Double TotalAllowableBudget { get; set; }

        public Double TotalCost { get; set; }
        public Double Deviation { get; set; }
        public Double Percentage { get; set; }
        public Double SettledAmount { get; set; }

        public int UserPriority { get; set; }
    }
}

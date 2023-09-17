using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.ServiceModels.Settlement
{
    public  class SettlementApprovalStatus
    {
        public string Status { get; set; }
        public string Username { get; set; }
        public int Priority { get; set; }
        public int RolePriority { get; set; }
    }
}

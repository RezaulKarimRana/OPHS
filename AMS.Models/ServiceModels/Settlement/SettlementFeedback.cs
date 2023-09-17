using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.ServiceModels.Settlement
{
    public class SettlementFeedback
    {
        public int SettlementId { get; set; }
        public int UserId { get; set; }
        public int Feedback { get; set; }
        public int CurrentUserRolePiority { get; set; }
        public string Remarks { get; set; }
    }
}

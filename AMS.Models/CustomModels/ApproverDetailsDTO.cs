using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.CustomModels
{
    public class ApproverDetailsDTO
    {
        public int Id { get; set; }
        public int ApproverId { get; set; }
        public string ApproverFirstName { get; set; }
        public string ApproverLastName { get; set; }
        public string ApproverUserName { get; set; }
        public string ApproverStatus { get; set; }
        public int ApproverRoleId { get; set; }
        public int ApproverPriority { get; set; }
        public DateTime? ApproverFeedbackDate { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.CustomModels
{
    public class MemoApproverDetailsDTO
    {
        public int EstimateMemoId { get; set; }
        public int ApproverId { get; set; }
        public string ApproverUserName { get; set; }
        public string ApproverEmail { get; set; }
        public string ApproverFullName { get; set; }
        public string ApproverDepartment { get; set; }
        public int ApproverDepartmentId { get; set; }
        public int ApproverPriority { get; set; }
        public string ApproverStatus { get; set; }
        public int ApproverRoleId { get; set; }
        public string ApproverRoleName { get; set; }
        public DateTime ApproverPlanDate { get; set; }
        public string Remarks { get; set; }
        public int RolePriority_Id { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace AMS.Models.ViewModel
{
    public class ApproverModificationVM
    {
        public int RequestId { get; set; }
        public int ApproverId { get; set; }
        public string ApproverFullName { get; set; }
        public string ApproverDepartment { get; set; }
        public int ApproverDepartmentId { get; set; }
        public int ApproverPriority { get; set; }
        public int ApproverStatus { get; set; }
        public int ApproverRoleId { get; set; }
        public string ApproverRoleName { get; set; }
        public DateTime PlanDate { get; set; }
        public string PlanDateString
        {
            get
            {
                return PlanDate == null ? string.Empty : PlanDate.ToString("yyyy-MM-dd");
            }
        }
        public int RolePriority_Id { get; set; }
    }
    public class ApproverModificationUpdateModel
    {
        public int ModuleId { get; set; }
        public string RequestNo { get; set; }
        public string Items { get; set; }
        public List<ApproverModel> Approvers { get; set; }
    }
    public class ApproverModel
    {
        public int ApproverId { get; set; }
        public int RolePriority_Id { get; set; }
        public DateTime PlanDate { get; set; }
        public bool IsApproved { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
    }
    public class ApproverRoleUpdateModel
    {
        public int ModuleId { get; set; }
        public string RequestNo { get; set; }
        public int ApproverId { get; set; }
        public int RoleId { get; set; }
    }
    public class StatusUpdateModel
    {
        public int ModuleId { get; set; }
        public int StatusId { get; set; }
        public string RequestNo { get; set; }
    }
    public class UserDepartmentUpdateModel
    {
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
    }
}

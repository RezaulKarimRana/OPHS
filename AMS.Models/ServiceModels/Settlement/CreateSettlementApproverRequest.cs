using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.ServiceModels.Settlement
{
    public class CreateSettlementApproverRequest 
    {
       
            public int Settlement_Id { get; set; }
            public int User_Id { get; set; }
            public string ApproverFullName { get; set; }
            public string ApproverDepartment { get; set; }
            public int Priority { get; set; }
           public int Created_By { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }
            public int RolePriority_Id { get; set; }
            public string ApproverRoleName { get; set; }
            public string ApproverRole { get; set; }
            public string PlanDate { get; set; }
            public string ExpectedTime { get; set; }

            public CreateSettlementApproverRequest()
            {
                Remarks = "";
                RolePriority_Id = 0;
            }
        
    }
}

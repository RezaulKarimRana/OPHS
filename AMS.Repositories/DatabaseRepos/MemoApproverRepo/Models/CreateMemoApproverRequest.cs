using System;

namespace AMS.Repositories.DatabaseRepos.MemoApproverRepo.Models
{
    public class CreateMemoApproverRequest
    {
        public int UserId { get; set; }
        public int PriorityId { get; set; }
        public string StatusId { get; set; }
        public string Remarks { get; set; }
        public int RolePriorityId { get; set; }
        public int Created_By { get; set; }

        public CreateMemoApproverRequest()
        {
            StatusId = "2";
            Remarks = "";
        }
    }
}

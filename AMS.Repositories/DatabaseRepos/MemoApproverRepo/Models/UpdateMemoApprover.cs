using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.MemoApproverRepo.Models
{
    public class UpdateMemoApprover : UpdatedBy
    {
        public string Status { get; set; }
        public string Remarks { get; set; }
        public bool IsFinalApproved { get; set; }
    }
}

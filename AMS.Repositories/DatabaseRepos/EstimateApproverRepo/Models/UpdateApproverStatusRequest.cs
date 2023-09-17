namespace AMS.Repositories.DatabaseRepos.EstimateApproverRepo.Models
{
    public class UpdateApproverStatusRequest
    {
        public string status { get; set; }
        public string remarks { get; set; }
        public int id { get; set; }
        public int updatedby { get; set; }
    }
}

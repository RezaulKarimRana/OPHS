namespace Repositories.DatabaseRepos.DashboardRepo.Models
{
    public class GetCountForAllPendingParkingsForNav
    {
        public int TotalDraftParking { get; set; }
        public int TotalCompletedParking { get; set; }
        public int TotalRollbackParking { get; set; }
        public int TotalPendingApprovalParking { get; set; }
    }
}

namespace Models.ServiceModels.Dashboard
{
    public class GetCountForAllPendingParkingForNavService : ServiceResponse
    {
        public int TotalDraftParking { get; set; }
        public int TotalCompletedParking { get; set; }
        public int TotalRollbackParking { get; set; }
        public int TotalPendingApprovalParking { get; set; }
    }
}

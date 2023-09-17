namespace AMS.Models.ServiceModels.Dashboard
{
    public class GetIndexDashBoardResponse : ServiceResponse
    {
        public int TotalSessions { get; set; }

        public int TotalUsers { get; set; }

        public int TotalRoles { get; set; }

        public int TotalConfigItems { get; set; }

        public int TotalRunningBudget { get; set; }

        public int TotalCompletedBudget { get; set; }
        public int TotalDraftedBudget { get; set; }

        public decimal TotalAmount { get; set; }
    }
}

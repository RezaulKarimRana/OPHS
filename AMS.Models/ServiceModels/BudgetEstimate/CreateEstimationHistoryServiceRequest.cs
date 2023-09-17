namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class CreateEstimationHistoryServiceRequest
    {
        public int EstimateType { get; set; }
        public string Status { get; set; }
        public string SystemID { get; set; }
        public int ProjectId { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }

        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }

        public string Remarks { get; set; }
        public double TotalPrice { get; set; }
        public int Estimation_Id { get; set; }

        public CreateEstimationHistoryServiceRequest()
        {
            SystemID = "";
            ProjectId = 0;
            Objective = "";
            Details = "";
            Remarks = "";
            TotalPrice = 0;
        }
    }
}

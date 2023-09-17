namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class CreateParticularWiseSummaryHistoryServiceRequest
    {
        public int Particular_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
        public int ParticularWishSummary_Id { get; set; }
    }
}

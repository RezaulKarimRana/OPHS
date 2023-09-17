namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class CreateParticularWiseSummaryServiceRequest
    {
        public int Particular_Id { get; set; }
        public string ParticlarName { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
    }
}

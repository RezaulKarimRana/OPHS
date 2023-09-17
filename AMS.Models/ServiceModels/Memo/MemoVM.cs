namespace AMS.Models.ServiceModels.Memo
{
    public class MemoVM
    {
        public int Id { get; set; }
        public int EstimateReferenceId { get; set; }
        public string EstimateType { get; set; }
        public string EstimationIdentity { get; set; }
        public string Subject { get; set; }
        public double BudgetPrice { get; set; }
        public double AllowableBudget { get; set; }
        public double TotalCost { get; set; }
        public double TotalDeviation { get; set; }
        public int TotalRow { get; set; }
    }
}

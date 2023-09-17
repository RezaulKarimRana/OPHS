namespace AMS.Models.ServiceModels.Settlement
{
    public class SettlementSummaryVM
    {
        public int Draft { get; set; }
        public int Pending { get; set; }
        public int CR { get; set; }
        public int Completed { get; set; }
        public int Rejected { get; set; }
        public int Total { get; set; }
    }
}

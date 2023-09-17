namespace AMS.Models.ServiceModels.Memo
{
    public class MemoSummaryVM
    {
        public int Pending { get; set; }
        public int CR { get; set; }
        public int Completed { get; set; }
        public int Rejected { get; set; }
        public int Total { get; set; }
    }
}

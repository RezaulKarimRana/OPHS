namespace Models.CustomModels
{
    public class LoadApproverFeedBackDetails
    {
        public int EstimateApproverTableRowId { get; set; }
        public int EstimateId { get; set; }
        public int ApproverUserId { get; set; }
        public string ApproverUserName { get; set; }
        public string ApproverFullName { get; set; }
        public string FeedBack { get; set; }
        public int EstimateStatus { get; set; }
        public string EstimateApproverStatusString { get; set; }
        public string FeedBackDate { get; set; }
    }
}

namespace AMS.Models.ViewModel
{
    public class ApprovalFeedBackDTO
    {
        public int MemoId { get; set; }
        public string Feedback { get; set; }
        public string Remarks { get; set; }
        public bool IsFinalApproved { get; set; }
    }
}

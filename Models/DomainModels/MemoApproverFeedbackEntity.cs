namespace Models.DomainModels
{
    public class MemoApproverFeedbackEntity : BaseEntity
    {
        public int MemoApproverId { get; set; }
        public int EstimateMemoId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }

    }
}

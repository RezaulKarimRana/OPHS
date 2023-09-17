namespace AMS.Models.DomainModels
{
    public class EstimateApproverFeedbackEntity : BaseEntity
    {
        public int EstimateApprover_Id { get; set; }
        public int Estimation_Id { get; set; }
        public string FeedbackRemarks { get; set; }

        /// <summary>
        /// 2= pending at user side
        /// -404= CR raise from user side
        /// 100= completed
        /// </summary>
        public int Status { get; set; }
    }
}

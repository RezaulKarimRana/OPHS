using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.EstimateApproverFeedbackRepo.Models
{
    public class CreateApproverFeedbackRequest : CreatedBy
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
        public int DivisionId { get; set; }

        public CreateApproverFeedbackRequest()
        {
            FeedbackRemarks = "";
            DivisionId = 0;
        }
    }
}

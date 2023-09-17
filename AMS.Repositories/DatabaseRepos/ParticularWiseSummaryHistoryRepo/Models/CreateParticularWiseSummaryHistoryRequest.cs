using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo.Models
{
    public class CreateParticularWiseSummaryHistoryRequest : CreatedBy
    {
        public int Particular_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
        //public int ParticularWishSummary_Id { get; set; }
    }
}

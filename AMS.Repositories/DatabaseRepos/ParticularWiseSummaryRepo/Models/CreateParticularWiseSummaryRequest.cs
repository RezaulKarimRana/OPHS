using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.ParticularWiseSummaryRepo.Models
{
    public class CreateParticularWiseSummaryRequest : CreatedBy
    {
        public int Particular_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
    }
}

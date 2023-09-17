namespace AMS.Models.DomainModels
{
    public class ParticularWiseSummaryHistoryEntity : BaseEntity
    {
        public int Particular_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
        public int ParticularWishSummary_Id { get; set; }
    }
}

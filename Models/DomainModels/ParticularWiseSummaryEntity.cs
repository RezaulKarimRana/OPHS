namespace Models.DomainModels
{
    public class ParticularWiseSummaryEntity : BaseEntity
    {
        public int Particular_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
    }
}

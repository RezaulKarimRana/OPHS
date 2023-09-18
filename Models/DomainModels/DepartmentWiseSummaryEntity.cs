namespace Models.DomainModels
{
    public class DepartmentWiseSummaryEntity : BaseEntity
    {
        public int Department_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
    }
}

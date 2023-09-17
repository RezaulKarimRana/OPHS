namespace AMS.Models.DomainModels
{
    public class DepartmentWiseSummaryHistoryEntity : BaseEntity
    {
        public int Department_Id { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
        public int DepartmentWiseSummary_Id { get; set; }
    }
}

namespace AMS.Models.CustomModels
{
    public class DepartWiseSummaryDetailsByEstimationId
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public double Price { get; set; }
        public int EstimationId { get; set; }

    }
}

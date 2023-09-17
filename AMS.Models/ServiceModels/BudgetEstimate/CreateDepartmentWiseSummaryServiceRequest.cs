namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class CreateDepartmentWiseSummaryServiceRequest
    {
        public int Department_Id { get; set; }
        public string DepartmentName { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
    }
}

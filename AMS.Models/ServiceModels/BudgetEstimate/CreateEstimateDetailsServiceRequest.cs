namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class CreateEstimateDetailsServiceRequest
    {
        public int Estimation_Id { get; set; }
        public string Particular { get; set; }
        public string ItemCategory { get; set; }
        public string ItemName { get; set; }
        public int Item_Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemUnit { get; set; }
        public int NoOfMachineAndUsagesAndTeamAndCar { get; set; }
        public int NoOfDayAndTotalUnit { get; set; }
        public double QuantityRequired { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public string Remarks { get; set; }
        public string AreaType { get; set; }
        public string ResponsibleDepartmentName { get; set; }       
        public int ResponsibleDepartment_Id { get; set; }
        public string DistrictName { get; set; }
        public string ThanaName { get; set; }
        public int Thana_Id { get; set; }
        public int EstimateSettleItemId { get; set; }

        public CreateEstimateDetailsServiceRequest()
        {
            Remarks = "";
        }
    }
}

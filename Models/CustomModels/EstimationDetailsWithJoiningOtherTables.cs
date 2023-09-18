namespace Models.CustomModels
{
    public class EstimationDetailsWithJoiningOtherTables
    {
        public int EstimationId { get; set; }
        public int ParticularId { get; set; }
        public string Particular { get; set; }
        public int ItemCategoryId { get; set; }
        public string ItemCategory { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string ItemUnit { get; set; }
        public int NoOfMachineAndUsagesAndTeamAndCar { get; set; }
        public int NoOfDayAndTotalUnit { get; set; }
        public double QuantityRequired { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public string Remarks { get; set; }
        public string AreaType { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public string DistrictName { get; set; }
        public int ThanaId { get; set; }
        public string ThanaName { get; set; }
        public int EstimateSettleItemId { get; set; }

        public EstimationDetailsWithJoiningOtherTables()
        {
            AreaType = "";
            Remarks = "";
        }
    }
}

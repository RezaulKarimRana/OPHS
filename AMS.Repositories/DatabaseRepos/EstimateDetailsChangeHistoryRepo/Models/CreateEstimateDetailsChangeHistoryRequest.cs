using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo.Models
{
    public class CreateEstimateDetailsChangeHistoryRequest : CreatedBy
    {
        public int Estimation_Id { get; set; }
        public int Item_Id { get; set; }
        public int NoOfMachineAndUsagesAndTeamAndCar { get; set; }
        public int NoOfDayAndTotalUnit { get; set; }
        public double QuantityRequired { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public string Remarks { get; set; }
        public string AreaType { get; set; }
        public int ResponsibleDepartment_Id { get; set; }
        public int Thana_Id { get; set; }
    }
}

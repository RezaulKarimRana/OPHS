using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.ServiceModels.Settlement
{
    public class CreateSettlementItemRequest
    {
        public int EstimationId { get; set; }
        public int SettlementId { get; set; }
        public int EstimateSettleItemId { get; set; }

        //public int ParticularId { get; set; }
        public string Particular { get; set; }
        //public int ItemCategoryId { get; set; }
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
        public string ResponsibleDepartmentName { get; set; }
        public int ResponsibleDepartment_Id { get; set; }
        public string DistrictName { get; set; }
        public int ThanaId { get; set; }
        //public string ThanaName { get; set; }
        public double AlreadySettle { get; set; }
        public double SettleActualQuantity { get; set; }
        public int SettleItemId { get; set; }
        public double ActualQuantity { get; set; }
        public double ActualUnitPrice { get; set; }
        public double ActualTotalPrice { get; set; }
        public string SettleItemRemarks { get; set; }
        public int CreatedBy { get; set; }

        public CreateSettlementItemRequest()
        {
            SettleItemRemarks = "";
        }
    }
}

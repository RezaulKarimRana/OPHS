using AMS.Models.DomainModels;
using System;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.BudgetEstimate
{
    public class EstimateEditVM
    {
        public EstimateEditVM()
        {
            ItemList = new List<ItemVM>();
            EstimateDetailsList = new List<EstimateDetailsVM>();
            EstimateApproverList = new List<EstimateApproverVM>();
            ParticularWiseSummaryList = new List<ParticularWiseSummaryVM>();
            DepartmentWiseSummaryList = new List<DepartmentWiseSummaryVM>();
        }

        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public int EstimateType_Id { get; set; }
        public int CurrencyType { get; set; }
        public string EstimateType { get; set; }
        public string Status { get; set; }
        public int Project_Id { get; set; }
        public int IsFinalSettle { get; set; }
        public string ProjectName { get; set; }
        public string EstimationIdentity { get; set; }
        public string Subject { get; set; }
        public string Objective { get; set; }
        public string Details { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public string Remarks { get; set; }
        public Double TotalPrice { get; set; }
        public UserEntity CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }

        public int Priority { get; set; }
        public int NextPriority { get; set; }
        public int TotalRow { get; set; }
        public int isItAllowableForSettlement { get; set; }
        public int draftExists { get; set; }
        public int fDraftExists  { get; set; }
        
        
        public string IsInFinalApproval { get; set; }
        public List<ItemVM> ItemList { get; set; }
        public List<ParticularWiseSummaryVM> ParticularWiseSummaryList { get; set; }
        public List<DepartmentWiseSummaryVM> DepartmentWiseSummaryList { get; set; }
        public List<EstimateApproverVM> EstimateApproverList { get; set; }
        public List<EstimateDetailsVM> EstimateDetailsList { get; set; }


        #region Fund Requistion & Disburse Related
        public int TotalAllowableBudget { get; set; }
        public int TotalRequisitionAmount { get; set; }
        public int RemainingBudget { get; set; }
        public int TotalReceived { get; set; }


        #endregion

    }

    public class EstimateDetailsVM
    {
        public int Estimation_Id { get; set; }
        public int Item_Id { get; set; }
        public int NoOfMachineAndUsagesAndTeamAndCar { get; set; }
        public int NoOfDayAndTotalUnit { get; set; }
        public double QuantityRequired { get; set; }
        public double UnitPrice { get; set; }
        public string Unit { get; set; }
        public double TotalPrice { get; set; }
        public string Remarks { get; set; }
        public string AreaType { get; set; }

        public string ResponsibleDepartment { get; set; }
        public string Thana { get; set; }
        public string District { get; set; }

        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public string ParticularName { get; set; }
        public string ItemCode { get; set; }

        public int ResponsibleDepartment_Id { get; set; }
        public int Thana_Id { get; set; }
    }

    public class ItemVM
    {
        //Item Details
        public int Particular_Id { get; set; }
        public int ItemCategory_Id { get; set; }
        public int Item_Id { get; set; }
        public string Name { get; set; }
        public int Unit_Id { get; set; }
        public string ItemCode { get; set; }
        public int IndicatingUnitPrice { get; set; }
        public int System_Id { get; set; }
        public string SystemName { get; set; }
    }

    public class ParticularWiseSummaryVM
    {
        public int Particular_Id { get; set; }
        public string ParticularName { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
    }

    public class DepartmentWiseSummaryVM
    {
        public int Department_Id { get; set; }
        public string DepartmentName { get; set; }
        public double TotalPrice { get; set; }
        public int Estimate_Id { get; set; }
    }

    public class EstimateApproverVM
    {
        public int Estimate_Id { get; set; }
        public int User_Id { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string UserDept { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string ResponseTime { get; set; }
        public int RolePriority_Id { get; set; }
    }
}

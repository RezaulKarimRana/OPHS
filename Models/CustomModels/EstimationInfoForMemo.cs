using Models.DomainModels;
using System;
using System.Collections.Generic;

namespace Models.CustomModels
{
    public class EstimationInfoForMemo
    {
        public string EstimateIdProtected { get; set; }
        public int EstimateId { get; set; }
        public int EstimationTypeId { get; set; }
        public string EstimationTypeName { get; set; }
        public string EstimationTypeProject { get; set; }
        public string EstimationStatus { get; set; }
        public string EstimationSystemID { get; set; }
        public string EstimationIdentifier { get; set; }
        public string EstimationSubject { get; set; }
        public string EstimationObjective { get; set; }
        public string EstimationDetails { get; set; }
        public DateTime EstimationPlanStartDate { get; set; }
        public DateTime EstimationPlanEndDate { get; set; }
        public string EstimationRemarks { get; set; }
        public double EstimaionTotalPrice { get; set; }
        public int CreatorID { get; set; }
        public string CreateorFullName { get; set; }
        public string CreatorDepartment { get; set; }
        public string CreatorEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        public int EstimationReferenceId { get; set; }
        public double AllowableBudget { get; set; }
        public double TotalCost { get; set; }
        public double Deviation { get; set; }
        public double Percentage { get; set; }
        public int MemoId { get; set; }
        public string Justification { get; set; }
        public bool CanDeleteAttachments { get; set; }
        public List<EstimateMemoAttachmentsEntity> EstimateMemoAttachments { get; set; }

        #region strProperties
        public string strEstimaionTotalPrice { get; set; }
        public string strAllowableBudget { get; set; }
        public string strTotalCost { get; set; }
        public string strDeviation { get; set; }
        #endregion

        public EstimationInfoForMemo()
        {
            EstimateMemoAttachments = new List<EstimateMemoAttachmentsEntity>();
        }
    }
}

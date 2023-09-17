using AMS.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.CustomModels
{
    public class EstimateMemoDetails
    {
        public int Id { get; set; }
        public string EstimateType { get; set; }
        public string EstimateStatus { get; set; }
        public string EstimationIdentity { get; set; }
        public string Subject { get; set; }
        public double BudgetPrice { get; set; }
        public double AllowableBudget { get; set; }
        public double FinalSettledAmount { get; set; }
        public double TotalDeviation { get; set; }
        public int TotalRow { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public int InitiatorId { get; set; }
        public UserEntity InitiatorDetail { get; set; }
        public List<ApproverDetailsDTO> ApproverDetailsDTO { get; set; }

        public EstimateMemoDetails()
        {
            ApproverDetailsDTO = new List<ApproverDetailsDTO>();
            InitiatorDetail = new UserEntity();
        }
    }
}

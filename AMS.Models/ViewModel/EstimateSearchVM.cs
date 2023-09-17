using System;

namespace AMS.Models.ViewModel
{
    public class EstimateSearchVM
    {
        public string Id { get; set; }
        public string EstimationTypeName { get; set; }
        public int CurrencyType { get; set; }
        public string EstimateSubject { get; set; }
        public string EstimateIdentity { get; set; }
        public string EstimationStatus { get; set; }
        public string Creator { get; set; }
        public string CreateorFullName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalPrice { get; set; }
    }
}

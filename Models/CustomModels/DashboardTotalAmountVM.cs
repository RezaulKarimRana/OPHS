using System.Collections.Generic;

namespace Models.CustomModels
{
    public  class DashboardTotalAmountVM
    {
        public string Currency { get; set; }
        public double TotalAmount { get; set; }
    }
    public class SimpleReportViewModel
    {
        public string DimensionOne { get; set; }
        public int Quantity { get; set; }
    }
    public class StackedViewModel
    {
        public string StackedDimensionOne { get; set; }
        public List<SimpleReportViewModel> LstData { get; set; }
    }
}

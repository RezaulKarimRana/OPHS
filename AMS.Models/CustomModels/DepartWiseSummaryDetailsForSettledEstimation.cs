using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.CustomModels
{
    public class DepartWiseSummaryDetailsForSettledEstimation
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public double TotalCost { get; set; }
        public double TotalRunningCost { get; set; }
        public double TotalBudget { get; set; }
    }
    public class DepartWiseSummaryDetailsForSettledEstimationIncludeTADA
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public double TotalCost { get; set; }
        public double TotalRunningCost { get; set; }
        public double TotalBudget { get; set; }
        public double TotalTADABudget { get; set; }
        public double TotalAllowableBudget
        {
            get
            {
                return TotalBudget - TotalTADABudget;
            }
        }
        public double Deviation
        {
            get
            {
                return TotalAllowableBudget - TotalCost;
            }
        }
        public double Parcentage
        {
            get
            {
                if (TotalCost <= 0)
                    return 0;
                return (TotalCost*100)/ TotalBudget;
            }
        }
    }
}

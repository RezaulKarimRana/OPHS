using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.CustomModels
{
    public class EstimateSettleItemDetails
    {
        public string ParticularName { get; set; }
        public string ItemCategoryName { get; set; }
        public string ItemName { get; set; }
        public int ActualQuantity { get; set; }
        public double ActualUnitPrice { get; set; }
        public double ActualTotalPrice { get; set; }
        public string Remarks { get; set; }
    }
}

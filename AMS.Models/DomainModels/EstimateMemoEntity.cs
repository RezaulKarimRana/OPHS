using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.DomainModels
{
    public class EstimateMemoEntity : BaseEntity
    {
        public EstimateMemoEntity()
        {
            Justification = "";
        }
        public int EstimateReferenceId { get; set; }
        public int EstimateId { get; set; }
        public string Status { get; set; }
        public string EstimateIdentifier { get; set; }
        public Double TotalDeviation { get; set; }
        public String Justification { get; set; }

    }
}

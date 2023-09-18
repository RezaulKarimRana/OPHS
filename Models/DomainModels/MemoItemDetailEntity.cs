using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DomainModels
{
    public class MemoItemDetailEntity : BaseEntity
    {
        public MemoItemDetailEntity()
        {
            MemoRemarks = "";
        }
        public int EstimateMemoId { get; set; }
        public int EstimateDetailId { get; set; }
        public Double Deviation { get; set; }
        public string MemoRemarks { get; set; }      
    }
}

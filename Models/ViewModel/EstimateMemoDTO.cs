using System;

namespace Models.ViewModel
{
    public class EstimateMemoDTO
    {
        public Double TotalDeviation { get; set; }
        public string JustificaitonText { get; set; }
        public int EstimateReferId { get; set; }
        public int EstimateId { get; set; }

        public EstimateMemoDTO()
        {
            JustificaitonText = "";
        }
    }
}

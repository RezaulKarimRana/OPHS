using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models.DomainModels
{
    public class EstimationReferenceEntity : BaseEntity
    {
        public int EstimationId { get; set; }
        public Double AllowableBudget { get; set; }
        public Double RemainingBudget { get; set; }
        public Double AlreadySettle { get; set; }
        public int IsFinalSettle { get; set; }

    }
}

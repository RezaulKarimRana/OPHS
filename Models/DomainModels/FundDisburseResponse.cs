using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DomainModels
{
    public class FundDisburseResponse : BaseEntity
    {
        public int DisburseAmount { get; set;  }
        public string Remarks { get; set;  }
        public int ResponseStatus { get; set;  }
        public int FundDisburseId { get; set;  }
    }
}

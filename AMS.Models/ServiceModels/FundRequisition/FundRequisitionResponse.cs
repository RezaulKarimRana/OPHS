using System;
using System.Collections.Generic;
using System.Text;
using AMS.Models.CustomModels;

namespace AMS.Models.ServiceModels.FundRequisition
{
    public class FundRequisitionResponse : ServiceResponse
    {

        public EstimationWithEstimationType Estimation { get; set; }
        public FundRequisitionResponse()
        {
            Estimation = new EstimationWithEstimationType();
        }
    }
}

using AMS.Models.CustomModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Admin.Estimation
{
    public class EstimationNameListGetResponse : ServiceResponse
    {
        public IList<GetAllEstimationSubjectNames> EstimationNames { get; set; }

        public EstimationNameListGetResponse()
        {
            EstimationNames = new List<GetAllEstimationSubjectNames>();
        }
    }
}

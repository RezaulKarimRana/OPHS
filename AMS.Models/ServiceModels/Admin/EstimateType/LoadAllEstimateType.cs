using AMS.Models.DomainModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Admin.EstimateType
{
    public class LoadAllEstimateType : ServiceResponse
    {
        public IList<EstimateTypeEntity> EstimateTypes { get; set; }
        public LoadAllEstimateType()
        {
            EstimateTypes = new List<EstimateTypeEntity>();
        }
    }
}

using AMS.Models.DomainModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Admin.Dist
{
    public class GetAllDistrictResponse : ServiceResponse
    {
        public IList<DistEntity> Districts { get; set; }
        public GetAllDistrictResponse()
        {
            Districts = new List<DistEntity>();
        }
    }
}

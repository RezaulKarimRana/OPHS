using AMS.Models.DomainModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Admin.Thana
{
    public class GetThanaByDistIDResponse : ServiceResponse
    {
        public IList<ThanaEntity> ThanaS { get; set; }

        public GetThanaByDistIDResponse()
        {
            ThanaS = new List<ThanaEntity>();
        }
    }
}

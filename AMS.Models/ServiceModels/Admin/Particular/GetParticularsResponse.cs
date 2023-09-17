using AMS.Models.DomainModels;
using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Admin.Particular
{
    public class GetParticularsResponse : ServiceResponse
    {
        public List<ParticularEntity> Particulars { get; set; }

        public GetParticularsResponse()
        {
            Particulars = new List<ParticularEntity>();
        }
    }
}

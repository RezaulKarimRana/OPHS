using System.Collections.Generic;
using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.Sessions
{
    public class SessionLog
    {
        public SessionLogEntity Entity { get; set; }

        public List<SessionLogEvent> Events { get; set; }
    }
}

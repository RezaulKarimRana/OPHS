using System.Collections.Generic;
using Models.DomainModels;

namespace Models.ServiceModels.Admin.Sessions
{
    public class SessionLog
    {
        public SessionLogEntity Entity { get; set; }

        public List<SessionLogEvent> Events { get; set; }
    }
}

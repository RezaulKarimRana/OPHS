using System.Collections.Generic;
using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.SessionEvents
{
    public class GetSessionEventsResponse : ServiceResponse
    {
        public List<SessionEventEntity> SessionEvents { get; set; }

        public GetSessionEventsResponse()
        {
            SessionEvents = new List<SessionEventEntity>();
        }
    }
}

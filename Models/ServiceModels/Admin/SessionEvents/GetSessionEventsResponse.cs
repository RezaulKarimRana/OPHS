using System.Collections.Generic;
using Models.DomainModels;

namespace Models.ServiceModels.Admin.SessionEvents
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

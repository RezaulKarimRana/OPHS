using System;
using System.Collections.Generic;
using Models.DomainModels;

namespace Models.ServiceModels.Admin.Sessions
{
    public class SessionLogEvent
    {
        public SessionEventEntity Event { get; set; }

        public Dictionary<string, string> Info { get; set; }

        public DateTime Event_Date { get; set; }

        public SessionLogEvent()
        {
            Info = new Dictionary<string, string>();
        }
    }
}

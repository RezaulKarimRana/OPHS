using System;
using System.Collections.Generic;
using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.Sessions
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

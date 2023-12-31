using System;
using Models.DomainModels;

namespace Models.ServiceModels.Admin.Sessions
{
    public class Session
    {
        public SessionEntity Entity { get; set; }

        public string Username { get; set; }

        public DateTime? Last_Session_Log_Date { get; set; }

        public DateTime? Last_Session_Event_Date { get; set; }
    }
}

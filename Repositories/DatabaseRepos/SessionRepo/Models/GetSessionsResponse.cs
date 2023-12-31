using System;
using Models.DomainModels;

namespace Repositories.DatabaseRepos.SessionRepo.Models
{
    public class GetSessionsResponse : SessionEntity
    {
        public string Username { get; set; }

        public DateTime? Last_Session_Log_Date { get; set; }

        public DateTime? Last_Session_Event_Date { get; set; }

    }
}

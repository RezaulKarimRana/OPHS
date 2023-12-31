using System.Collections.Generic;

namespace Models.ServiceModels.Admin.Sessions
{
    public class GetSessionsResponse : ServiceResponse
    {
        public List<Session> Sessions { get; set; }

        public string SelectedFilter { get; set; }

        public GetSessionsResponse()
        {
            Sessions = new List<Session>();
        }
    }
}

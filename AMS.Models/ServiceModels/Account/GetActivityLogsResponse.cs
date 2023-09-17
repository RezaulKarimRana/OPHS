using System.Collections.Generic;

namespace AMS.Models.ServiceModels.Account
{
    public class GetActivityLogsResponse : ServiceResponse
    {
        public List<ActivityLog> ActivityLogs { get; set; }

        public GetActivityLogsResponse()
        {
            ActivityLogs = new List<ActivityLog>();
        }
    }
}

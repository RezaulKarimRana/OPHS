using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.SessionEvents
{
    public class GetSessionEventResponse : ServiceResponse
    {
        public SessionEventEntity SessionEvent { get; set; }
    }
}

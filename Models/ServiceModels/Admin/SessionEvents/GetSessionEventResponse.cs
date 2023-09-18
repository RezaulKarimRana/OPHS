using Models.DomainModels;

namespace Models.ServiceModels.Admin.SessionEvents
{
    public class GetSessionEventResponse : ServiceResponse
    {
        public SessionEventEntity SessionEvent { get; set; }
    }
}

using AMS.Models.DomainModels;

namespace AMS.Models.ServiceModels.Admin.Sessions
{
    public class GetSessionResponse : ServiceResponse
    {
        public UserEntity User { get; set; }

        public SessionEntity Session { get; set; }
    }
}

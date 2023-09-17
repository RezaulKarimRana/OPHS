using AMS.Models.DomainModels;

namespace AMS.Models.ManagerModels.Session
{
    public class GetSessionResponse
    {
        public int SessionLogId { get; set; }

        public bool IsDebug { get; set; }

        public SessionEntity SessionEntity { get; set; }
    }
}

using Models.DomainModels;

namespace Models.ManagerModels.Session
{
    public class GetSessionResponse
    {
        public int SessionLogId { get; set; }

        public bool IsDebug { get; set; }

        public SessionEntity SessionEntity { get; set; }
    }
}

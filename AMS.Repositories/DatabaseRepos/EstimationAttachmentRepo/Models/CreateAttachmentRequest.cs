using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.EstimationAttachmentRepo.Models
{
    public class CreateAttachmentRequest : CreatedBy
    {
        public string URL { get; set; }
        public string FileName { get; set; }
        public int Estimation_Id { get; set; }
    }
}

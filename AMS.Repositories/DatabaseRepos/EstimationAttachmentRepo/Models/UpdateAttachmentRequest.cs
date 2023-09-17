using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.EstimationAttachmentRepo.Models
{
    public class UpdateAttachmentRequest : UpdatedBy
    {
        public string URL { get; set; }
        public string FileName { get; set; }
    }
}

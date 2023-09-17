using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.DistRepo.Models
{
    public class CreateDistRequest : CreatedBy
    {
        public string Name { get; set; }
        public int Division_Id { get; set; }
    }
}

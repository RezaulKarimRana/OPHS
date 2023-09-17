using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.DistRepo.Models
{
    public class UpdateDistRequest : UpdatedBy
    {
        public string Name { get; set; }
        public int Division_Id { get; set; }
    }
}

using AMS.Repositories.DatabaseRepos.Common;

namespace AMS.Repositories.DatabaseRepos.ThanaRepo.Models
{
    public class CreateThanaRequest : CreatedBy
    {
        public string Name { get; set; }
        public int District_Id { get; set; }
    }
}

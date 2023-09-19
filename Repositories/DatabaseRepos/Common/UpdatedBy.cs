namespace Repositories.DatabaseRepos.Common
{
    public abstract class UpdatedBy
    {
        public int Id { get; set; }

        public int Updated_By { get; set; }
    }
}

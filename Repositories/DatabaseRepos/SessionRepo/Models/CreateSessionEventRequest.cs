namespace AMS.Repositories.DatabaseRepos.SessionRepo.Models
{
    public class CreateSessionEventRequest
    {
        public string Key { get; set; }

        public string Description { get; set; }

        public int Created_By { get; set; }
    }
}
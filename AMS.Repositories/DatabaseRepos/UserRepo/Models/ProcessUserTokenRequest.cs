using System;

namespace AMS.Repositories.DatabaseRepos.UserRepo.Models
{
    public class ProcessUserTokenRequest
    {
        public Guid Guid { get; set; }

        public int Updated_By { get; set; }
    }
}

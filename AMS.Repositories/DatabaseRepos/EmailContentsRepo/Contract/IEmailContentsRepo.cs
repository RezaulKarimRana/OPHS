using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EmailContentsRepo.Contract
{
    public interface IEmailContentsRepo
    {
        Task<int> SaveForEmailServer(CreateEmailContantRequest request);
    }
}

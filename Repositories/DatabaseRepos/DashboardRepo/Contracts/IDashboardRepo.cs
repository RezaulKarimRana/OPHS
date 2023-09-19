using System.Threading.Tasks;
using Repositories.DatabaseRepos.DashboardRepo.Models;

namespace Repositories.DatabaseRepos.DashboardRepo.Contracts
{
    public interface IDashboardRepo
    {
        Task<GetDashboardResponse> GetDashboard();
        Task<GetCountForAllPendingParkingsForNav> GetNavBarCount(int userId);
    }
}

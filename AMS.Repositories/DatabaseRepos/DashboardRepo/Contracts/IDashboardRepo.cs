using System.Threading.Tasks;
using AMS.Repositories.DatabaseRepos.DashboardRepo.Models;

namespace AMS.Repositories.DatabaseRepos.DashboardRepo.Contracts
{
    public interface IDashboardRepo
    {
        Task<GetDashboardResponse> GetDashboard();
        Task<GetCountForAllPendingParkingsForNav> GetNavBarCount(int userId);
    }
}

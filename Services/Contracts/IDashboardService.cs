using System.Threading.Tasks;
using Models.ServiceModels.Dashboard;

namespace AMS.Services.Contracts
{
    public interface IDashboardService
    {
        Task<GetIndexDashBoardResponse> GetIndexDashBoard();
        Task<GetCountForAllPendingParkingForNavService> GetNavBarCount();
    }
}

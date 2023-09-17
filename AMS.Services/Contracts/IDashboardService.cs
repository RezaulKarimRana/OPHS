using System.Threading.Tasks;
using AMS.Models.ServiceModels.Dashboard;

namespace AMS.Services.Contracts
{
    public interface IDashboardService
    {
        Task<GetIndexDashBoardResponse> GetIndexDashBoard();
        Task<GetCountForAllPendingParkingForNavService> GetNavBarCount();
    }
}

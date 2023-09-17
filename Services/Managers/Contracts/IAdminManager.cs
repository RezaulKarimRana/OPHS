using System.Threading.Tasks;
using AMS.Models.ManagerModels.Admin;

namespace AMS.Services.Managers.Contracts
{
    public interface IAdminManager
    {
        Task<CheckForAdminUserResponse> CheckForAdminUser();
    }
}

using System.Threading.Tasks;
using Models.ManagerModels.Admin;

namespace AMS.Services.Managers.Contracts
{
    public interface IAdminManager
    {
        Task<CheckForAdminUserResponse> CheckForAdminUser();
    }
}

using System.Threading.Tasks;
using Models.ManagerModels.Admin;

namespace Services.Managers.Contracts
{
    public interface IAdminManager
    {
        Task<CheckForAdminUserResponse> CheckForAdminUser();
    }
}

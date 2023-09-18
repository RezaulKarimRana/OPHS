using System.Threading.Tasks;
using Models.ServiceModels.Admin;

namespace AMS.Services.Admin.Contracts
{
    public interface IAdminService
    {
        Task<CheckIfCanCreateAdminUserResponse> CheckIfCanCreateAdminUser();

        Task<CreateAdminUserResponse> CreateAdminUser(CreateAdminUserRequest request);
    }
}

using System.Threading.Tasks;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Users;
using AMS.Repositories.DatabaseRepos.UserRepo.Models.User;

namespace AMS.Services.Admin.Contracts
{
    public interface IUserService
    {
        Task<GetUsersResponse> GetUsers();

        Task<GetUsersWithDepartmentNameRequest> GetUsersWithDepartmentService();

        Task<DisableUserResponse> DisableUser(DisableUserRequest request);

        Task<EnableUserResponse> EnableUser(EnableUserRequest request);

        Task<UnlockUserResponse> UnlockUser(UnlockUserRequest request);

        Task<GetUserResponse> GetUser(GetUserRequest request);

        Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request);

        Task<CreateUserResponse> CreateUser(CreateUserRequest request);

        Task<ConfirmRegistrationResponse> ConfirmRegistration(ConfirmRegistrationRequest request);

        Task<GenerateResetPasswordUrlResponse> GenerateResetPasswordUrl(GenerateResetPasswordUrlRequest request);

        Task<SendResetPasswordEmailResponse> SendResetPasswordEmail(SendResetPasswordEmailRequest request);
        Task<UserEntity> GetById(int id);
        Task<UserDepartmentResponse> GetUserAndDepartmentByIdService(int userId);
        Task<string> getUserEmailAddressByDepartmentId(int departmentId);
        Task<Follower> GetFollowersByUserId(int userId);
    }
}

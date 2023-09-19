using System.Threading.Tasks;
using Models.DomainModels;
using Models.ServiceModels.Admin.Users;
using Repositories.DatabaseRepos.UserRepo.Models.User;

namespace Services.Admin.Contracts
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

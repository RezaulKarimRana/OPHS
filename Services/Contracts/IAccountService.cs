using System.Threading.Tasks;
using Models.ServiceModels;
using Models.ServiceModels.Account;

namespace AMS.Services.Contracts
{
    public interface IAccountService
    {
        Task<LoginResponse> Login(LoginRequest request);

        Task<RegisterResponse> Register(RegisterRequest request);

        Task<ActivateAccountResponse> ActivateAccount(ActivateAccountRequest request);

        Task<ValidateResetPasswordTokenResponse> ValidateResetPasswordToken(ValidateResetPasswordTokenRequest request);

        Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request);

        Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request, string emailBody, string password);

        Task<UpdatePasswordResponse> UpdatePassword(UpdatePasswordRequest request);

        Task Logout();

        Task<GetProfileResponse> GetProfile();

        Task<UpdateProfileResponse> UpdateProfile(UpdateProfileRequest request);

        Task<DuplicateUserCheckResponse> DuplicateUserCheck(DuplicateUserCheckRequest request);

        Task<DuplicateRoleCheckResponse> DuplicateRoleCheck(DuplicateRoleCheckRequest request);

        Task<GetActivityLogsResponse> GetActivityLogs(GetActivityLogsRequest request);

        Task<SendFeedbackResponse> SendFeedback(SendFeedbackRequest request);
    }
}

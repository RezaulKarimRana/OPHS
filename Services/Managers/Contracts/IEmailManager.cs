using System.Threading.Tasks;
using AMS.Models.ServiceModels.Email;

namespace AMS.Services.Managers.Contracts
{
    public interface IEmailManager
    {
        Task SendAccountActivation(SendAccountActivationRequest request);

        Task SendResetPassword(SendResetPasswordRequest request);

        Task SendFeedback(SendFeedbackRequest request);
    }
}

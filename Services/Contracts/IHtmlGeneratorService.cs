using System.Threading.Tasks;

namespace AMS.Services.Contracts
{
    public interface IHtmlGeneratorService
    {
        Task<string> getPasswordResetEmailBody(string password);

    }
}

using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IHtmlGeneratorService
    {
        Task<string> getPasswordResetEmailBody(string password);

    }
}

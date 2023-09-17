using System.Threading.Tasks;
using AMS.Infrastructure.Email.Models;

namespace AMS.Infrastructure.Email.Contracts
{
    public interface IEmailProvider
    {
        Task Send(SendRequest request);
    }
}

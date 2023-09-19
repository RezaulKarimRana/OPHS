using System.Threading.Tasks;
using Infrastructure.Email.Models;

namespace Infrastructure.Email.Contracts
{
    public interface IEmailProvider
    {
        Task Send(SendRequest request);
    }
}

using AMS.Models.ServiceModels.Admin.Estimation;
using System.Threading.Tasks;

namespace AMS.Services.Admin.Contracts
{
    public interface IEstimationService
    {
        Task<EstimationNameListGetResponse> GetAllEstimationNameListService();
    }
}

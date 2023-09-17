using AMS.Repositories.DatabaseRepos.ProcurementApproval.Models;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ProcurementApproval.Contracts
{
    public interface IProcurementApprovalRepo
    {
        Task<int> CreateProcurementApproval(CreateProcurementApprovalRequest request);
        Task<GetProcurementApprovalResponse> GetProcurementApprovalByEstimateId(int estimateId);
        Task DeleteProcurementApprovalByEstimateID(int estimateID);
    }
}

using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Contracts
{
    public interface IEstimateApproverChangeHistoryRepo
    {
        Task<int> CreateEstimateApproverChangeHistory(CreateEstimateApproverChangeHistoryRequest request);
        Task<List<EstimateApproverChageHistoryEntity>> GetAllEstimateApproverChangeHistory();
        Task<EstimateApproverChageHistoryEntity> GetSingleEstimateApproverChangeHistory(int id);
    }
}

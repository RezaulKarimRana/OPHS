using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.Settlement;

namespace AMS.Repositories.DatabaseRepos.SettlementItemRepo
{
    public interface ISettlementItemRepo
    {
        Task<List<EstimateSettleCompleteItem>> getSettlementItemsByEstimateId(int userId, int departmentId, int estimateId);

        Task<List<EstimateSettleCompleteItem>> getSettlementItemsBySettlementId(int userId, int departmentId,
            int settlementId);
        Task<int> CreateOrModifyEstimateSettleItem(CreateSettlementItemRequest request);
        Task<int> CreateOrModifySettleItem(CreateSettlementItemRequest request);

    }
}

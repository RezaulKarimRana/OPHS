using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateMemo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using AMS.Models.ServiceModels.Memo;
using AMS.Models.ViewModel;

namespace AMS.Repositories.DatabaseRepos.EstimateMemo.Contracts
{
    public interface IEstimateMemoEntityRepo
    {
        Task<IList<PendingMemo>> LoadAllPendingMemo();
        Task<List<EstimationSettleItemDetailDTO>> LoadEstimationSettleItemDetailsByEstimationId(int estimationId);
        Task<List<EstimateSettleItemDetails>> LoadSettleItemDetailsBySettleItemAndEstimationId(int estimationId,
            int estimateSettleItem);
        Task<int> CreateEstimationMemo(AddBudgetEstimationMemoDTO request, int userId);
        Task<EstimateMemoEntity> GetEstimateMemoEntityById(int id);
        Task<EstimationInfoForMemo> EstimationMemoInfoDetailsById(int id);
        Task<EmailSenderInfo> getMemoRelatedEmailAddressByMemo(int memoId);
        Task<MemoSummaryVM> LoadMemoSummary(int userId);
        Task<MemoSummaryVM> LoadApproverMemoSummary(int userId);
        Task<List<MemoVM>> LoadMemoByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE);
        Task<List<MemoVM>> LoadApproverMemoByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE);
    }
}

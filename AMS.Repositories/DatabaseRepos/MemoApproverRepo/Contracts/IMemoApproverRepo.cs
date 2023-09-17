using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateMemo.Models;
using AMS.Repositories.DatabaseRepos.MemoApproverRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.MemoApproverRepo.Contracts
{
    public interface IMemoApproverRepo
    {
        Task<int> CreateEstimateApproverForMemo(CreateMemoApproverRequest request);
        Task<IList<PendingMemo>> LoadAllPendingMemoApprovalForAUser(int user_Id);
        Task<IList<EstimateMemoDetails>> LoadAllRunningMemoApprovalForAUser(int user_Id, int pageNumber, int pageSize,string whereClause);
        Task<int> CheckIsValidToShowInMemoParking(int estimateMemoId, int priority);
        Task<List<MemoApproverDetailsDTO>> LoadMemoApproverDetailsByMemoId(int memoId);
        Task<List<MemoApproverEntity>> LoadLatestPendingMemoApproverByMemoId(int memoId);
        Task<MemoApproverEntity> GetLatestPendingApproverByMemoIdAndUserId(int memoId, int userId);
        Task<List<MemoApproverEntity>> GetLatestPendingApproveresOfAMemo(int memoId);
        Task UpdateMemoApproverOnApproval(UpdateMemoApprover requestDto);
        Task<List<LoadMemoApproverFeedBackDetails>> LoadMemoApproverFeedBackDetails(int memoId);
        Task<List<ApproverDetailsDTO>> LoadApproverDetailsByMemo(int memoId);
    }
}

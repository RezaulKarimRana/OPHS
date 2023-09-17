using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.EstimateMemo.Models;
using AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using AMS.Models.ServiceModels.Memo;

namespace AMS.Services.Memo.Contracts
{
    public interface IMemoService
    {
        Task<List<PendingMemo>> LoadAllPendingMemoService();
        Task<EstimationReferenceEntity> GetEstimationReferenceByIdService(int id);
        Task<EstimationReferenceEntity> GetEstimationReferenceByEstimateService(int estimateId);
        Task<List<EstimationSettleItemDetailDTO>> LoadEstimationSettleItemDetailsByEstimationIdService(int estimationId);
        Task<List<EstimateSettleItemDetails>> LoadSettleItemDetailsBySettleItemAndEstimationIdService(int estimationId, int estimateSettleItem);
        Task<int> SaveEstimationMemoService(AddBudgetEstimationMemoDTO request);
        Task<int> SaveEstimateMemoAttachment(CreateAttachmentForMemoRequest attachReq);
        Task<IList<PendingMemo>> LoadAllPendingMemoApprovalForAUserService(int user_Id);
        Task<IList<EstimateMemoDetails>> LoadAllRunningMemoApprovalForAUserService(int user_Id, int pageNumber, int pageSize, string whereClause);
        Task<bool> CheckIsValidToShowInMemoParkingService(int estimateMemoId, int priority);
        Task<EstimateMemoEntity> GetEstimateMemoEntityByIdService(int id);
        Task<EstimationInfoForMemo> EstimationMemoInfoDetailsByIdService(int id);
        Task<List<EstimateMemoAttachmentsEntity>> LoadMemoAttachmentsByMemoService(int estimateMemoId);
        Task<List<MemoApproverDetailsDTO>> LoadMemoApproverDetailsByMemoIdService(int memoId);
        Task<List<MemoApproverEntity>> LoadLatestPendingMemoApproverByMemoIdService(int memoId);
        Task<MemoApproverEntity> GetLatestPendingApproverByMemoIdAndUserIdService(int memoId, int userId);
        Task<IList<MemoApproverEntity>> GetLatestPendingApproveresOfAMemoService(int memoId);
        Task<bool> SaveApproverApprovalFeedBack(int memoId, string approverFeedBackStatus, string approverFeedBack, bool isFinalApproved);
        Task<IList<LoadMemoApproverFeedBackDetails>> LoadMemoApproverFeedBackDetailsService(int memoId);
        Task<List<ApproverDetailsDTO>> LoadApproverDetailsByMemoService(int memoId);
        Task<MemoSummaryVM> LoadMemoSummary();
        Task<MemoSummaryVM> LoadApproverMemoSummary();
        Task<List<MemoVM>> LoadMemoByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE);
        Task<List<MemoVM>> LoadApproverMemoByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE);
        Task<bool> DeleteAttachmentsById(int id);
    }
}

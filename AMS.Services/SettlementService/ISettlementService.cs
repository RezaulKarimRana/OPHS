using System.Collections.Generic;
using System.Threading.Tasks;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Settlement;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.SettlementRepo.Models;

namespace AMS.Services.SettlementService
{
    public interface ISettlementService
    {
        Task<SettlementInitModel> GetSettlementInitData();
        Task<int> SaveSettlementData(SettlmentDTO requestDto, int status);
        Task<CreateSettlementRequest> getDraftSettlement(int estimateId);
        Task<CreateSettlementRequest> getSettlementBySettlementId(int settlementId);
        Task<List<SettlementVM>> LoadAllSettlementForFollower(int userId);
        Task<List<SettlementApproverDetails>> LoadSettlementApproverDetailsBySettlementId(int settlementId);
        Task<List<SettlementApproverFeedbackDetails>> LoadSettlementApproverRemarks(int settlementId);
        Task<int> UpdateSettlementStatusAndRelatedData(SettlementFeedback settlementFeedback);
        Task<List<SettlementVM>> LoadSettlementByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE);
        Task<List<SettlementApproverDetails>> GetAllApproverBySettlementIdForRollbackDraft(int settlementId);
        Task<int> SaveEstimateSettlementAttachment(CreateAttachmentForSettlementRequest attachReq);
        Task<List<ReadyForSettlementVM>> ReadyToSettlementList(int currentPageIndex, int pAGE_SIZE);
        Task<List<SettlementVM>> getInCompletedSettlementForCheckFinalSettlement(int estimationId);
        Task<List<EstimateSettlementAttachmentsEntity>> LoadSettlementAttachmentsBySettlementId(int settlementId);
        Task<SettlementSummaryVM> LoadSettlementSummary();
        Task<bool> DeleteAttachmentsById(int id);
    }
}

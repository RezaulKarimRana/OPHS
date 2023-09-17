using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.Settlement;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using AMS.Repositories.DatabaseRepos.SettlementRepo.Models;

namespace AMS.Repositories.DatabaseRepos.SettlementRepo
{
    public interface ISettlementRepo
    {
        Task<int> CreateOrModifySettlement(CreateSettlementRequest request);
        Task<CreateSettlementRequest> getDraftSettlmentId(int estimateId, int userId);
        Task<CreateSettlementRequest> getSettlementBySettlementId(int settlementId, int userId);
        Task<int> CreateSettlementApprover(CreateSettlementApproverRequest request);
        Task<List<SettlementVM>> LoadAllSettlementForFollower(int userId);
        Task<List<SettlementApproverDetails>> LoadSettlementApproverDetailsBySettlementId(int settlementId);
        Task<List<SettlementApproverFeedbackDetails>> LoadSettlementApproverFeedBackDetailsBySettlementId(int settlementId);
        Task<int> UpdateSettlementFeedback(SettlementFeedback request);
        Task<List<SettlementVM>> LoadSettlementByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE);
        Task<List<SettlementApproverDetails>> GetAllApproverBySettlementIdForRollbackDraft(int settlementId);
        Task<List<SettlementVM>> getInCompletedSettlementForCheckFinalSettlement(int userId, int departmentId,int estimationId);
        Task<int> CreateAttachmentForSettlement (CreateAttachmentForSettlementRequest request);
        Task<List<EstimateSettlementAttachmentsEntity>> LoadSettlementAttachmentsBySettlementId(int estimateSettlementId);
        Task<EmailSenderInfo> getSettlementRelatedEmailAddressBySettlement(int settlementId);
        Task<CreateSettlementRequest> getSettlementById(int settlementId);
        Task<List<ReadyForSettlementVM>> ReadyToSettlementList(int userId, int currentPageIndex, int pAGE_SIZE, int departmentId);
        Task<SettlementSummaryVM> LoadSettlementSummary(int userId);
        Task<bool> DeleteAttachmentsById(int id);
    }
}

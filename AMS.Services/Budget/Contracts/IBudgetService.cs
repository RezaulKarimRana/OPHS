using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using AMS.Models.ViewModel;
using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.BudgetEstimate.DraftedBudget;
using AMS.Repositories.DatabaseRepos.ProcurementApproval.Models;

namespace AMS.Services.Budget.Contracts
{
    public interface IBudgetService
    {
        Task<List<EstimateVM>> LoadAllPendingEstimateByUser(int userId);
        Task<bool> IsValidToShowInParking(int estimationId, int priority);
        Task<EstimationEntity> GetEstimateById(int id);
        Task<List<EstimateDetailsEntity>> LoadEstimateDetailByEstimationId(int estimationId);
        Task<List<ParticularWiseSummaryEntity>> LoadParticularWiseSummaryByEstimationId(int estimationId);
        Task<List<DepartmentWiseSummaryEntity>> LoadDepartmentWiseSummaryByEstimationId(int estimationId);
        Task<List<EstimateApproverEntity>> LoadEstimateApproverByEstimationId(int estimationId);
        Task<CreateEstimateResponse> CreateBudgeEstimation(CreateEstimateRequest estimateBudget);
        Task<CreateEstimateApproverResponse> CreateBudgetEstimateApprover(CreateEstimateApproverServiceRequest approverReq);
        Task<int> SaveBudgetData(AddBudgetEstimation requestDto);
        Task<int> DraftBudgetData(AddBudgetEstimation requestDto);
        Task<CreateEstimateDetailsServiceResponse> CreateBudgetEstimaeDetails(CreateEstimateDetailsServiceRequest detailsReq);
        Task<CreateDepartmentWiseSummaryServiceResponse> CreateBudgetDepartmentWiseSummary(CreateDepartmentWiseSummaryServiceRequest deptSummaryReq);
        Task<CreateParticularWiseSummaryResponse> CreateParticularWiseSummary(CreateParticularWiseSummaryServiceRequest partiSummaryReq);
        Task<CreateAttachmentServiceResponse> CreateEstimateAttachment(CreateAttachmentServiceRequest attachReq);
        Task<CreateEstimationHistoryServiceResponse> CreateEstimateHistory(CreateEstimationHistoryServiceRequest estiReq);
        Task<CreateEstimateApproverChangeHistoryServiceResponse> CreateEstimateApproverHistory(CreateEstimateApproverChangeHistoryServiceRequest approReq);
        Task<CreateEstimateDetailsChangeHistoryResponse> CreateEstimationDetailsHistry(CreateEstimateDetailsChangeHistoryServiceRequest detailHisReq);
        Task<List<EstimateEditVM>> LoadAllPendingEstimate(int userId, int currentPageIndex, int pAGE_SIZE);
        Task<List<EstimateEditVM>> LoadAllCompleteEstimate(int userId, int currentPageIndex, 
            int pAGE_SIZE, string whereClause, bool IsNotForCount = true);
        Task<List<EstimateEditVM>> LoadRejectedEstimate(int userId, int currentPageIndex, int pAGE_SIZE);
        Task<CreateDepartmentWiseSummaryHistoryResponse> CreateDepartmentWiseSummaryHistory(CreateDepartmentWiseSummaryHistoryServiceRequest deptSummaryHistoryReq);
        Task<CreateParticularWiseSummaryHistoryResponse> CreateParticularWiseSummaryHistory(CreateParticularWiseSummaryHistoryServiceRequest partiSummaryReq);
        Task<ParticularWiseSummaryEntity> GetParticularWiseSummaryByEstimationId(int estimationId);
        Task<DepartmentWiseSummaryEntity> GetDepartmentWiseSummaryByEstimationId(int estimationId);
        Task<SingleEstimationWithTypeResponse> GetOneEstimationWithType(int estiId);
        Task<GetProcurementApprovalResponse> GetProcurementApprovalByEstimateService(int estimateId);
        Task<List<ParticularWiseSummaryDetailsByEstimationId>> LoadParticularSummaryDetails(int estimationId);
        Task<List<DepartWiseSummaryDetailsByEstimationId>> LoadDepartmentSummaryDetails(int estimationId);
        void CompleteBudget(int id, int StatusCode, int completed);
        void CompleteApproverFeedback(int estimateId, int userId, string feedback, string remarks);
        Task<DisabledEstimationResponse> DisabledResponse(int estimationId);
        Task<BudgetEstimationResponse> UpdateEstimation(BudgetEstimationUpdateRequest request, int status);
        //Task<DeleteEstimateDetailsApproverAttachmentHistoryResponse> DeleteDetailApproverHistory(int estimateId);
        Task<EstimateEditVM> CastEstimateToVM(EstimationEntity estimate, List<EstimateApproverEntity> estimateApprovers, List<EstimateDetailsEntity> estimateDetails = null, List<ParticularWiseSummaryEntity> particularWiseSummary = null, List<DepartmentWiseSummaryEntity> departmentWiseSummary = null);
        Task<int> SaveEstimationDetailsApproversSummaries(AddBudgetEstimation request);
        Task<int> ReDraftEstimationDetailsApproversSummaries(AddBudgetEstimation request);
        Task<List<EstimationAttachmentEntity>> LoadAttachmentsByEstimateId(int estimateId);

        Task<GetDraftBudgetEstimationbyUserResponse> LoadDraftEstimation(int id, int start, int pAGE_SIZE);
        Task<GetDraftBudgetEstimationbyUserResponse> LoadOngoinEstimationByUserService(int userId);
        Task<GetDraftBudgetEstimationbyUserResponse> LoadCREstimation(int userId, int start, int pAGE_SIZE);
        Task ReDefineEstimationDetailsAndSummaries(AddBudgetEstimation request);
        Task ReDefineEsitmationDetailsApproversAndSummariesService(ModifyEstimationAndReAddOther request);
        Task<List<EstimationSearch>> GetSearchService(string keyWord);
        Task<AddBudgetEstimation> GetEstimationFullInfo(int estimateId);
        Task<List<EstimateEditVM>> LoadAllApprovedEstimateByUserDepartment(int userId, int currentPageIndex, int pAGE_SIZE);

        Task<EstimationInfoForMemo> EstimationInfoService(int estiId);

        Task<IList<DepartWiseSummaryDetailsForSettledEstimation>> LoadDeptSummaryForSettledEstimateByaEstimationService(int estimationId);
        Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>> LoadParticularWiseSummaryForAEstimationSettleService(int estimationId);

        Task<IList<DepartWiseSummaryDetailsForSettledEstimation>>
            LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(int estimationId);

        Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>>
            LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(int estimationId);
        Task<List<EstimateVM>> LoadAllEstimateByUserExceptPending(int userId);

        Task<List<EstimateVM>> LoadAllRejectedEstimateBySpecificUser(int userId);
        Task<List<EstimateVM>> LoadAllRunningEstimateBySpecificUser(int userId);
        Task<List<EstitmationApprovalInfo>> GetAllEstimationApprovalInfoByUserId();
        Task<List<DashboardTotalAmountVM>> GetAllBudgetAmountSumByUserId();

        Task<IList<DepartWiseSummaryDetailsForSettledEstimation>> LoadDeptSummeryForRunningSettlementBySettlementId(
            int settlementId, int estimationId);

        Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>>
            LoadParticularWiseSummaryForRunningSettlementBySettlementId(int settlementId, int estimationId);



    }
}

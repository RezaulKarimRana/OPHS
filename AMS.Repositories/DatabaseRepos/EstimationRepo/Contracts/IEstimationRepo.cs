using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Repositories.DatabaseRepos.EstimationRepo.Models;
using AMS.Repositories.DatabaseRepos.ProcurementApproval.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimationRepo.Contracts
{
    public interface IEstimationRepo
    {
        Task<int> CreateEstimation(CreateEstimationRequest request);
        Task<List<EstimationEntity>> GetAllEstimation();
        Task<List<GetAllEstimationSubjectNames>> GetAllEstimationSubjectNameList();
        Task<EstimationEntity> GetSingleEstimation(int id);
        Task UpdateEstimation(UpdateEstimationRequest request);
        Task DeleteEstimation(DeleteEstimationRequest request);
        Task<List<EstimateVM>> LoadAllPendingEstimateByUser(int userId);
        Task<int> CheckIsValidToShowInParking(int estimationId, int priority);
        Task<EstimationEntity?> GetById(int id);      
        Task<List<DraftedBudgetEstimationByUser>> LoadDraftEstimation(int id, int start, int pAGE_SIZE);
        Task<List<DraftedBudgetEstimationByUser>> LoadOngoinEstimationByUser(int userId);
        Task CompleteBudget(int id, int userId, int statusCode);
        Task<EstimationWithEstimationType> SingleEstimationWithType(int estiId);
        Task DisabledEstimation(DisableEstimation request);
        Task<List<EstimateEditVM>> LoadAllPendingEstimate(int userId, int currentPageIndex, int pAGE_SIZE, int UserId);
        Task<List<EstimateEditVM>> LoadAllCompleteEstimate(int userId,
            int currentPageIndex, int pAGE_SIZE, int UserId, string whereClause, bool IsNotForCount = true);
        Task<List<EstimateEditVM>> LoadAllCompleteEstimateExceptFinance(int userId, 
            int currentPageIndex, int pAGE_SIZE, int UserId, string whereClause, bool IsNotForCount = true);
        Task<List<EstimateEditVM>> LoadRejectedEstimate(int userId, int currentPageIndex, int pAGE_SIZE, int UserId);
        Task<List<DraftedBudgetEstimationByUser>> LoadCREstimation(int userId, int start, int pAGE_SIZE);
        Task UpdateTotalPrice(UpdateEstimateTotalPriceRequest request);
        Task IncreaseIncrementalValue(int updatedUserId);
        Task<AutoIncrementedValueTableEntity> GetIncrementedValue();
        Task<List<EstimationSearch>> GetEstimateSearch(string keyWord, int userID);
        
        Task<List<EstimateEditVM>> LoadAllApprovedEstimateByUserDepartment(int userId, int currentPageIndex, int pAGE_SIZE, int UserId, int departmentId);
        Task<EstimationInfoForMemo> EstimationInfo(int estiId);
        Task<List<EstimateVM>> LoadAllEstimateByUserExceptPending(int userId);
        Task<List<EstimateVM>> LoadAllRejectedEstimateBySpecificUser(int userId);
        Task<List<EstimateVM>> LoadAllRunningEstimateBySpecificUser(int userId);
        Task<List<EstitmationApprovalInfo>> load_all_approval_data_forUserByUserID(int userId);
        Task<List<DashboardTotalAmountVM>> GetAllBudgetAmountSumByUserId(int userId);

    }
}
 
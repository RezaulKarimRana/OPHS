using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.BudgetEstimate.DraftedBudget;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Services.Budget.Contracts
{
    public interface IDraftedBudgetService
    {
        //Task<GetDraftBudgetEstimationbyUserResponse> GetDraftBudgetEstimation();
        Task<GetDraftBudgetEstimationResponse> GetSingleDraft(int estimateId);
        Task<List<EstimateApproverByEstimateId>> LoadEstimateApproverDetailsByEstimation(int estiId);
        Task<List<EstimationDetailsWithJoiningOtherTables>> LoadWholeEstimationDetailsByEstimation(int estiId);
    }
}

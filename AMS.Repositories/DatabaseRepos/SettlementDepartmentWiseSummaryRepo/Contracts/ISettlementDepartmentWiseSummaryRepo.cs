using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.SettlementDepartmentWiseSummaryRepo.Contracts
{
    public interface ISettlementDepartmentWiseSummaryRepo
    {
        Task<IList<DepartWiseSummaryDetailsForSettledEstimation>> LoadDeptSummaryForSettledEstimateByaEstimation(int estimationId);

        Task<IList<DepartWiseSummaryDetailsForSettledEstimation>>
            LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(int estimationId);

        Task<IList<DepartWiseSummaryDetailsForSettledEstimation>> LoadDeptSummaryForRunningSettlementBySettlementId(
            int settlementId, int estimationId);

    }
}

using AMS.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.SettlementParticularWiseSummaryRepo.Contracts
{
    public interface ISettlementParticularWiseSummaryRepo
    {
        Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>> LoadParticularWiseSummaryForAEstimationSettle(int estimationId);

        Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>>
            LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(int estimationId);

        Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>>
            LoadParticularWiseSummaryForRunningSettlementBySettlementId(int settlementId, int estimationId);

    }
}

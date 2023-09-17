using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Models.CustomModels;

namespace AMS.Services.SettlementItem
{
    public interface ISettlementItemService
    {
        Task<List<EstimateSettleCompleteItem>> getSettlementItemsByEstimateId(int estiId);
        Task<List<EstimateSettleCompleteItem>> getSettlementItemsBySettlementId(int settlementId);
    }
}

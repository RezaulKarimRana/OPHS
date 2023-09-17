using AMS.Models.CustomModels;
using AMS.Services.Budget.Contracts;
using AMS.Services.SettlementItem;
using AMS.Services.SettlementService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class ParticularWiseSummaryController : BaseController
    {
        private readonly IBudgetService _budgetService;
        private readonly ISettlementItemService _settlementItemService;
        private readonly ILogger<ParticularWiseSummaryController> _logger;
        public ParticularWiseSummaryController(IBudgetService budgetService, ILoggerFactory loggerFactory, ISettlementItemService settlementService)
        {
            _budgetService = budgetService;
            _settlementItemService = settlementService;
            _logger = loggerFactory.CreateLogger<ParticularWiseSummaryController>();
        }

        [HttpGet]
        public async Task<IActionResult> GetParticularSummaryDetails(int estimationId)
        {
            try
            {
                var response = await _budgetService.LoadParticularSummaryDetails(estimationId);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetParticularSummaryForASettledEstimation(int estimationId)
        {
            try
            {
                var response = new List<ParticularWiseSummaryDetailsForSettledEstimationIncludeTADA>();
                var particular = await _budgetService.LoadParticularWiseSummaryForAEstimationSettleService(estimationId);
                var items = await _settlementItemService.getSettlementItemsByEstimateId(estimationId);
                foreach (var item in particular)
                {
                    var tadaAmount = items.Where(x => x.ParticularId == item.ParticularId && x.ItemCategory == "TA/DA" && x.DepartmentId != 7 && x.DepartmentId != 41).Sum(s => s.TotalPrice);
                    response.Add(new ParticularWiseSummaryDetailsForSettledEstimationIncludeTADA
                    {
                        ParticularId = item.ParticularId,
                        ParticularName = item.ParticularName,
                        TotalCost = item.TotalCost,
                        TotalRunningCost = item.TotalRunningCost,
                        TotalBudget = item.TotalBudget,
                        TotalTADABudget = tadaAmount
                    });
                }
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetParticularSummaryForASettledEstimationWithBudgetData(int estimationId)
        {
            try
            {
                var response = await _budgetService.LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(estimationId);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetParticularSummaryForRunningSettlementBySettlementId(int settlementId,int estimationId)
        {
            try
            {
                var response = await _budgetService.LoadParticularWiseSummaryForRunningSettlementBySettlementId(settlementId, estimationId);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}

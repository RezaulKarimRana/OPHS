using AMS.Models.CustomModels;
using AMS.Services.Budget.Contracts;
using AMS.Services.SettlementItem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class DepartmentWiseSummaryOfBudgetEstimationController : Controller
    {
        private readonly IBudgetService _budgetService;
        private readonly ISettlementItemService _settlementItemService;
        private readonly ILogger<DepartmentWiseSummaryOfBudgetEstimationController> _logger;

        public DepartmentWiseSummaryOfBudgetEstimationController(IBudgetService budgetService, ILoggerFactory loggerFactory,ISettlementItemService settlementItemService)
        {
            _budgetService = budgetService;
            _settlementItemService = settlementItemService;
            _logger = loggerFactory.CreateLogger<DepartmentWiseSummaryOfBudgetEstimationController>();
        }

        [HttpGet]
        public async Task<IActionResult> LoadDepartmentWiseSummaryDetailsByEstimationId(int estimationId)
        {
            try
            {
                var response = await _budgetService.LoadDepartmentSummaryDetails(estimationId);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            } 
        }

        [HttpGet]
        public async Task<IActionResult> LoadDepartmentWiseSummaryForASettledEstimation(int estimationId)
        {
            try
            {
                var response = new List<DepartWiseSummaryDetailsForSettledEstimationIncludeTADA>();
                var dept = await _budgetService.LoadDeptSummaryForSettledEstimateByaEstimationService(estimationId);
                var items = await _settlementItemService.getSettlementItemsByEstimateId(estimationId);
                foreach (var item in dept)
                {
                    var tadaAmount = items.Where(x => x.DepartmentId == item.DepartmentId && x.ItemCategory == "TA/DA").Sum(s => s.TotalPrice);
                    if(item.DepartmentId != 7 && item.DepartmentId != 41)
                    response.Add(new DepartWiseSummaryDetailsForSettledEstimationIncludeTADA
                    {
                        DepartmentId = item.DepartmentId,
                        DepartmentName = item.DepartmentName,
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
        public async Task<IActionResult> LoadDepartmentWiseSummaryForASettledEstimationWithBudgetData(int estimationId)
        {
            try
            {
                var response = await _budgetService.LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(estimationId);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadDepartmentWiseSummaryForRunningSettlementBySettlementId(int settlementId , int estimationId)
        {
            try
            {
                var response = await _budgetService.LoadDeptSummeryForRunningSettlementBySettlementId(settlementId, estimationId);
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

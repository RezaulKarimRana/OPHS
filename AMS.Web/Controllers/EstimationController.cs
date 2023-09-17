using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class EstimationController : Controller
    {
        private readonly IEstimationService _estimationService;
        private readonly IBudgetService _budgetService;
        private readonly ILogger<EstimationController> _logger;

        public EstimationController(IEstimationService estimationService, IBudgetService budgetService, ILoggerFactory loggerFactory)
        {
            _estimationService = estimationService;
            _budgetService = budgetService;
            _logger = loggerFactory.CreateLogger<EstimationController>();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllEstimationName()
        {
            var response = await _estimationService.GetAllEstimationNameListService();
            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetEstimationInfobyId(int estimationId)
        {
            try
            {
                var response = await _budgetService.GetOneEstimationWithType(estimationId); 
                return new JsonResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }     
            
        }

        [HttpGet]
        public async Task<IActionResult> GetProcurementApprovalEstimationInfobyEstimateId(int estimationId)
        {
            try
            {
                var response = await _budgetService.GetProcurementApprovalByEstimateService(estimationId);
                return new JsonResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetAllEstimationApprovalInfoByUserId()
        {

            var response = await _budgetService.GetAllEstimationApprovalInfoByUserId();
            return new JsonResult(response);
        }
    }
}

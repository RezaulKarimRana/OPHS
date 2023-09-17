using AMS.Models.ViewModel;
using AMS.Services.Budget.Contracts;
using AMS.Services.Managers.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;

namespace AMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class DraftedBudgeEstimationController : Controller
    {
        private readonly IDraftedBudgetService _draftbudgetService;
        private readonly IBudgetService _budgetService;
        private readonly ISessionManager _sessionManager;
        private readonly ILogger<DraftedBudgeEstimationController> _logger;
        IDataProtector _protector;

        public DraftedBudgeEstimationController(IDraftedBudgetService draftedbudgetService,
            IBudgetService budgetService, ILoggerFactory loggerFactory,
            ISessionManager sessionManager, IDataProtectionProvider provider)
        {
            _draftbudgetService = draftedbudgetService;
            _budgetService = budgetService;
            _sessionManager = sessionManager;
            _protector = provider.CreateProtector(GetType().FullName);
            _logger = loggerFactory.CreateLogger<DraftedBudgeEstimationController>();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllApproverByEstimate(int estiId)
        {
            var response = await _draftbudgetService.LoadEstimateApproverDetailsByEstimation(estiId);
            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEstimationDetailsByEstimate(int estiId)
        {
            var response = await _draftbudgetService.LoadWholeEstimationDetailsByEstimation(estiId);
            return new JsonResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> ReStructedEstimation(string dto) //ModifyEstimationAndReAddOther
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<ModifyEstimationAndReAddOther>(dto);

                await _budgetService.ReDefineEsitmationDetailsApproversAndSummariesService(jsonData);
                return Json(1);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }

        //updateEstimation(estimationForUpdate);
        //deleteOldDetails(estimationId);
        //postEditedDataForBudgetEstimation(data, estimationAttachment);
    }
}

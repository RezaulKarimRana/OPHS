using System.Threading.Tasks;
using AMS.Models.ViewModel;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
using AMS.Services.Managers.Contracts;
using AMS.Services.SettlementItem;
using AMS.Services.SettlementService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SettlementItemController : Controller
    {
        private readonly ILogger<BudgetEstimationController> _logger;

        IDataProtector _protector;
        private readonly ISessionManager _sessionManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment hostingEnv;
        private readonly IDashboardService _dashboardService;
        private readonly ISettlementItemService _settlementItemService;
        private readonly ISettlementService _settlementService;

        public SettlementItemController(ILoggerFactory loggerFactory, IBudgetService budgetService,
            IWebHostEnvironment env,
            ISessionManager sessionManager, IDataProtectionProvider provider, IConfiguration configuration,
            IDashboardService dashboardService,
            ISettlementItemService settlementItemService,
            ISettlementService settlementService)
        {
            _logger = loggerFactory.CreateLogger<BudgetEstimationController>();

            _sessionManager = sessionManager;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
            _dashboardService = dashboardService;
            hostingEnv = env;
            _settlementItemService = settlementItemService;
            _settlementService = settlementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettlementItemsByEstimateId(int estiId)
        {
            var response = await _settlementItemService.getSettlementItemsByEstimateId(estiId);
            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetSettlementItemsBySattlementId(int settlementId)
        {
            var response = await _settlementItemService.getSettlementItemsBySettlementId(settlementId);
            return new JsonResult(response);
        }


    }
}
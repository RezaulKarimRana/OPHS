using AMS.Models.ServiceModels.Dashboard;
using AMS.Models.ViewModel;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
using AMS.Services.Managers.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SearchController : BaseController
    {
        private readonly ILogger<SearchController> _logger;
        private readonly IBudgetService _budgetService;
        private readonly ISessionManager _sessionManager;
        private readonly IConfiguration _configuration;
        private readonly IDashboardService _dashboardService;
        IDataProtector _protector;

        public SearchController(ILoggerFactory loggerFactory, IBudgetService budgetService,
            ISessionManager sessionManager, IDataProtectionProvider provider, IConfiguration configuration, IDashboardService dashboardService)
        {
            _logger = loggerFactory.CreateLogger<SearchController>();
            _budgetService = budgetService;
            _sessionManager = sessionManager;
            _configuration = configuration;
            _dashboardService = dashboardService;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
        }

        [HttpGet]
        public async Task<IActionResult> ShowEstimationSearchList(string keyWord = "")
        {
            try
            {
                var sessionUser = _sessionManager.GetUser();
                if (sessionUser == null)
                {
                    throw new Exception("user session expired");
                }

                var responseFromDB = await _budgetService.GetSearchService(keyWord);

                var response = from item in responseFromDB
                               select new EstimateSearchVM
                               {
                                   Id = _protector.Protect(item.Id.ToString()),
                                   EstimationTypeName = item.EstimationTypeName,
                                   EstimateSubject = item.EstimateSubject,
                                   EstimateIdentity = item.EstimateIdentity,
                                   EstimationStatus = item.EstimationStatus,
                                   Creator = item.Creator,
                                   CreateorFullName = item.CreateorFullName,
                                   StartDate = item.StartDate,
                                   EndDate = item.EndDate,
                                   TotalPrice = item.TotalPrice,
                                   CurrencyType = item.CurrencyType
                               };
                var responseForNav = await _dashboardService.GetNavBarCount();
                if (responseForNav.IsSuccessful)
                {
                    ViewData["CountPendingObject"] = new GetCountForAllPendingParkingForNavService
                    {
                        TotalDraftParking = responseForNav.TotalDraftParking,
                        TotalCompletedParking = responseForNav.TotalCompletedParking,
                        TotalPendingApprovalParking = responseForNav.TotalPendingApprovalParking,
                        TotalRollbackParking = responseForNav.TotalRollbackParking
                    };
                }
                return View(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

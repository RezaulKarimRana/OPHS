using AMS.Models.ServiceModels.Dashboard;
using AMS.Services.Admin.Contracts;
using AMS.Services.Contracts;
using AMS.Services.Managers.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AMS.Web.Attributes;
using AMS.Infrastructure.Authorization;
using AMS.Services.SettlementService;
using AMS.Models.ServiceModels.Settlement;

namespace AMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SettlementApproverController : BaseController
    {
        #region Instant Variable Declaration
        private readonly ILogger<SettlementApproverController> _logger;
        private readonly ISettlementService _settlementService;
        private readonly ISessionManager _sessionManager;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public IDataProtector _protector;
        private readonly IDashboardService _dashboardService;
        

        #endregion


        #region Constructor
        public SettlementApproverController(ILoggerFactory loggerFactory, ISessionManager sessionManager,
             IUserService userService,
             IDataProtectionProvider provider, IConfiguration configuration, IDashboardService dashboardService
             ,ISettlementService settlementService)
        {
            _logger = loggerFactory.CreateLogger<SettlementApproverController>();
            _sessionManager = sessionManager;
            _userService = userService;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
            _dashboardService = dashboardService;
            _settlementService = settlementService;
        }

        #endregion
        [AuthorizePermission(PermissionKeys.RunningBudgetForSepecificUser)]
        public async Task<IActionResult> FollowerSettlementList()
        {
            try
            {
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
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
            return View();
        }
        public async Task<IActionResult> LoadFollowerSettlements(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                var follower = await _userService.GetFollowersByUserId(user.Id);
                if(follower == null)
                {
                    return RedirectToHome();
                }
                List<SettlementVM> result = await _settlementService.LoadAllSettlementForFollower(follower.FollowerUserId);
                
                long recordsTotal = result.Count;
                long recordsFiltered = recordsTotal;
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                var data = new List<object>();
                foreach (var d in result)
                {
                    var str = new List<string>();
                    var id = d.SettlementId;

                    str.Add(sl.ToString());
                    str.Add(getConcateSettlementDetails(d.EstimateIdentifier, d.Subject,
                        d.CreatorFullName, d.SettlementInitiateDate));
                    str.Add(
                        getConcateAmount(d.TotalBudgedPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                            d.AllowableBudget.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                            d.AlreadySettle.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                            d.SettleAmount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))));

                    string actionButtons = "<a href='/Settlement/SettlementSummaryViewByEstimateId?id=" +
                                     _protector.Protect(d.EstimationId.ToString()) +
                                     "' class='btn btn-outline-info text-decoration-none'>";
                    actionButtons +=
                        "<i class='fa fa-list-alt' data-toggle='tooltip' data-placement='left' title='Settlement Summary'></i> ";
                    actionButtons += "Summary</a>";

                    str.Add(actionButtons);
                    data.Add(str);
                    sl++;
                }
                return Json(new
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    start = start,
                    length = length,
                    data = data
                });
            }

            catch (Exception e)
            {
                var data = new List<object>();
                _logger.LogError(e, e.Message);
                return Json(new
                {
                    draw = draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    start = start,
                    length = length,
                    data = data
                });
            }
        }
        private string getConcateSettlementDetails(
            string Identifier,
            string Subject,
            string SettlementCreatedBy,
            string settlementCreatedDate
        )
        {
            return
                $"<b style=\"color: LightSeaGreen;\">Identifier:</b>{Identifier} <br/>" +
                $"<b style=\"color: LightSeaGreen;\">Subject:</b>{Subject} <br/>" +
                $" <b style=\"color: LightSeaGreen;\">Settlement Initiator:</b> {SettlementCreatedBy} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> settlement Date:</b> {settlementCreatedDate} <br/>";
        }
        private string getConcateAmount(
            string totalPrice,
            string allowableBudget,
            string alreadySettledAmount,
            string settlementAmount
        )
        {
            return
                $"<b style=\"color: LightSeaGreen;\">Total Price:</b>{totalPrice} <br/>" +
                $"<b style=\"color: LightSeaGreen;\">Allowable Budget:</b>{allowableBudget} <br/>" +
                $" <b style=\"color: LightSeaGreen;\">Already Settle Amount:</b> {alreadySettledAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Settlement Amount:</b> {settlementAmount} <br/>";
        }
    }
}

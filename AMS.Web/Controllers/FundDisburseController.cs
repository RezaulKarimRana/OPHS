using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Dashboard;
using AMS.Models.ServiceModels.FundDisburse;
using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
using AMS.Services.FundDisburseService;
using AMS.Services.FundRequisitionService;
using AMS.Services.Managers.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class FundDisburseController : BaseController
    {

        #region Instant Variable Declaration
        private readonly ILogger<BudgetApproverController> _logger;
        private readonly IBudgetService _budgetService;
        private readonly ISessionManager _sessionManager;
        private readonly IItemService _itemService;
        private readonly IDepartmentService _departmentService;
        private readonly IThanaService _thanaService;
        private readonly IUserService _userService;
        private readonly IParticularService _particularService;
        private readonly IItemCategoryService _itemCategoryService;
        private readonly IDistService _distService;
        private readonly IBudgetApproverService _budgetApproverService;
        private readonly IConfiguration _configuration;
        public IDataProtector _protector;
        private readonly IDashboardService _dashboardService;
        private readonly IFundRequisitionService _fundRequisitionService;
        private readonly IFundDisburseService _fundDisburseService;

        #endregion


        #region Constructor
        public FundDisburseController(ILoggerFactory loggerFactory, IBudgetService budgetService, ISessionManager sessionManager,
             IItemService itemService, IDepartmentService departmentService, IThanaService thanaService, IUserService userService,
             IParticularService particularService, IItemCategoryService itemCategoryService, IDistService distService,
             IBudgetApproverService budgetApproverService, IDataProtectionProvider provider, IConfiguration configuration, IDashboardService dashboardService,
             IFundRequisitionService fundRequisitionService,
             IFundDisburseService fundDisburseService
            )
        {
            _logger = loggerFactory.CreateLogger<BudgetApproverController>();
            _budgetService = budgetService;
            _sessionManager = sessionManager;
            _itemService = itemService;
            _departmentService = departmentService;
            _thanaService = thanaService;
            _userService = userService;
            _particularService = particularService;
            _itemCategoryService = itemCategoryService;
            _distService = distService;
            _budgetApproverService = budgetApproverService;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
            _dashboardService = dashboardService;
            _fundRequisitionService = fundRequisitionService;
            _fundDisburseService = fundDisburseService;
        }

        #endregion
        [HttpGet]
        public async Task<IActionResult> NewFundDisburse(string id)
        {
            try
            {

                var fundRequisitionId = Convert.ToInt32(_protector.Unprotect(id));
                var fundRequisitionResponse = await _fundRequisitionService.GetFundRequisitionHistoryByFundRequisitionId(fundRequisitionId);
                var response = await _budgetService.GetOneEstimationWithType(fundRequisitionResponse.EstimatationId);
                var responseForNav = await _dashboardService.GetNavBarCount();
                ViewBag.fundRequisition = fundRequisitionResponse;
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



        [HttpGet]
        public async Task<IActionResult> GetFundDisburseHistoryByEstimateId(int estimationId)
        {
            try
            {
                var response = await _fundDisburseService.GetFundDisburseHistoryByEstimateId(estimationId);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
                throw;
            }
        }



        #region NEW FUND Disburse 
        [HttpPost]
        public async Task<JsonResult> PostNewFundDisburse(/*AddFundRequisition*/string fundDisburse)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<FundDisburse>(fundDisburse);
                var result = await _fundDisburseService.addFundDisburse(jsonData);
                return Json(result);
            }
            catch (Exception e)
            {
                //capture log
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }

        #endregion new Fund Disburse 


        #region  FUND Disburse Receive & Rollback

        [HttpGet]
        public async Task<IActionResult> FundReceivedByRequestor(string disburseID)
        {
            try
            {

                var fundDisburseId = Convert.ToInt32(_protector.Unprotect(disburseID));
                var fundDisburseResponse = await _fundDisburseService.GetFundDisburseHistoryByFundDisburseId(fundDisburseId);

                var responseForNav = await _dashboardService.GetNavBarCount();
                ViewBag.fundRequisition = await _fundRequisitionService.GetFundRequisitionHistoryByEstimationId(fundDisburseResponse.EstimatationId);
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
                return View(fundDisburseResponse);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<JsonResult> PostFundDisburseReceiveOrRollback(/*AddFundRequisition*/string fundDisburse)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<FundDisburse>(fundDisburse);
                var result = await _fundDisburseService.FundDisburseReceiveOrRollback(jsonData);
                return Json(result);
            }
            catch (Exception e)
            {
                //capture log
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }

        #endregion FUND Disburse Receive & Rollback

        #region  FUND  Re-Disburse By Finance
        [HttpGet]
        public async Task<IActionResult> FundReDisburseByFinance(string disburseID)
        {
            try
            {

                var fundDisburseId = Convert.ToInt32(_protector.Unprotect(disburseID));
                var fundDisburseResponse = await _fundDisburseService.GetFundDisburseHistoryByFundDisburseId(fundDisburseId);

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
                return View(fundDisburseResponse);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public async Task<JsonResult> PostFundReDisburseByFinance(/*AddFundRequisition*/string fundDisburse)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<FundDisburse>(fundDisburse);
                var result = await _fundDisburseService.FundReDisburse(jsonData);
                return Json(result);
            }
            catch (Exception e)
            {
                //capture log
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }

        #endregion FUND Disburse Receive & Rollback


        #region Fund Disburse Readonly View
        [HttpGet]
        public async Task<IActionResult> FundDisburseReadonlyDetails(string disburseID)
        {
            try
            {

                var fundDisburseId = Convert.ToInt32(_protector.Unprotect(disburseID));
                var fundDisburseResponse = await _fundDisburseService.GetFundDisburseHistoryByFundDisburseId(fundDisburseId);

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
                return View(fundDisburseResponse);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion


        #region Fund Disburse List for Requsition Department For Acknowledgement 
        [HttpGet]
        public async Task<IActionResult> PendingListForRequisitionDepartment()
        {
            try
            {
                var user = await _sessionManager.GetUser();

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

        [HttpPost]
        public async Task<IActionResult> PendingListForRequisitionDepartment(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();


                List<FundDisburseVM> resultSet = await _fundDisburseService.PendingFundDisburseListForAcknowledgement(0, start, PAGE_SIZE);
                //int count = resultSet.Count > 0 ? resultSet[0].TotalRow : 0;
                int count = 0;
                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet)
                {
                    var str = new List<string>();
                    var id = d.FundRequisitionId;

                    str.Add(sl.ToString());
                    //str.Add(d.EstimateType);
                    str.Add(d.EstimateIdentifier);
                    str.Add(d.EstimationSubject);
                    //str.Add(this.getConcateAmount(d.AllowableBudget, d.FundRequisitionAmount, d.FundDisburseAmount));

                    //str.Add(Convert.ToDateTime(d.FundRequisitionDate, cultures).ToString("d"));

                    //str.Add(d.FundSenderName);
                    // str.Add(d.RequisitionType);
                    str.Add(
                        getConcateRequisitionDetails(
                            Convert.ToDateTime(d.FundRequisitionDate, cultures).ToString("d"),
                            Convert.ToDateTime(d.ProposedDisburseDate, cultures).ToString("d"),
                            Convert.ToDateTime(d.FundAvailableDate, cultures).ToString("d"),
                            d.RequisitionType



                        ) +
                        this.getConcateAmount(d.AllowableBudget, d.FundRequisitionAmount, d.FundDisburseAmount)
                        +
                        $"<b style=\"color: LightSeaGreen;\">Sender Name:</b>" +
                        $"{d.FundSenderName} "
                    );

                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = "<a href='/FundDisburse/FundReceivedByRequestor?disburseID=" + _protector.Protect(d.FundDisburseId.ToString()) + "' class='btn btn-outline-primary text-decoration-none'>";
                    actionButtons += "<i class='fas fa-receipt' data-toggle='tooltip' data-placement='left' title='Show'>  &nbsp;Receive </i>";
                    actionButtons += "</a>";

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

        [HttpGet]
        public async Task<IActionResult> FundDisburseByFinanceWaitingForAcknowledge()
        {
            try
            {
                var user = await _sessionManager.GetUser();

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

        [HttpPost]
        public async Task<IActionResult> FundDisburseByFinanceWaitingForAcknowledge(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                if (user.Department_Id != 19) return RedirectToHome();

                List<FundDisburseVM> resultSet = await _fundDisburseService.FundDisburseByFinanceWaitingForAcknowledge(0, start, PAGE_SIZE);
                //int count = resultSet.Count > 0 ? resultSet[0].TotalRow : 0;
                int count = 0;
                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet)
                {
                    var str = new List<string>();
                    var id = d.FundRequisitionId;

                    str.Add(sl.ToString());
                    //str.Add(d.EstimateType);
                    str.Add(d.EstimateIdentifier);
                    str.Add(d.EstimationSubject);
                    //str.Add(this.getConcateAmount(d.AllowableBudget, d.FundRequisitionAmount, d.FundDisburseAmount));

                    //str.Add(Convert.ToDateTime(d.FundRequisitionDate, cultures).ToString("d"));

                    //str.Add(d.FundSenderName);
                    // str.Add(d.RequisitionType);
                    str.Add(
                        getConcateRequisitionDetails(
                            Convert.ToDateTime(d.FundRequisitionDate, cultures).ToString("d"),
                            Convert.ToDateTime(d.ProposedDisburseDate, cultures).ToString("d"),
                            Convert.ToDateTime(d.FundAvailableDate, cultures).ToString("d"),
                            d.RequisitionType



                        ) +
                        this.getConcateAmount(d.AllowableBudget, d.FundRequisitionAmount, d.FundDisburseAmount)
                        +
                        $"<b style=\"color: LightSeaGreen;\">Sender Name:</b>" +
                        $"{d.FundSenderName} "
                    );

                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = " <br/> <a  class=' btn btn-outline-primary text-decoration-none' href='/FundDisburse/FundDisburseReadonlyDetails?disburseID=" + _protector.Protect(d.FundDisburseId.ToString()) + "' class='text-decoration-none'>";
                    actionButtons += "<i class='fas fa-eye' data-toggle='tooltip' data-placement='left' title='Show'>Details</i>";
                    actionButtons += "</a>";

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

        #endregion Fund Disburse List for Requsition Department For Acknowledgement 

        #region Fund Disburse List PendingRollBackListForReSubmitByFinance
        [HttpGet]
        public async Task<IActionResult> PendingRollBackListForReSubmitByFinance()
        {
            try
            {
                var user = await _sessionManager.GetUser();

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

        [HttpPost]
        public async Task<IActionResult> PendingRollBackListForReSubmitByFinance(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();


                List<FundDisburseVM> resultSet = await _fundDisburseService.PendingRollBackListForReSubmitByFinance(0, start, PAGE_SIZE);
                //int count = resultSet.Count > 0 ? resultSet[0].TotalRow : 0;
                int count = 0;
                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet)
                {
                    var str = new List<string>();
                    var id = d.FundRequisitionId;

                    str.Add(sl.ToString());
                    //str.Add(d.EstimateType);
                    str.Add(d.EstimateIdentifier);
                    str.Add(d.EstimationSubject);
                    str.Add(this.getConcateAmount(d.AllowableBudget, d.FundRequisitionAmount, d.FundDisburseAmount));

                    str.Add(Convert.ToDateTime(d.FundRequisitionDate, cultures).ToString("d"));

                    str.Add(d.FundSenderName);
                    str.Add(d.RequisitionType);

                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = "<a href='/FundDisburse/FundReDisburseByFinance?disburseID=" + _protector.Protect(d.FundDisburseId.ToString()) + "' class='text-decoration-none'>";
                    actionButtons += "<i class='fas fa-money-bill' data-toggle='tooltip' data-placement='left' title='Show'> Re-Disburse Money </i>";
                    actionButtons += "</a>";

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

        #endregion Fund Disburse List PendingRollBackListForReSubmitByFinance 


        #region Fund Disburse List CompletedDisburseListDepartmentWise
        [HttpGet]
        public async Task<IActionResult> CompletedDisburseListDepartmentWise()
        {
            try
            {
                var user = await _sessionManager.GetUser();

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

        [HttpPost]
        public async Task<IActionResult> CompletedDisburseListDepartmentWise(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();


                List<FundDisburseVM> resultSet = await _fundDisburseService.CompletedDisburseListDepartmentWise(0, start, PAGE_SIZE);
                //int count = resultSet.Count > 0 ? resultSet[0].TotalRow : 0;
                int count = 0;
                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet)
                {
                    var str = new List<string>();
                    var id = d.FundRequisitionId;

                    str.Add(sl.ToString());
                    //str.Add(getConcateEstimateAndFundRequistionDetails(d.EstimateIdentifier , ));
                    str.Add(d.EstimateIdentifier);
                    str.Add(d.EstimationSubject);
                    str.Add(
                        getConcateRequisitionDetails(
                                    Convert.ToDateTime(d.FundRequisitionDate, cultures).ToString("d"),
                                    Convert.ToDateTime(d.ProposedDisburseDate, cultures).ToString("d"),
                                    Convert.ToDateTime(d.FundAvailableDate, cultures).ToString("d"),
                                    d.RequisitionType



                                ) +
                        this.getConcateAmount(d.AllowableBudget, d.FundRequisitionAmount, d.FundDisburseAmount)
                        +$"<b style=\"color: LightSeaGreen;\"> Fund Received Amount :</b> " + d.ReceivedAmountByRequestor.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))
                        +
                        $"<br><b style=\"color: LightSeaGreen;\">Sender Name:</b>" +
                        $"{d.FundSenderName} <br/><b style=\"color: DarkCyan;\">Receiver Name: </b>{d.FundReceiverName}"
                        );

                    //str.Add(getConcateRequisitionDetails(
                    //        Convert.ToDateTime(d.FundRequisitionDate, cultures).ToString("d"),
                    //        Convert.ToDateTime(d.ProposedDisburseDate, cultures).ToString("d"),
                    //        Convert.ToDateTime(d.FundAvailableDate, cultures).ToString("d"),
                    //        d.RequisitionType

                        
                        
                    //    )
                     
                    //);

                    //str.Add(  $"<b style=\"color: LightSeaGreen;\">Sender Name:</b>" +
                    //          $"{d.FundSenderName} <br/><b style=\"color: DarkCyan;\">Receiver Name: </b>{d.FundReceiverName}");
                    

                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = " <br/> <a  class=' btn btn-outline-primary text-decoration-none' href='/FundDisburse/FundDisburseReadonlyDetails?disburseID=" + _protector.Protect(d.FundDisburseId.ToString()) + "' class='text-decoration-none'>";
                    actionButtons += "<i class='fas fa-eye' data-toggle='tooltip' data-placement='left' title='Show'>Details</i>";
                    actionButtons += "</a>";

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

        #endregion Fund Disburse List CompletedDisburseListDepartmentWise



       

        private string getConcateRequisitionDetails(string requestDate, string proposedDisburseDate,
            string fundAvailableDate, string requisitionType)
        {
            return
                $"<b style=\"color: LightSeaGreen;\">Request Date :</b>{requestDate} <br/> <b style=\"color: LightSeaGreen;\">Proposed Disburse Date:</b> {proposedDisburseDate} " +
                $"<br/><b style=\"color: LightSeaGreen;\"> Fund Avaialabe Date:</b> {fundAvailableDate} <br/> " +
                $"<b style=\"color: LightSeaGreen;\"> Requisiton Type:</b> {requisitionType} <br/>";
        }


        private string getConcateAmount(double allowableBudget, double FundRequisitionAmount, double fundDisburseAmount)
        {
            return
                $"<b style=\"color: LightSeaGreen;\">Allowable Budget:</b>{allowableBudget.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))} <br/>" +
                $" <b style=\"color: LightSeaGreen;\">Requisition Amount:</b> {FundRequisitionAmount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Request Disburse Amount:</b> {fundDisburseAmount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))} <br/>";


        }
        private string getConcateEstimateAndFundRequistionDetails(
            string Identifier,
            string Subject
        )
        {
            return
                $"<b style=\"color: LightSeaGreen;\">Identifier: </b>{Identifier} <br/>" +
                $"<b style=\"color: LightSeaGreen;\">Subject: </b>{Subject} <br/>";

        }



    }
}

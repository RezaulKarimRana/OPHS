using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.Dashboard;
using AMS.Models.ServiceModels.FundRequisition;
using AMS.Models.ViewModel;
using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
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
using System.Linq;
using System.Threading.Tasks;
using AMS.Models.ServiceModels.Settlement;

namespace AMS.Web.Controllers
{
   

    [Authorize]
    [Route("[controller]/[action]")]
    public class FundRequisitionController : BaseController
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

        #endregion


        #region Constructor
        public FundRequisitionController(ILoggerFactory loggerFactory, IBudgetService budgetService, ISessionManager sessionManager,
             IItemService itemService, IDepartmentService departmentService, IThanaService thanaService, IUserService userService,
             IParticularService particularService, IItemCategoryService itemCategoryService, IDistService distService,
             IBudgetApproverService budgetApproverService, IDataProtectionProvider provider, IConfiguration configuration, IDashboardService dashboardService,
             IFundRequisitionService fundRequisitionService
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
        }

        #endregion


        #region All Approved estimate budget list
        [HttpGet]
        public async Task<IActionResult> ApprovedBudgetList()
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

        [HttpPost]
        public async Task<IActionResult> ApprovedBudgetList(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                List<EstimateEditVM> resultSet = await _budgetService.LoadAllApprovedEstimateByUserDepartment(0, start, PAGE_SIZE);
                int count = resultSet.Count > 0 ? resultSet[0].TotalRow : 0;
                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet)
                {
                    var str = new List<string>();
                    var id = d.Id;

                    str.Add(sl.ToString());
                    //str.Add(d.EstimateType);
                    str.Add(d.EstimationIdentity);
                    str.Add(d.Subject);
                    
                    str.Add( "<b> Start Date : </b> " + Convert.ToDateTime(d.PlanStartDate, cultures).ToString("d")
                    + " <br/><b> End Date: </b> " + Convert.ToDateTime(d.PlanEndDate, cultures).ToString("d")
                    );
                  
                    str.Add(
                        getConcateAmount(
                                d.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                                d.TotalAllowableBudget.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                                d.TotalRequisitionAmount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                                d.TotalReceived.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                                d.RemainingBudget.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))
                                )
                        );

                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = "";

                    if (user.Department_Id == 5 ||
                        user.Department_Id == 6 ||
                        user.Department_Id == 7
                       )
                    {
                        if (d.isItAllowableForSettlement > 0)
                        {


                            if (d.IsFinalSettle == 1 && d.draftExists == 0  )
                            {

                                actionButtons += "<a href='/Settlement/SettlementSummaryViewByEstimateId?id=" +
                                                 _protector.Protect(id.ToString()) +
                                                 "' class=' btn btn-outline-primary text-decoration-none'>";
                                actionButtons +=
                                    "<i class='fa fa-xs fa-history' data-toggle='tooltip' data-placement='left' title='Settlement Summary '>  &nbsp; Summary </i></a>";
                            }
                            else
                            {
                                actionButtons += "<a href='/FundRequisition/NewFundRequisition?id=" +
                                                 _protector.Protect(id.ToString()) +
                                                 "' class=' btn btn-outline-primary text-decoration-none'>";

                                actionButtons +=
                                    "<i class='fas fa-xs fa-money-bill' data-toggle='tooltip' data-placement='left' title='new Fund '>  &nbsp;New Fund </i>";
                                actionButtons += "</a><br/><br/>";
                                if (user.Department_Id == 5)
                                {
                                    actionButtons += " <a href='/Settlement/NewSettlementView?id=" +
                                                    _protector.Protect(id.ToString()) +
                                                    "' class=' btn btn-outline-info'>";

                                    actionButtons +=
                                    "<i class='fas fa-xs fa-file-invoice-dollar' data-toggle='tooltip' data-placement='left' title='new Settlement'>  &nbsp;New Settlement";
                                    //if (d.draftExists > 0) actionButtons += "<i >draft</i> ";
                                    if (d.draftExists > 0)
                                        actionButtons += " <i class='badge badge-warning'> Draft </  > ";
                                    actionButtons += " </i></a>";
                                }
                            }


                        }

                    }
                    

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
        #endregion End All Approved Budget List

        #region Submitted Fund Requistion by Department wise
        [HttpGet]
        public async Task<IActionResult> SubmitedFundRequistionList()
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

        [HttpPost]
        public async Task<IActionResult> SubmitedFundRequistionList(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                List<FundRequisitionVM> resultSet = await _fundRequisitionService.SubmitedFundRequistionList(0, start, PAGE_SIZE);
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
                    str.Add(getConcateEstimateAndFundRequistionDetails(d.EstimateIdentifier , d.Subject ));
                    double dueAmount = d.Amount - d.TotalReceived;
                    str.Add(
                        getConcateRequisitionDetails(
                            Convert.ToDateTime(d.ProposedDisburseDate, cultures).ToString("d"),
                            d.RequisitionType.ToString(),
                            d.Amount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                            d.TotalReceived,
                            dueAmount,
                            d.CreateorFullName,
                            d.RequisitionStatus.ToString()
                            )
                        );
                                
                  

                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = "<a href='/FundRequisition/ViewFundRequistion?id=" + _protector.Protect(id.ToString()) + "' class=' btn btn-outline-primary text-decoration-none'>";
                    actionButtons += "<i class='fas fa-eye' data-toggle='tooltip' data-placement='left' title='Show'>  &nbsp;View </i>";
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
        public async Task<IActionResult> RejectedFundRequistionList()
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

        [HttpPost]
        public async Task<IActionResult> RejectedFundRequistionList(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                List<FundRequisitionVM> resultSet = await _fundRequisitionService.FundRequistionListByStatus(0, start, PAGE_SIZE, -500);
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
                    str.Add(getConcateEstimateAndFundRequistionDetails(d.EstimateIdentifier, d.Subject));
                    double dueAmount = d.Amount - d.TotalReceived;
                    str.Add(
                        getConcateRequisitionDetails(
                            Convert.ToDateTime(d.ProposedDisburseDate, cultures).ToString("d"),
                            d.RequisitionType.ToString(),
                            d.Amount.ToString(),
                            d.TotalReceived,
                            dueAmount,
                            d.CreateorFullName,
                            d.RequisitionStatus.ToString()
                            )
                        );



                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = "<a href='/FundRequisition/ViewFundRequistion?id=" + _protector.Protect(id.ToString()) + "' class=' btn btn-outline-primary text-decoration-none'>";
                    actionButtons += "<i class='fas fa-money-bill' data-toggle='tooltip' data-placement='left' title='Show'>  &nbsp;View </i>";
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

        #endregion
        [HttpGet]
        public async Task<IActionResult> CompletedFundRequistionList()
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

        [HttpPost]
        public async Task<IActionResult> CompletedFundRequistionList(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                List<FundRequisitionVM> resultSet = await _fundRequisitionService.FundRequistionListByStatus(0, start, PAGE_SIZE, 100);
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
                    str.Add(getConcateEstimateAndFundRequistionDetails(d.EstimateIdentifier, d.Subject));
                    double dueAmount = d.Amount - d.TotalReceived;
                    str.Add(
                        getConcateRequisitionDetails(
                            Convert.ToDateTime(d.ProposedDisburseDate, cultures).ToString("d"),
                            d.RequisitionType.ToString(),
                            d.Amount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                            d.TotalReceived,
                            dueAmount,
                            d.CreateorFullName,
                            d.RequisitionStatus.ToString()
                            )
                        );



                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = "<a href='/FundRequisition/ViewFundRequistion?id=" + _protector.Protect(id.ToString()) + "' class='btn btn-outline-primary text-decoration-none'>";
                    actionButtons += "<i class='fas fa-eye' data-toggle='tooltip' data-placement='left' title='Show'>  &nbsp;View </i>";
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

        [HttpPost]
        public async Task<IActionResult> RejectFundRequisition(int fundRequistionId, string remarks)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();


                _fundRequisitionService.UpdateFundRequistionStatus(fundRequistionId, remarks);


                return Json(1);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(e);
            }
        }


        #region Fund Requisition List for Finance
        [HttpGet]
        public async Task<IActionResult> FundRequistionListForFinance()
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user.Department_Id != 19) return RedirectToHome();
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
        public async Task<IActionResult> FundRequistionListForFinance(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                if(user.Department_Id != 19) return RedirectToHome();

                List<FundRequisitionVM> resultSet = await _fundRequisitionService.FundRequistionListForFinance(0, start, PAGE_SIZE);
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
                    str.Add(getConcateEstimateAndFundRequistionDetails(d.EstimateIdentifier , d.Subject));
                    
                    //double dueAmount = d.Amount - d.TotalReceived;
                    double dueAmount = d.Amount - d.AlreadyDisburseAmount;
                    str.Add(
                        getConcateRequisitionDetailsForPending(
                            Convert.ToDateTime(d.ProposedDisburseDate, cultures).ToString("d"),
                            d.RequisitionType.ToString(),
                            d.Amount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                            d.TotalReceived,
                            dueAmount,
                            d.AlreadyDisburseAmount,
                            d.CreateorFullName,
                            d.RequisitionStatus.ToString()
                        )
                        //+
                        //"<b style=\"color: LightSeaGreen;\">Already Disburse Amount : </b> " + d.AlreadyDisburseAmount
                        +
                        "<b style=\"color: LightSeaGreen;\">Requisition Department : </b> " + d.RequistionDepartmentName
                    );



          
                 
                        // str.Add(d.RequistionDepartmentName);


                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = "";
                    actionButtons += "<a href='/FundDisburse/NewFundDisburse?id=" + _protector.Protect(d.FundRequisitionId.ToString()) + "' class=' btn btn-outline-primary text-decoration-none'>";
                    actionButtons += "<i class='fas fa-xs fa-money-check' data-toggle='tooltip' data-placement='left' title='New Fund Disburse '>  &nbsp; New Disburse </i></a>";
                    actionButtons += "<hr><a href='/FundRequisition/ViewFundRequistion?id=" + _protector.Protect(d.FundRequisitionId.ToString()) + "' class='  btn btn-outline-primary  text-decoration-none'>";
                    actionButtons += "<i class='fas fa-xs fa-eye' data-toggle='tooltip' data-placement='left' title='Show'>  &nbsp;View </i>";
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
        public async Task<IActionResult> RejectedFundRequistionListForFinance()
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user.Department_Id != 19) return RedirectToHome();
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
        public async Task<IActionResult> RejectedFundRequistionListForFinance(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                if (user.Department_Id != 19) return RedirectToHome();

                List<FundRequisitionVM> resultSet = await _fundRequisitionService.RejectedFundRequistionListForFinance(0, start, PAGE_SIZE);
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
                    str.Add(getConcateEstimateAndFundRequistionDetails(d.EstimateIdentifier, d.Subject));

                    //double dueAmount = d.Amount - d.TotalReceived;
                    double dueAmount = d.Amount - d.AlreadyDisburseAmount;
                    str.Add(
                        getConcateRequisitionDetailsForPending(
                            Convert.ToDateTime(d.ProposedDisburseDate, cultures).ToString("d"),
                            d.RequisitionType.ToString(),
                            d.Amount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")),
                            d.TotalReceived,
                            dueAmount,
                            d.AlreadyDisburseAmount,
                            d.CreateorFullName,
                            d.RequisitionStatus.ToString()
                        )
                        //+
                        //"<b style=\"color: LightSeaGreen;\">Already Disburse Amount : </b> " + d.AlreadyDisburseAmount
                        +
                        "<b style=\"color: LightSeaGreen;\">Requisition Department : </b> " + d.RequistionDepartmentName
                    );





                    // str.Add(d.RequistionDepartmentName);


                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = "";
                    //actionButtons += "<a href='/FundDisburse/NewFundDisburse?id=" + _protector.Protect(d.FundRequisitionId.ToString()) + "' class=' btn btn-outline-primary text-decoration-none'>";
                    //actionButtons += "<i class='fas fa-xs fa-money-check' data-toggle='tooltip' data-placement='left' title='New Fund Disburse '>  &nbsp; New Disburse </i></a>";
                    actionButtons += "<hr><a href='/FundRequisition/ViewFundRequistion?id=" + _protector.Protect(d.FundRequisitionId.ToString()) + "' class='  btn btn-outline-primary  text-decoration-none'>";
                    actionButtons += "<i class='fas fa-xs fa-eye' data-toggle='tooltip' data-placement='left' title='Show'>  &nbsp;View </i>";
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
        #endregion Fund Requisition List for Finance

        #region New Fund Requisition
        [HttpGet]
        public async Task<IActionResult> NewFundRequisition(string id)
        {
            try
            {
                var estimationId = Convert.ToInt32(_protector.Unprotect(id));
                ViewBag.fundRequisition = await _fundRequisitionService.GetFundRequisitionHistoryByEstimationId(estimationId);

                    
                var response = await _budgetService.GetOneEstimationWithType(estimationId);
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


        [HttpGet]
        public async Task<IActionResult> ViewFundRequistion(string id)
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

        [HttpPost]
        public async Task<JsonResult> PostNewFundRequistion(/*AddFundRequisition*/string requestDto)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<FundRequisition>(requestDto);
                var result = await _fundRequisitionService.addFundRequisition(jsonData);
                return Json(result);
            }
            catch (Exception e)
            {
                //capture log
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetFundRequistionDisburseHistory(int estimationId)
        {
            var response = await _fundRequisitionService.getFundRequisitionDisburseHistory(estimationId);
            return new JsonResult(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetFundRequistionDisburseHistoryOfOtherDepartment(int estimationId)
        {
            var response = await _fundRequisitionService.getFundRequisitionDisburseHistoryOfOtherDepartment(estimationId);
            return new JsonResult(response);
        }
        #endregion New Fund Requisition

        [HttpGet]
        public async Task<IActionResult> GetInCompletedFundRequisitionForCheckFinalSettlement(int estimationId)
        {
            var response = await _fundRequisitionService.getInCompletedFundRequistionForCheckFinalSettlement(estimationId);
            return new JsonResult(response);
        }


        private string getConcateAmount(
            string totalPrice,
            string allowableBudget,
            string FundRequisitionAmount,
            string fundReceiveAmount,
            string remainingBudget
        )
        {
            return
                $"<b style=\"color: LightSeaGreen;\">Total Price:</b>{totalPrice} <br/>" +
                $"<b style=\"color: LightSeaGreen;\">Allowable Budget:</b>{allowableBudget} <br/>" +
                $" <b style=\"color: LightSeaGreen;\">Requisition Amount:</b> {FundRequisitionAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Received Amount:</b> {fundReceiveAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Remaining Budget:</b> {remainingBudget} <br/>";


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


        private string getConcateRequisitionDetails( string proposedDisburseDate,
             string requisitionType , string requisitionAmount, int receivedAmout , double dueAmount, string RequestorName , string requisitionStatus)
        {
            string receivedAmount = receivedAmout.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"));
            string sDueAmount = dueAmount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"));
            
            return
                $"<b style=\"color: LightSeaGreen;\">Proposed Disburse Date:</b> {proposedDisburseDate} <br/> " +
                
                $"<b style=\"color: LightSeaGreen;\"> Requisiton Type:</b> {requisitionType} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Requisiton Amount:</b> {requisitionAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Received Amount:</b> {receivedAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Due Amount:</b> {sDueAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Requestor Name:</b> {RequestorName} <br/>" + 
                $"<b style=\"color: LightSeaGreen;\"> Requisition Status:</b> {requisitionStatus} <br/>";
        }
        private string getConcateRequisitionDetailsForPending(string proposedDisburseDate,
            string requisitionType, string requisitionAmount, int receivedAmout, double dueAmount,double alreadyDisburseAmount, string RequestorName, string requisitionStatus)
        {
            string receivedAmount = receivedAmout.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"));
            string sDueAmount = dueAmount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"));
            string sAlreadyDisburseAmount = alreadyDisburseAmount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"));

            return
                $"<b style=\"color: LightSeaGreen;\">Proposed Disburse Date:</b> {proposedDisburseDate} <br/> " +

                $"<b style=\"color: LightSeaGreen;\"> Requisiton Type:</b> {requisitionType} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Requisiton Amount:</b> {requisitionAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Received Amount:</b> {receivedAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Due Amount:</b> {sDueAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Already Disburse Amount:</b> {sAlreadyDisburseAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Requestor Name:</b> {RequestorName} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Requisition Status:</b> {requisitionStatus} <br/>";
        }


    }
}

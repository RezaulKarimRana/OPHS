using AMS.Common.Helpers;
using AMS.Infrastructure.Authorization;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.Dashboard;
using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
using AMS.Services.Managers.Contracts;
using AMS.Services.Memo.Contracts;
using AMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class StatusBoardController : BaseController
    {
        #region Instance  Variable
        private readonly ILogger<StatusBoardController> _logger;
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
        private readonly IDashboardService _dashboardService;
        private readonly IConfiguration _configuration;
        public IDataProtector _protector;
        private readonly IMemoService _memoService;

        #endregion

        #region Constructor
        public StatusBoardController(ILoggerFactory loggerFactory, IBudgetService budgetService, ISessionManager sessionManager,
             IItemService itemService, IDepartmentService departmentService, IThanaService thanaService, IUserService userService,
             IParticularService particularService, IItemCategoryService itemCategoryService, IDistService distService,
             IBudgetApproverService budgetApproverService, IDashboardService dashboardService,
             IDataProtectionProvider provider, IConfiguration configuration, IMemoService memoService)
        {
            _logger = loggerFactory.CreateLogger<StatusBoardController>();
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
            _dashboardService = dashboardService;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
            _memoService = memoService;
        }

        #endregion


        [HttpGet]
        public async Task<IActionResult> RunningBoard(int? page)
        {
            var user = await _sessionManager.GetUser();
            if (user == null) return RedirectToHome();
            int currentPageIndex = page.HasValue ? page.Value : 0;
            //TODO: Need to pick only 10 from DB. Will update this later.
            List<EstimateEditVM> result = await _budgetService.LoadAllPendingEstimate(user.Id, currentPageIndex, PAGE_SIZE);

            List<EstimateEditVM> editList = new();

            foreach (var item in result)
            {
                EstimationEntity estimate = await _budgetService.GetEstimateById(item.Id);
                if (estimate == null) continue;
                //List<EstimateDetailsEntity> estimateDetails = await _budgetService.LoadEstimateDetailByEstimationId(item.Id);
                //List<ParticularWiseSummaryEntity> particularWiseSummary = await _budgetService.LoadParticularWiseSummaryByEstimationId(item.Id);
                //List<DepartmentWiseSummaryEntity> departmentWiseSummary = await _budgetService.LoadDepartmentWiseSummaryByEstimationId(item.Id);
                List<EstimateApproverEntity> estimateApprovers = await _budgetService.LoadEstimateApproverByEstimationId(item.Id);
                EstimateEditVM editVm = await _budgetService.CastEstimateToVM(estimate, estimateApprovers, null, null, null);
                editVm.IsInFinalApproval = item.IsInFinalApproval;
                editList.Add(editVm);
            }
            int count = result.Count > 0 ? result[0].TotalRow : 0;
            var paging = new Pagination(count, currentPageIndex, PAGE_SIZE);
            ViewBag.paged = paging;
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
            var responsedData = editList.OrderBy(x => x.Id).ToList();
            return View(responsedData);
        }

        #region Unused Code

        //[HttpGet]
        //public async Task<IActionResult> CompleteBoard(int? page)
        //{
        //    var user = await _sessionManager.GetUser();
        //    if (user == null) return RedirectToHome();
        //    int currentPageIndex = page.HasValue ? page.Value : 0;
        //    //TODO: Need to pick only 10 from DB. Will update this later.
        //    List<EstimateEditVM> result = await _budgetService.LoadAllCompleteEstimate(0, currentPageIndex, PAGE_SIZE,"");

        //    List<EstimateEditVM> editList = new List<EstimateEditVM>();

        //    foreach (var item in result)
        //    {
        //        EstimationEntity estimate = await _budgetService.GetEstimateById(item.Id);
        //        if (estimate == null) continue;
        //        //List<EstimateDetailsEntity> estimateDetails = await _budgetService.LoadEstimateDetailByEstimationId(item.Id);
        //        //List<ParticularWiseSummaryEntity> particularWiseSummary = await _budgetService.LoadParticularWiseSummaryByEstimationId(item.Id);
        //        //List<DepartmentWiseSummaryEntity> departmentWiseSummary = await _budgetService.LoadDepartmentWiseSummaryByEstimationId(item.Id);
        //        List<EstimateApproverEntity> estimateApprovers = await _budgetService.LoadEstimateApproverByEstimationId(item.Id);
        //        EstimateEditVM editVm = await _budgetService.CastEstimateToVM(estimate, estimateApprovers, null, null, null);
        //        editList.Add(editVm);
        //    }
        //    int count = result.Count > 0 ? result[0].TotalRow : 0;
        //    var paging = new Pagination(count, currentPageIndex, PAGE_SIZE);
        //    ViewBag.paged = paging;
        //    var responseForNav = await _dashboardService.GetNavBarCount();
        //    if (responseForNav.IsSuccessful)
        //    {
        //        ViewData["CountPendingObject"] = new GetCountForAllPendingParkingForNavService
        //        {
        //            TotalDraftParking = responseForNav.TotalDraftParking,
        //            TotalCompletedParking = responseForNav.TotalCompletedParking,
        //            TotalPendingApprovalParking = responseForNav.TotalPendingApprovalParking,
        //            TotalRollbackParking = responseForNav.TotalRollbackParking
        //        };
        //    }

        //    var responsedData = editList.OrderBy(x => x.Id).ToList();
        //    return View(responsedData);
        //}

        #endregion

        [HttpGet]
        public async Task<IActionResult> RejectedBoard(int? page)
        {
            var user = await _sessionManager.GetUser();
            if (user == null) return RedirectToHome();
            int currentPageIndex = page.HasValue ? page.Value : 0;
            //TODO: Need to pick only 10 from DB. Will update this later.
            List<EstimateEditVM> result = await _budgetService.LoadRejectedEstimate(0, currentPageIndex, PAGE_SIZE);

            List<EstimateEditVM> editList = new List<EstimateEditVM>();

            foreach (var item in result)
            {
                EstimationEntity estimate = await _budgetService.GetEstimateById(item.Id);
                if (estimate == null) continue;
                //List<EstimateDetailsEntity> estimateDetails = await _budgetService.LoadEstimateDetailByEstimationId(item.Id);
                //List<ParticularWiseSummaryEntity> particularWiseSummary = await _budgetService.LoadParticularWiseSummaryByEstimationId(item.Id);
                //List<DepartmentWiseSummaryEntity> departmentWiseSummary = await _budgetService.LoadDepartmentWiseSummaryByEstimationId(item.Id);
                List<EstimateApproverEntity> estimateApprovers = await _budgetService.LoadEstimateApproverByEstimationId(item.Id);
                EstimateEditVM editVm = await _budgetService.CastEstimateToVM(estimate, estimateApprovers, null, null, null);
                editList.Add(editVm);
            }
            int count = result.Count > 0 ? result[0].TotalRow : 0;
            var paging = new Pagination(count, currentPageIndex, PAGE_SIZE);
            ViewBag.paged = paging;
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

            var responsedData = editList.OrderBy(x => x.Id).ToList();
            return View(responsedData);
        }

        [HttpGet]
        public async Task<IActionResult> CompletedEstimateBudget()
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
                throw e;
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadCompletedEstimationParkeingData(int draw = 0, int start = 0, int length = 0
            , string searchIdentity = "", string searchSubject = "")
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var whereClause = "";
                if (!String.IsNullOrEmpty(searchIdentity))
                {
                    whereClause += " and e.[UniqueIdentifier] like '%" + searchIdentity + "%'";
                }

                if (!String.IsNullOrEmpty(searchSubject))
                {
                    whereClause += " and e.[Subject] like '%" + searchSubject + "%'";
                }

                var result = await _budgetService.LoadAllCompleteEstimate(user.Id,
                    start, length,whereClause);

                var responseData = new List<EstimateEditVM>();

                foreach (var item in result)
                {
                    var estimate = await _budgetService.GetEstimateById(item.Id);
                    if (estimate == null) continue;

                    var estimateApprovers = await _budgetService.LoadEstimateApproverByEstimationId(item.Id);
                    var editVm = await _budgetService.CastEstimateToVM(estimate, estimateApprovers, null, null, null);
                    editVm.TotalRow = item.TotalRow;
                    responseData.Add(editVm);
                }

                var resultSet = responseData.OrderBy(x => x.Id).ToList();

                int count = 0;
                if (String.IsNullOrEmpty(whereClause))
                    count = resultSet.Count > 0 ? resultSet[0].TotalRow : 0;
                else
                {
                    var forCountData =  await _budgetService.LoadAllCompleteEstimate(user.Id,
                    start, length, whereClause, false);

                    count = forCountData.Count();
                }

                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();

                foreach (var d in resultSet)
                {
                    var str = new List<string>();
                    var id = d.Id;
                    var lastItem = d.EstimateApproverList.Last();

                    #region Budget Summary
                    str.Add(@"<div class='card'>
                                <div class='card-header'>
                                    <b>Identity : <mark> " + d.EstimationIdentity + @" </mark> </b>
                                </div>
                                <div class='card-header'>
                                    <b>Subject :</b> " + d.Subject + @"
                                </div>
                                <ul class='list-group list-group-flush'>
                                    <li class='list-group-item'><b>Start Date : </b>" + d.PlanStartDate.ToString("d") + @"</li>
                                    <li class='list-group-item'><b>End Date : </b>" + d.PlanEndDate.ToString("d") + @"</li>
                                    <li class='list-group-item'><b>Total Budget Price : </b>" + d.TotalPrice.ToString()+"" + @Utility.CurrencyTypeConvertToStringFormat(d.CurrencyType) + @"</li>
                                </ul>
                            </div>");
                    #endregion
                    #region Creator Summary
                    str.Add(@"<ul class='list-group list-group-flush'>
                                <li class='list-group-item'><b>Creator Name: </b>" + d.CreatedBy.First_Name + " " + d.CreatedBy.Last_Name + @"</li>
                                <li class='list-group-item'><b>Creator Username: </b>" + d.CreatedBy.Username + @"</li>
                                <li class='list-group-item'><b>Creation Date : </b>" + d.CreatedTime.ToString("d") + @"</li>
                            </ul>"); 
                    #endregion

                    str.Add("<button type='button' class='btn btn-outline-success'>Completed</button> ");

                    var approverTag = "";

                    foreach(var _item in d.EstimateApproverList.OrderByDescending(x => x.Priority).ToList())
                    {
                        approverTag += @"<div style='padding: 3px;display: inline-block'>
                                        <button type= 'button' class='btn btn-success' data-toggle='tooltip' 
                                        title='" + _item.UserFullName+ @" Response: " + _item.ResponseTime + @"'>
                                            " + _item.UserName + @"
                                        </button>
                                    </div>";

                        if (!_item.Equals(lastItem))
                            approverTag += @"<i class='fa fa-arrow-circle-right'></i>";

                    }

                    str.Add(approverTag);

                    string actionButtons = "<a href='/BudgetEstimation/ViewEstimation?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                    actionButtons += "<i class='fas fa-eye' data-toggle='tooltip' data-placement='left' title='Show'></i>";
                    actionButtons += "</a>";

                    str.Add(actionButtons);
                    data.Add(str);
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


        #region Memo Status Board
        [HttpGet]
        public async Task<IActionResult> CompletedMemoBudgetStatusBoard()
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
                throw e;
            }
        }

        [HttpGet]
        public async Task<IActionResult> RunningMemoBudgetStatusBoard()
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
                throw e;
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadRunningMemoParkingData(int draw = 0, int start = 0, int length = 0
            ,string searchIdentity = "", string searchSubject = "")
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var whereClause = "";
                if(!String.IsNullOrEmpty(searchIdentity))
                {
                    whereClause += " and e.[UniqueIdentifier] like '%" + searchIdentity + "%'";
                }

                if (!String.IsNullOrEmpty(searchSubject))
                {
                    whereClause += " and e.[Subject] like '%" + searchSubject + "%'";
                }

                var resultSet = await _memoService.LoadAllRunningMemoApprovalForAUserService(user.Id
                    , start, PAGE_SIZE,whereClause);

                foreach (var _item in resultSet)
                {
                    _item.InitiatorDetail = await _userService.GetById(_item.InitiatorId);
                    _item.ApproverDetailsDTO = await _memoService.LoadApproverDetailsByMemoService(_item.Id);
                }
                int count = resultSet.Count > 0 ? resultSet.Count : 0;
                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();

                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet)
                {
                    var str = new List<string>();
                    var id = d.Id;
                    int NextApprovalPriority = d.ApproverDetailsDTO.Where(x => x.ApproverRoleId !=3 
                        && x.ApproverStatus == BaseEntity.EntityStatus.Pending.ToString()).Max(x => x.ApproverPriority);

                    var lastApprover = d.ApproverDetailsDTO.Last();

                    #region Memo Summary
                    str.Add(@"<div class='card'>
                                <div class='card-header'>
                                    <b>Identity : <mark> " + d.EstimationIdentity + @" </mark> </b>
                                </div>
                                <div class='card-header'>
                                    <b>Subject :</b> " + d.Subject + @"
                                </div>
                                <ul class='list-group list-group-flush'>
                                    <li class='list-group-item'><b>Total Budget : </b>" + d.BudgetPrice.ToString("c", CultureInfo.CreateSpecificCulture("bn-BD")) + @"</li>
                                    <li class='list-group-item'><b>Total Allowable : </b>" + d.AllowableBudget.ToString("c", CultureInfo.CreateSpecificCulture("bn-BD")) + @"</li>
                                    <li class='list-group-item'><b>Total Cost : </b>" + d.FinalSettledAmount.ToString("c", CultureInfo.CreateSpecificCulture("bn-BD")) + @"</li>
                                    <li class='list-group-item'><b>Deviation : </b>" + d.TotalDeviation.ToString("c", CultureInfo.CreateSpecificCulture("bn-BD")) + @"</li>
                                </ul>
                            </div>");
                    #endregion
                    #region Inititor Summary
                    str.Add(@"<ul class='list-group list-group-flush'>
                                <li class='list-group-item'><b>Initiator Name: </b>" + d.InitiatorDetail.First_Name + " " + d.InitiatorDetail.Last_Name + @"</li>
                                <li class='list-group-item'><b>Initiator Username: </b>" + d.InitiatorDetail.Username + @"</li>
                                <li class='list-group-item'><b>Initiating Date : </b>" + d.CreatedDateTime.ToString("d") + @"</li>
                            </ul>");
                    #endregion

                    str.Add("<button type='button' class='btn btn-outline-primary'>Running</button> ");

                    var approverTag = "<hr> <b><span>Approver : </span> </b>";
                    var informerTag = "<b><span>Informer : </span> </b>";

                    foreach (var _item in d.ApproverDetailsDTO.OrderByDescending(x => x.ApproverPriority).ToList())
                    {
                        if (_item.ApproverRoleId != 3)
                        {
                            if(_item.ApproverStatus == BaseEntity.EntityStatus.Completed.ToString())
                            {
                                approverTag += @"<div style='padding: 5px;display: inline-block'>
                                        <button type= 'button' class='btn btn-success' data-toggle='tooltip' 
                                        title='" + _item.ApproverFirstName + " " + _item.ApproverLastName + @"; Response: " 
                                        + _item.ApproverFeedbackDate + @"'><i class='fa fa-check' aria-hidden='true'></i>
                                            " + _item.ApproverUserName + @"
                                        </button>
                                    </div>";
                            }
                            else if(_item.ApproverStatus == BaseEntity.EntityStatus.Pending.ToString())
                            {
                                var buttonColor = "";
                                var icon = "";
                                if(_item.ApproverPriority == NextApprovalPriority)
                                {  
                                    buttonColor = "btn-warning";
                                }
                                else
                                {
                                    icon = "<i class='fa fa-arrow-right' aria-hidden='true'></i>";
                                    buttonColor = "btn-info";
                                }
                                approverTag += @"<div style='padding: 5px;display: inline-block'>
                                        <button type= 'button' class='btn " + buttonColor + @"' data-toggle='tooltip' 
                                        title='" + _item.ApproverFirstName + " " + _item.ApproverLastName + @";'>" + icon + @"
                                            " + _item.ApproverUserName + @"
                                        </button>
                                    </div>";
                            }

                            if (!_item.Equals(lastApprover))
                                approverTag += @"<i class='fa fa-arrow-circle-right'></i>";
                        }
                        else if (_item.ApproverRoleId == 3)
                        {
                            informerTag += @"<div style='padding: 5px;display: inline-block'>
                                        <button type= 'button' class='btn btn-secondary' data-toggle='tooltip' 
                                        title='" + _item.ApproverFirstName + " " + _item.ApproverFirstName + @"'>
                                            " + _item.ApproverUserName + @"
                                        </button>
                                    </div>";
                        }
                    }

                    str.Add(informerTag + approverTag);

                    string actionButtons = "<a href='/BudgetMemo/Details?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                    actionButtons += "<i class='fas fa-eye' data-toggle='tooltip' data-placement='left' title='Show'></i>";
                    actionButtons += "</a>";

                    str.Add(actionButtons);
                    data.Add(str);
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
        [AuthorizePermission(PermissionKeys.RunningBudgetForSepecificUser)]
        public async Task<IActionResult> RunningFollowingStatusBoard(int? page)
        {
            var user = await _sessionManager.GetUser();
            if (user == null) return RedirectToHome();
            var follower = await _userService.GetFollowersByUserId(user.Id);
            if (follower == null)
            {
                return RedirectToHome();
            }
            int currentPageIndex = page.HasValue ? page.Value : 0;
            //TODO: Need to pick only 10 from DB. Will update this later.
            List<EstimateEditVM> result = await _budgetService.LoadAllPendingEstimate(follower.FollowerUserId, currentPageIndex, PAGE_SIZE);

            List<EstimateEditVM> editList = new();

            foreach (var item in result)
            {
                EstimationEntity estimate = await _budgetService.GetEstimateById(item.Id);
                if (estimate == null) continue;
                //List<EstimateDetailsEntity> estimateDetails = await _budgetService.LoadEstimateDetailByEstimationId(item.Id);
                //List<ParticularWiseSummaryEntity> particularWiseSummary = await _budgetService.LoadParticularWiseSummaryByEstimationId(item.Id);
                //List<DepartmentWiseSummaryEntity> departmentWiseSummary = await _budgetService.LoadDepartmentWiseSummaryByEstimationId(item.Id);
                List<EstimateApproverEntity> estimateApprovers = await _budgetService.LoadEstimateApproverByEstimationId(item.Id);
                EstimateEditVM editVm = await _budgetService.CastEstimateToVM(estimate, estimateApprovers, null, null, null);
                editVm.IsInFinalApproval = item.IsInFinalApproval;
                editList.Add(editVm);
            }
            int count = result.Count > 0 ? result[0].TotalRow : 0;
            var paging = new Pagination(count, currentPageIndex, PAGE_SIZE);
            ViewBag.paged = paging;
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
            var responsedData = editList.OrderBy(x => x.Id).ToList();
            return View(responsedData);
        }
    }
}

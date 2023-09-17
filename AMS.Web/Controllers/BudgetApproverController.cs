using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.Dashboard;
using AMS.Models.ViewModel;
using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
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
using AMS.Web.Attributes;
using Microsoft.AspNetCore.Identity;
using AMS.Infrastructure.Authorization;
using AMS.Common.Helpers;

namespace AMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class BudgetApproverController : BaseController
    {
        #region Instant Variable Declaration
        private readonly ILogger<BudgetApproverController> _logger;
        private readonly IBudgetService _budgetService;
        private readonly ISessionManager _sessionManager;
        private readonly IPermissionsService _permissionsService;

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
        

        #endregion


        #region Constructor
        public BudgetApproverController(ILoggerFactory loggerFactory, IBudgetService budgetService, ISessionManager sessionManager,
             IItemService itemService, IDepartmentService departmentService, IThanaService thanaService, IUserService userService,
             IParticularService particularService, IItemCategoryService itemCategoryService, IDistService distService,
             IBudgetApproverService budgetApproverService, IDataProtectionProvider provider, IConfiguration configuration, IDashboardService dashboardService
             ,IPermissionsService permissionsService)
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
            _permissionsService = permissionsService;
        }

        #endregion
        [AuthorizePermission(PermissionKeys.RunningBudgetForSepecificUser)]
        public async Task<IActionResult> PendingParkingSpecificUser()
        {
            try
            {
                

                    // var user = await _sessionManager.GetUser();

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
        //AllBudgetParkingForSpecificUserExceptPending
        public async Task<IActionResult> LoadPendingBudgetForSpecificUser(int draw = 0, int start = 0, int length = 0)
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
                List<EstimateVM> result = await _budgetService.LoadAllPendingEstimateByUser(follower.FollowerUserId);
                var data = new List<object>();
                List<EstimateVM> resultSet = new List<EstimateVM>();
                foreach (var item in result)
                {
                    bool isValid = await _budgetService.IsValidToShowInParking(item.EstimationId, item.Priority);
                    if (!isValid) continue;
                    resultSet.Add(item);
                }

                long recordsTotal = resultSet.Count;
                long recordsFiltered = recordsTotal;
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet)
                {
                    var str = new List<string>();
                    var id = d.EstimationId;

                    str.Add(sl.ToString());
                    str.Add(d.Subject);
                    str.Add(d.EstimateIdentifier);
                    str.Add(d.EstimateType);
                    str.Add(Convert.ToDateTime(d.PlanStartDate, cultures).ToString("d"));
                    str.Add(Convert.ToDateTime(d.PlanEndDate, cultures).ToString("d"));
                    str.Add(d.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.CreateorFullName);

                    string actionTable = "";
                    if (user.Id == 16 && user.Username == "arif" && user.Email_Address == "arif@summit-centre.com")
                    {
                        string actionShowButtons = "<button class='btn btn-secondary'><a href='/BudgetApprover/Edit?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                        actionShowButtons += "<i class='fas fa-edit' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
                        actionShowButtons += "</a></button>";

                        string actionApproveButtons = "<button class='btn btn-success' data-toggle='modal' data-target='#remarksModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionApproveButtons += "<i class='fa fa-check' data-toggle='tooltip' data-placement='left' title='Approve'></i>";
                        actionApproveButtons += "</button>";

                        string actionRBButtons = "<button class='btn btn-warning' data-toggle='modal' data-target='#remarksRBModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionRBButtons += "<i class='fa fa-undo' data-toggle='tooltip' data-placement='left' title='Rollback'></i>";
                        actionRBButtons += "</button>";

                        string actionRejectButtons = "<button class='btn btn-danger' data-toggle='modal' data-target='#remarksRejectModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionRejectButtons += "<i class='fa fa-ban' data-toggle='tooltip' data-placement='left' title='Reject'></i>";
                        actionRejectButtons += "</button>";

                        actionTable = @"<table><tbody><tr><td>" + actionShowButtons + "</td><td>" +
                            actionApproveButtons + "</td><td>" + actionRBButtons + "</td><td>" +
                                actionRejectButtons + "</td></tr></tbody></table>";

                        str.Add(actionTable);
                    }
                    else
                    {
                        string actionButtons = "<a href='/BudgetEstimation/ViewEstimation?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                        actionButtons += "<i class='fa fa-eye fa-2x' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
                        actionButtons += "</a>";

                        str.Add(actionButtons);
                    }

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


        [AuthorizePermission(PermissionKeys.AllBudgetForSepecificUser)]
        public async Task<IActionResult> AllBudgetParkingForSpecificUserExceptPending()
        {
            try
            {


                // var user = await _sessionManager.GetUser();

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
        //AllBudgetParkingForSpecificUserExceptPending
        public async Task<IActionResult> LoadAllBudgetParkingForSpecificUserExceptPending(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                var follower = await _userService.GetFollowersByUserId(user.Id);
                if (follower == null)
                {
                    return RedirectToHome();
                }
                List<EstimateVM> result = await _budgetService.LoadAllEstimateByUserExceptPending(follower.FollowerUserId);
                var data = new List<object>();
                //List<EstimateVM> resultSet = new List<EstimateVM>();
                //foreach (var item in result)
                //{
                //    bool isValid = await _budgetService.IsValidToShowInParking(item.EstimationId, item.Priority);
                //    if (!isValid) continue;
                //    resultSet.Add(item);
                //}

                long recordsTotal = result.Count;
                long recordsFiltered = recordsTotal;
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in result)
                {
                    var str = new List<string>();
                    var id = d.EstimationId;

                    str.Add(sl.ToString());
                    str.Add(d.Subject);
                    str.Add(d.EstimateIdentifier);
                    str.Add(d.EstimateType);
                    str.Add(Convert.ToDateTime(d.PlanStartDate, cultures).ToString("d"));
                    str.Add(Convert.ToDateTime(d.PlanEndDate, cultures).ToString("d"));
                    str.Add(d.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.CreateorFullName);

                    string actionTable = "";
                    if (user.Id == 16 && user.Username == "arif" && user.Email_Address == "arif@summit-centre.com")
                    {
                        string actionShowButtons = "<button class='btn btn-secondary'><a href='/BudgetApprover/Edit?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                        actionShowButtons += "<i class='fas fa-edit' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
                        actionShowButtons += "</a></button>";

                        string actionApproveButtons = "<button class='btn btn-success' data-toggle='modal' data-target='#remarksModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionApproveButtons += "<i class='fa fa-check' data-toggle='tooltip' data-placement='left' title='Approve'></i>";
                        actionApproveButtons += "</button>";

                        string actionRBButtons = "<button class='btn btn-warning' data-toggle='modal' data-target='#remarksRBModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionRBButtons += "<i class='fa fa-undo' data-toggle='tooltip' data-placement='left' title='Rollback'></i>";
                        actionRBButtons += "</button>";

                        string actionRejectButtons = "<button class='btn btn-danger' data-toggle='modal' data-target='#remarksRejectModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionRejectButtons += "<i class='fa fa-ban' data-toggle='tooltip' data-placement='left' title='Reject'></i>";
                        actionRejectButtons += "</button>";

                        actionTable = @"<table><tbody><tr><td>" + actionShowButtons + "</td><td>" +
                            actionApproveButtons + "</td><td>" + actionRBButtons + "</td><td>" +
                                actionRejectButtons + "</td></tr></tbody></table>";

                        str.Add(actionTable);
                    }
                    else
                    {
                        string actionButtons = "<a href='/BudgetEstimation/ViewEstimation?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                        actionButtons += "<i class='fa fa-eye fa-2x' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
                        actionButtons += "</a>";

                        str.Add(actionButtons);
                    }

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



        public async Task<IActionResult> Index()
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
       

        public async Task<IActionResult> LoadPendingBudget(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                List<EstimateVM> result = await _budgetService.LoadAllPendingEstimateByUser(user.Id);
                var data = new List<object>();
                List<EstimateVM> resultSet = new List<EstimateVM>();
                foreach (var item in result)
                {
                    bool isValid = await _budgetService.IsValidToShowInParking(item.EstimationId, item.Priority);
                    if (!isValid) continue;
                    resultSet.Add(item);
                }

                long recordsTotal = resultSet.Count;
                long recordsFiltered = recordsTotal;
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet)
                {
                    var str = new List<string>();
                    var id = d.EstimationId;

                    str.Add(sl.ToString());
                    str.Add(d.Subject);
                    str.Add(d.EstimateIdentifier);
                    str.Add(d.EstimateType);
                    str.Add(Convert.ToDateTime(d.PlanStartDate, cultures).ToString("d"));
                    str.Add(Convert.ToDateTime(d.PlanEndDate, cultures).ToString("d"));
                    var totalPriceWithCurrency = d.TotalPrice.ToString("#,#.##", CultureInfo.CreateSpecificCulture("hi-IN")) + "" +
                                                 Utility.CurrencyTypeConvertToStringFormat(d.CurrencyType);
                    str.Add(totalPriceWithCurrency);
                    str.Add(d.CreateorFullName);

                    string actionTable = "";
                    if (user.Id == 16 && user.Username == "arif" && user.Email_Address == "arif@summit-centre.com")
                    {
                        string actionShowButtons = "<button class='btn btn-secondary'><a href='/BudgetApprover/Edit?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                        actionShowButtons += "<i class='fas fa-edit' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
                        actionShowButtons += "</a></button>";

                        string actionApproveButtons = "<button class='btn btn-success' data-toggle='modal' data-target='#remarksModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionApproveButtons += "<i class='fa fa-check' data-toggle='tooltip' data-placement='left' title='Approve'></i>";
                        actionApproveButtons += "</button>";

                        string actionRBButtons = "<button class='btn btn-warning' data-toggle='modal' data-target='#remarksRBModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionRBButtons += "<i class='fa fa-undo' data-toggle='tooltip' data-placement='left' title='Rollback'></i>";
                        actionRBButtons += "</button>";

                        string actionRejectButtons = "<button class='btn btn-danger' data-toggle='modal' data-target='#remarksRejectModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionRejectButtons += "<i class='fa fa-ban' data-toggle='tooltip' data-placement='left' title='Reject'></i>";
                        actionRejectButtons += "</button>";

                        actionTable = @"<table><tbody><tr><td>" + actionShowButtons + "</td><td>" + 
                            actionApproveButtons + "</td><td>" + actionRBButtons + "</td><td>" + 
                                actionRejectButtons + "</td></tr></tbody></table>";

                        str.Add(actionTable);
                    }
                    else
                    {
                        string actionButtons = "<a href='/BudgetApprover/Edit?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                        actionButtons += "<i class='fas fa-edit fa-2x' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
                        actionButtons += "</a>";

                        str.Add(actionButtons);
                    }
                    
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
        public async Task<IActionResult> CompleteList()
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
        public async Task<IActionResult> CompleteList(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                var totalData = await _budgetService.LoadAllCompleteEstimate(0,
                    start, PAGE_SIZE, "", false);

                List<EstimateEditVM> resultSet = await _budgetService.LoadAllCompleteEstimate(0, 
                    start, PAGE_SIZE, "");
                int count = resultSet.Count > 0 ? totalData.Count : 0;
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
                    str.Add(d.EstimateType);
                    str.Add(d.EstimationIdentity);
                    str.Add(d.Subject);
                    str.Add(Convert.ToDateTime(d.PlanStartDate, cultures).ToString("d"));
                    str.Add(Convert.ToDateTime(d.PlanEndDate, cultures).ToString("d"));
                    var totalPriceWithCurrency = d.TotalPrice.ToString() + "" +
                                                 Utility.CurrencyTypeConvertToStringFormat(d.CurrencyType);
                    str.Add(totalPriceWithCurrency);

                    //string actionButtons = "<a href='/BudgetApprover/Detail?id=" + id + "' class='text-decoration-none'>";
                    string actionButtons = "<a href='/BudgetEstimation/ViewEstimation?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                    actionButtons += "<i class='fas fa-eye' data-toggle='tooltip' data-placement='left' title='Show'></i>";
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
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                EstimationEntity estimate = await _budgetService.GetEstimateById(id);
                List<EstimateDetailsEntity> estimateDetails = await _budgetService.LoadEstimateDetailByEstimationId(estimate.Id);
                List<ParticularWiseSummaryEntity> particularWiseSummary = await _budgetService.LoadParticularWiseSummaryByEstimationId(estimate.Id);
                List<DepartmentWiseSummaryEntity> departmentWiseSummary = await _budgetService.LoadDepartmentWiseSummaryByEstimationId(estimate.Id);
                List<EstimateApproverEntity> estimateApprovers = await _budgetService.LoadEstimateApproverByEstimationId(estimate.Id);

                EstimateEditVM editVM = new EstimateEditVM();
                //editVM.ProjectName = _budgetService.GetByProjectId(estimate.Project_Id);
                editVM.PlanEndDate = estimate.PlanEndDate;
                editVM.Subject = estimate.Subject;
                editVM.Objective = estimate.Objective;
                editVM.Details = estimate.Details;
                editVM.PlanStartDate = estimate.PlanStartDate;
                editVM.PlanEndDate = estimate.PlanEndDate;
                editVM.Remarks = estimate.Remarks;
                editVM.TotalPrice = estimate.TotalPrice;
                editVM.Id = estimate.Id;

                //TODO: Load estimate type from table
                editVM.EstimateType = estimate.EstimateType_Id.ToString();
                foreach (var item in estimateDetails)
                {
                    var particularItem = _itemService.GetById(item.Item_Id).Result;
                    var particularCat = _itemCategoryService.GetById(particularItem.ItemCategory_Id).Result;
                    var partcular = _particularService.GetById(particularCat.Particular_Id).Result;
                    var thana = _thanaService.GetById(item.Thana_Id).Result;
                    var dist = _distService.GetById(thana.District_Id).Result;

                    EstimateDetailsVM estimateDetailsVM = new EstimateDetailsVM();
                    estimateDetailsVM.AreaType = item.AreaType;
                    estimateDetailsVM.Estimation_Id = item.Estimation_Id;
                    estimateDetailsVM.ItemName = particularItem.Name;
                    estimateDetailsVM.CategoryName = particularCat.Name;
                    estimateDetailsVM.ParticularName = partcular.Name;
                    estimateDetailsVM.ItemCode = particularItem.ItemCode;

                    estimateDetailsVM.UnitPrice = item.UnitPrice;
                    estimateDetailsVM.TotalPrice = item.TotalPrice;
                    //TODO: load data from Unit table
                    estimateDetailsVM.Unit = particularItem.Unit_Id.ToString();

                    estimateDetailsVM.NoOfDayAndTotalUnit = item.NoOfDayAndTotalUnit;
                    estimateDetailsVM.NoOfMachineAndUsagesAndTeamAndCar = item.NoOfMachineAndUsagesAndTeamAndCar;
                    estimateDetailsVM.QuantityRequired = item.QuantityRequired;
                    estimateDetailsVM.Remarks = item.Remarks;
                    estimateDetailsVM.ResponsibleDepartment = _departmentService.GetById(item.ResponsibleDepartment_Id).Result.Name;
                    estimateDetailsVM.Thana = _thanaService.GetById(item.Thana_Id).Result.Name;
                    estimateDetailsVM.Thana = thana.Name;
                    estimateDetailsVM.District = dist.Name;

                    editVM.EstimateDetailsList.Add(estimateDetailsVM);
                }

                foreach (var item in estimateApprovers)
                {
                    var user = _userService.GetById(item.User_Id).Result;
                    EstimateApproverVM approverVM = new EstimateApproverVM();
                    approverVM.Estimate_Id = item.Estimate_Id;
                    approverVM.Priority = item.Priority;
                    approverVM.Status = item.Status;
                    approverVM.UserName = user.First_Name;
                    approverVM.UserDept = _departmentService.GetById(user.Department_Id).Result.Name;
                    editVM.EstimateApproverList.Add(approverVM);
                }

                foreach (var item in particularWiseSummary)
                {
                    ParticularWiseSummaryVM particularSummaryVM = new ParticularWiseSummaryVM();
                    particularSummaryVM.Estimate_Id = item.Estimate_Id;
                    particularSummaryVM.ParticularName = _particularService.GetById(item.Particular_Id).Result.Name;
                    particularSummaryVM.TotalPrice = item.TotalPrice;
                    particularSummaryVM.Particular_Id = _particularService.GetById(item.Particular_Id).Result.Id;
                    editVM.ParticularWiseSummaryList.Add(particularSummaryVM);
                }

                foreach (var item in departmentWiseSummary)
                {
                    DepartmentWiseSummaryVM deptSummaryVM = new DepartmentWiseSummaryVM();
                    deptSummaryVM.Estimate_Id = item.Estimate_Id;
                    deptSummaryVM.DepartmentName = _departmentService.GetById(item.Department_Id).Result.Name;
                    deptSummaryVM.TotalPrice = item.TotalPrice;
                    deptSummaryVM.Department_Id = _departmentService.GetById(item.Department_Id).Result.Id;
                    editVM.DepartmentWiseSummaryList.Add(deptSummaryVM);
                }

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

                return View(editVM);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var value = Convert.ToInt32(_protector.Unprotect(id));
                var response = await _budgetService.GetOneEstimationWithType(value);
                if(response.Estimation.EstimationStatus != "2")
                    return RedirectToPage("/Error/401");

                var user = await _sessionManager.GetUser();
                if(user == null)
                    _ = new Exception("Invalid Estimate");

                var latestApprovers = await _budgetApproverService.GetLatestPendingApprovers(value);
                bool checkFlag = false;
                foreach(var app in latestApprovers)
                {
                    if(app.User_Id == user.Id)
                    {
                        checkFlag = true;
                        break;
                    }
                }
                if(checkFlag == false)
                {
                    return RedirectToPage("/Error/401");
                }
                var responseToGetApprover = await _budgetApproverService.GetApproverByEstimateANDUser(new 
                    Models.ServiceModels.BudgetEstimate.BudgetApprover.GetApproverByEstimateAndUser()
                {
                    EstimationId = value,
                    UserId = user.Id
                });
                ViewBag.PriorityLevel = responseToGetApprover.Priority;
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
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string feedback, string remarks)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var estimate = await _budgetService.GetEstimateById(Convert.ToInt32(id));
                if (estimate == null)
                    _ = new Exception("Invalid Estimate");

                if (estimate.Status != "2")
                {
                    return Json("Estimate Status has been changed.");
                }
                var latestApprovers = await _budgetApproverService.GetLatestPendingApprovers(Convert.ToInt32(estimate.Id));
                bool checkFlag = false;
                foreach (var app in latestApprovers)
                {
                    if (app.User_Id == user.Id)
                    {
                        checkFlag = true;
                        break;
                    }
                }
                if (checkFlag == false)
                {
                    return RedirectToPage("/Error/401");
                }
                List<EstimateApproverEntity> estimateApprovers = await _budgetService.LoadEstimateApproverByEstimationId(estimate.Id);
                var approverList = estimateApprovers.Where(x => x.Status == EstimationEntity.EntityStatus.Pending.ToString() && (x.RolePriority_Id == 1 || x.RolePriority_Id == 2)).ToList();
                //last Approver for this Budget
                if (approverList.Count == 1 && approverList[0].User_Id == user.Id && feedback == EstimationEntity.EntityStatus.Completed.ToString())
                {
                    _budgetService.CompleteBudget(estimate.Id, user.Id, EstimationEntity.EntityStatus.Completed);
                }


                var responseToGetApprover = await _budgetApproverService.GetApproverByEstimateANDUser(new Models.ServiceModels.BudgetEstimate.BudgetApprover.GetApproverByEstimateAndUser()
                {
                    EstimationId = estimate.Id,
                    UserId = user.Id
                });

                var responseToUpdateApproverStatus = await _budgetApproverService.UpdateEstimateApproverStatusById(responseToGetApprover.Id, feedback, remarks);

                if (BaseEntity.EntityStatus.CR == Convert.ToInt32(feedback) || BaseEntity.EntityStatus.Reject == Convert.ToInt32(feedback))
                {
                    await _budgetService.UpdateEstimation(new BudgetEstimationUpdateRequest()
                    {
                        EstimateId = estimate.Id,
                        EstimateType = estimate.EstimateType_Id,
                        Subject = estimate.Subject,
                        Objective = estimate.Objective,
                        Details = estimate.Details,
                        PlanEnd = estimate.PlanEndDate,
                        PlanStart = estimate.PlanStartDate,
                        TotalPrice = estimate.TotalPrice,
                        TotalPriceRemarks = estimate.TotalPriceRemarks,
                        CurrencyType = estimate.CurrencyType

                    }, Convert.ToInt32(feedback));
                }

                var responseToAddApproverFeedback = await _budgetApproverService.CreateApproverFeedBack(new Models.ServiceModels.BudgetEstimate.BudgetApprover.CreateApproverFeedBackServiceRequest()
                {
                    EstimateApprover_Id = responseToGetApprover.Id,
                    Estimation_Id = responseToGetApprover.Estimate_Id,
                    FeedbackRemarks = remarks,
                    Status = Convert.ToInt32(feedback)
                });

                return Json(1);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(e);
            }
        }




        [HttpPost]
        public async Task<IActionResult> FinalRecomendorFeedback(string id, string feedback, string remarks)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                if (user.Id != 20)
                {
                    return RedirectToHome();
                }

                var estimate = await _budgetService.GetEstimateById(Convert.ToInt32(id));
                if (estimate == null)
                    _ = new Exception("Invalid Estimate");

                if (estimate.Status != "2")
                {
                    return Json("Estimate Status has been changed.");
                }
                
                    _budgetService.CompleteBudget(estimate.Id, user.Id, EstimationEntity.EntityStatus.Completed);
                


                var responseToGetApprover = await _budgetApproverService.GetApproverByEstimateANDUser(new Models.ServiceModels.BudgetEstimate.BudgetApprover.GetApproverByEstimateAndUser()
                {
                    EstimationId = estimate.Id,
                    UserId = user.Id
                });

                var responseToUpdateApproverStatus = await _budgetApproverService.UpdateEstimateApproverStatusById(responseToGetApprover.Id, feedback, remarks);
                var responseFinalApproverConvertToInformer = await _budgetApproverService.FinalApproverConvertToInformer(estimate.Id);

                

                var responseToAddApproverFeedback = await _budgetApproverService.CreateApproverFeedBack(new Models.ServiceModels.BudgetEstimate.BudgetApprover.CreateApproverFeedBackServiceRequest()
                {
                    EstimateApprover_Id = responseToGetApprover.Id,
                    Estimation_Id = responseToGetApprover.Estimate_Id,
                    FeedbackRemarks = remarks,
                    Status = Convert.ToInt32(feedback)
                });

                return Json(1);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReRegisterParticularItemByEstimate(/*AddBudgetEstimation*/ string requestDto)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<AddBudgetEstimation>(requestDto);
                await _budgetService.ReDefineEstimationDetailsAndSummaries(jsonData);
                return Json(1);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetApproverRemarkList(int estimationId)
        {
            try
            {
                var response = await _budgetApproverService.LoadApproverRemarksService(estimationId);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
                throw;
            }
        }

        public async Task<IActionResult> RollbackEstimation(string id, string feedback, string remarks)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var estimate = await _budgetService.GetEstimateById(Convert.ToInt32(id));
                if (estimate == null)
                {
                    var e = new Exception("Invalid Estimate");
                    _logger.LogError(e, e.Message);
                    return Json(e);
                }
                    
                if (estimate.Status == EstimationEntity.EntityStatus.Pending.ToString())
                {
                    if (BaseEntity.EntityStatus.CR == Convert.ToInt32(feedback) || BaseEntity.EntityStatus.Reject == Convert.ToInt32(feedback))
                    {
                        await _budgetService.UpdateEstimation(new BudgetEstimationUpdateRequest()
                        {
                            EstimateId = estimate.Id,
                            EstimateType = estimate.EstimateType_Id,
                            Subject = estimate.Subject,
                            Objective = estimate.Objective,
                            Details = estimate.Details,
                            PlanEnd = estimate.PlanEndDate,
                            PlanStart = estimate.PlanStartDate,
                            TotalPrice = estimate.TotalPrice,
                            TotalPriceRemarks = estimate.TotalPriceRemarks
                            
                        }, Convert.ToInt32(feedback));
                    }

                    var responseToAddApproverFeedback = await _budgetApproverService.CreateApproverFeedBack(new Models.ServiceModels.BudgetEstimate.BudgetApprover.CreateApproverFeedBackServiceRequest()
                    {
                        EstimateApprover_Id = user.Id,
                        Estimation_Id = estimate.Id,
                        FeedbackRemarks = remarks,
                        Status = Convert.ToInt32(feedback)
                    });

                    return Json(1);
                }
                else
                {
                    var e = new Exception("This Estimate is not in pending status");
                    _logger.LogError(e, e.Message);
                    return Json(e);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(e);
            }
        }


        [AuthorizePermission(PermissionKeys.RejectedBudgetForSepecificUser)]
        public async Task<IActionResult> AllRejectedBudgetParkingForSpecificUser()
        {
            try
            {


                // var user = await _sessionManager.GetUser();

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
        public async Task<IActionResult> LoadAllRejectedBudgetParkingForSpecificUser(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                var follower = await _userService.GetFollowersByUserId(user.Id);
                if (follower == null)
                {
                    return RedirectToHome();
                }
                List<EstimateVM> result = await _budgetService.LoadAllRejectedEstimateBySpecificUser(follower.FollowerUserId);
                var data = new List<object>();
                //List<EstimateVM> resultSet = new List<EstimateVM>();
                //foreach (var item in result)
                //{
                //    bool isValid = await _budgetService.IsValidToShowInParking(item.EstimationId, item.Priority);
                //    if (!isValid) continue;
                //    resultSet.Add(item);
                //}

                long recordsTotal = result.Count;
                long recordsFiltered = recordsTotal;
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in result)
                {
                    var str = new List<string>();
                    var id = d.EstimationId;

                    str.Add(sl.ToString());
                    str.Add(d.Subject);
                    str.Add(d.EstimateIdentifier);
                    str.Add(d.EstimateType);
                    str.Add(Convert.ToDateTime(d.PlanStartDate, cultures).ToString("d"));
                    str.Add(Convert.ToDateTime(d.PlanEndDate, cultures).ToString("d"));
                    str.Add(d.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.CreateorFullName);

                    string actionTable = "";
                    if (user.Id == 16 && user.Username == "arif" && user.Email_Address == "arif@summit-centre.com")
                    {
                        string actionShowButtons = "<button class='btn btn-secondary'><a href='/BudgetApprover/Edit?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                        actionShowButtons += "<i class='fas fa-edit' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
                        actionShowButtons += "</a></button>";

                        string actionApproveButtons = "<button class='btn btn-success' data-toggle='modal' data-target='#remarksModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionApproveButtons += "<i class='fa fa-check' data-toggle='tooltip' data-placement='left' title='Approve'></i>";
                        actionApproveButtons += "</button>";

                        string actionRBButtons = "<button class='btn btn-warning' data-toggle='modal' data-target='#remarksRBModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionRBButtons += "<i class='fa fa-undo' data-toggle='tooltip' data-placement='left' title='Rollback'></i>";
                        actionRBButtons += "</button>";

                        string actionRejectButtons = "<button class='btn btn-danger' data-toggle='modal' data-target='#remarksRejectModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionRejectButtons += "<i class='fa fa-ban' data-toggle='tooltip' data-placement='left' title='Reject'></i>";
                        actionRejectButtons += "</button>";

                        actionTable = @"<table><tbody><tr><td>" + actionShowButtons + "</td><td>" +
                            actionApproveButtons + "</td><td>" + actionRBButtons + "</td><td>" +
                                actionRejectButtons + "</td></tr></tbody></table>";

                        str.Add(actionTable);
                    }
                    else
                    {
                        string actionButtons = "<a href='/BudgetEstimation/ViewEstimation?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                        actionButtons += "<i class='fa fa-eye fa-2x' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
                        actionButtons += "</a>";

                        str.Add(actionButtons);
                    }

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




        [AuthorizePermission(PermissionKeys.RunningBudgetForSepecificUser)]
        public async Task<IActionResult> AllRunningBudgetParkingForSpecificUser()
        {
            try
            {
                // var user = await _sessionManager.GetUser();

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
        public async Task<IActionResult> LoadAllRunningBudgetParkingForSpecificUser(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                var follower = await _userService.GetFollowersByUserId(user.Id);
                if (follower == null)
                {
                    return RedirectToHome();
                }
                List<EstimateVM> result = await _budgetService.LoadAllRunningEstimateBySpecificUser(follower.FollowerUserId);
                var data = new List<object>();
                //List<EstimateVM> resultSet = new List<EstimateVM>();
                //foreach (var item in result)
                //{
                //    bool isValid = await _budgetService.IsValidToShowInParking(item.EstimationId, item.Priority);
                //    if (!isValid) continue;
                //    resultSet.Add(item);
                //}

                long recordsTotal = result.Count;
                long recordsFiltered = recordsTotal;
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in result)
                {
                    var str = new List<string>();
                    var id = d.EstimationId;

                    str.Add(sl.ToString());
                    str.Add(d.Subject);
                    str.Add(d.EstimateIdentifier);
                    str.Add(d.EstimateType);
                    str.Add(Convert.ToDateTime(d.PlanStartDate, cultures).ToString("d"));
                    str.Add(Convert.ToDateTime(d.PlanEndDate, cultures).ToString("d"));
                    str.Add(d.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.CreateorFullName);

                    string actionTable = "";
                    if (user.Id == 16 && user.Username == "arif" && user.Email_Address == "arif@summit-centre.com")
                    {
                        string actionShowButtons = "<button class='btn btn-secondary'><a href='/BudgetApprover/Edit?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                        actionShowButtons += "<i class='fas fa-edit' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
                        actionShowButtons += "</a></button>";

                        string actionApproveButtons = "<button class='btn btn-success' data-toggle='modal' data-target='#remarksModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionApproveButtons += "<i class='fa fa-check' data-toggle='tooltip' data-placement='left' title='Approve'></i>";
                        actionApproveButtons += "</button>";

                        string actionRBButtons = "<button class='btn btn-warning' data-toggle='modal' data-target='#remarksRBModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionRBButtons += "<i class='fa fa-undo' data-toggle='tooltip' data-placement='left' title='Rollback'></i>";
                        actionRBButtons += "</button>";

                        string actionRejectButtons = "<button class='btn btn-danger' data-toggle='modal' data-target='#remarksRejectModal' data-uniqueIdentifier='" + d.EstimateIdentifier + "' data-id='" + id.ToString() + "'>";
                        actionRejectButtons += "<i class='fa fa-ban' data-toggle='tooltip' data-placement='left' title='Reject'></i>";
                        actionRejectButtons += "</button>";

                        actionTable = @"<table><tbody><tr><td>" + actionShowButtons + "</td><td>" +
                            actionApproveButtons + "</td><td>" + actionRBButtons + "</td><td>" +
                                actionRejectButtons + "</td></tr></tbody></table>";

                        str.Add(actionTable);
                    }
                    else
                    {
                        string actionButtons = "<a href='/BudgetEstimation/ViewEstimation?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                        actionButtons += "<i class='fa fa-eye fa-2x' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
                        actionButtons += "</a>";

                        str.Add(actionButtons);
                    }

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
    }
}

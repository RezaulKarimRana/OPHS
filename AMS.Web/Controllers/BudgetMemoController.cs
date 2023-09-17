using AMS.Models.ServiceModels.Dashboard;
using AMS.Models.ServiceModels.Memo;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo.Models;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
using AMS.Services.Managers.Contracts;
using AMS.Services.Memo.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class BudgetMemoController : BaseController
    {
        private readonly ILogger<BudgetMemoController> _logger;
        private readonly IBudgetService _budgetService;
        IDataProtector _protector;
        private readonly ISessionManager _sessionManager;
        private readonly IConfiguration _configuration;
        private readonly IDashboardService _dashboardService;
        private readonly IMemoService _memoService;
        private readonly IWebHostEnvironment hostingEnv;

        public BudgetMemoController(ILoggerFactory loggerFactory, IBudgetService budgetService, IWebHostEnvironment env,
            ISessionManager sessionManager, IDataProtectionProvider provider, IConfiguration configuration,
            IDashboardService dashboardService, IMemoService memoService)
        {
            _logger = loggerFactory.CreateLogger<BudgetMemoController>();
            _budgetService = budgetService;
            _sessionManager = sessionManager;
            _configuration = configuration;
            _dashboardService = dashboardService;
            _memoService = memoService;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
            hostingEnv = env;
        }

        [HttpGet]
        public async Task<IActionResult> ReadyForMemoList()
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null || user.Department_Id != 5) return RedirectToHome();


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
        public async Task<IActionResult> LoadReadyForMemoList(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null || user.Department_Id != 5) return RedirectToHome();

                var resultSet = await _memoService.LoadAllPendingMemoService();
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
                    var estimateReferId = d.EstimateReferenceId;
                    str.Add(sl.ToString());
                    str.Add(d.UniqueIdentifier);
                    str.Add(d.EstimateType);
                    str.Add(d.EstimateSubject);
                    str.Add(d.TotalAllowableBudget.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.TotalCost.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.Deviation.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.Percentage.ToString());
                    string actionButtons = "<a href='/BudgetMemo/Create?id=" + _protector.Protect(estimateReferId.ToString()) + "' class='btn btn-outline-primary text-decoration-none' >";
                    actionButtons += "<i class='fa fa-edit' data-toggle='tooltip' data-placement='left' title='Create Memo'></i> Create</a>";
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
        public async Task<IActionResult> Create(string id)
        {
            var user = await _sessionManager.GetUser();
            if (user == null || user.Department_Id != 5) return RedirectToHome();

            var value = Convert.ToInt32(_protector.Unprotect(id));

            var estimateReference = await _memoService.GetEstimationReferenceByIdService(value);

            if (estimateReference == null) return RedirectToHome();

            var response = await _budgetService.EstimationInfoService(estimateReference.EstimationId);
            response.AllowableBudget = estimateReference.AllowableBudget;
            response.Deviation = response.TotalCost - response.AllowableBudget;
            response.Percentage = (response.TotalCost / response.AllowableBudget)*100;
            response.EstimateIdProtected = _protector.Protect(estimateReference.EstimationId.ToString());
            response.CanDeleteAttachments = true;
            #region Nav Menu Count
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
            #endregion

            return View(response);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _sessionManager.GetUser();
            if (user == null || user.Department_Id != 5) return RedirectToHome();

            var value = Convert.ToInt32(_protector.Unprotect(id));

            var estimateMemo = await _memoService.GetEstimateMemoEntityByIdService(value);

            if (estimateMemo == null)
                return RedirectToPage("/Error/401");

            var estimateReference = await _memoService.GetEstimationReferenceByIdService(estimateMemo.EstimateReferenceId);

            if (estimateReference == null)
                return RedirectToPage("/Error/401");

            var response = await _memoService.EstimationMemoInfoDetailsByIdService(estimateMemo.Id);
            response.AllowableBudget = estimateReference.AllowableBudget;
            response.Deviation = response.TotalCost - response.AllowableBudget;
            response.Percentage = (response.TotalCost / response.AllowableBudget) * 100;
            response.EstimateMemoAttachments = await _memoService.LoadMemoAttachmentsByMemoService(estimateMemo.Id);
            response.EstimateIdProtected = _protector.Protect(estimateReference.EstimationId.ToString());
            response.CanDeleteAttachments = true;
            #region Nav Menu Count
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
            #endregion

            return View(response);
        }
        [HttpGet]
        public async Task<IActionResult> ApproverAction(string id)
        {
            var value = Convert.ToInt32(_protector.Unprotect(id));

            var estimateMemo = await _memoService.GetEstimateMemoEntityByIdService(value);

            if (estimateMemo == null)
                return RedirectToPage("/Error/401");

            var estimateReference = await _memoService.GetEstimationReferenceByIdService(estimateMemo.EstimateReferenceId);

            if (estimateReference == null)
                return RedirectToPage("/Error/401");

            var response = await _memoService.EstimationMemoInfoDetailsByIdService(estimateMemo.Id);
            response.AllowableBudget = estimateReference.AllowableBudget;
            response.Deviation = response.TotalCost - response.AllowableBudget;
            response.Percentage = (response.TotalCost / response.AllowableBudget) * 100;
            response.EstimateMemoAttachments = await _memoService.LoadMemoAttachmentsByMemoService(estimateMemo.Id);
            response.EstimateIdProtected = _protector.Protect(estimateReference.EstimationId.ToString());
            response.CanDeleteAttachments = false;
            #region Nav Menu Count
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
            #endregion

            return View(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEstimationSettleItemDetailsByEstimate(int estiId)
        {
            try
            {
                if (estiId < 1)
                {
                    throw new Exception("Invalid ID");
                }
                var response = await _memoService.LoadEstimationSettleItemDetailsByEstimationIdService(estiId);
                return new JsonResult(response);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoadSettledItemDetails(int estimateId, int settleitemId)
        {
            try
            {
                if (estimateId < 1 || settleitemId < 1)
                {
                    throw new Exception("Invalid ID");
                }
                var response = await _memoService.LoadSettleItemDetailsBySettleItemAndEstimationIdService(estimateId, settleitemId);
                return new JsonResult(response);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<JsonResult> PostEstimationMemo(string requestDto)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null || user.Department_Id != 5)
                {
                    return Json("Invalid");
                }

                var jsonData = JsonConvert.DeserializeObject<AddBudgetEstimationMemoDTO>(requestDto);

                var estimateReference = await _memoService.GetEstimationReferenceByIdService(jsonData.EstimateMemo.EstimateReferId);

                if (estimateReference == null)
                    throw new Exception("Invalid EstimateReference");

                if(estimateReference.EstimationId != jsonData.EstimateMemo.EstimateId)
                    throw new Exception("Invalid EstimateReference OR Estimation");

                var result = await _memoService.SaveEstimationMemoService(jsonData);
                return Json(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }
        [HttpPost]
        public async Task<JsonResult> UploadFilesOfMemo(int estimationMemoId)
        {
            var user = await _sessionManager.GetUser();
            if (user == null || user.Department_Id != 5)
            {
                return Json("Invalid");
            }

            string result;
            try
            {
                long size = 0;
                var files = Request.Form.Files;
                string uploads = Path.Combine(hostingEnv.WebRootPath, $@"uploadedFiles\MemoFiles\");
                foreach (var file in files)
                {
                    string FilePath = Path.Combine(uploads, file.FileName);

                    size += file.Length;
                    using (FileStream fs = System.IO.File.Create(FilePath))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                    var response = await _memoService.SaveEstimateMemoAttachment(new CreateAttachmentForMemoRequest()
                    {
                        URL = FilePath,
                        FileName = file.FileName,
                        EstimationMemo_Id = estimationMemoId
                    });
                }

                return Json(1);
            }

            catch (Exception ex)
            {
                result = ex.Message;
                return Json(result);
            }

        }
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var value = Convert.ToInt32(_protector.Unprotect(id));

                var estimateMemo = await _memoService.GetEstimateMemoEntityByIdService(value);

                if (estimateMemo == null)
                    return RedirectToPage("/Error/401");

                var estimateReference = await _memoService.GetEstimationReferenceByIdService(estimateMemo.EstimateReferenceId);

                if (estimateReference == null)
                    return RedirectToPage("/Error/401");

                var response = await _memoService.EstimationMemoInfoDetailsByIdService(estimateMemo.Id);
                response.AllowableBudget = estimateReference.AllowableBudget;
                response.Deviation = response.TotalCost - response.AllowableBudget;
                response.Percentage = (response.TotalCost / response.AllowableBudget) * 100;
                response.EstimateMemoAttachments = await _memoService.LoadMemoAttachmentsByMemoService(estimateMemo.Id);
                response.EstimateIdProtected = _protector.Protect(estimateReference.EstimationId.ToString());
                response.CanDeleteAttachments = false;
                #region Nav Bar Menu Count
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
                #endregion

                return View(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> MyList()
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                var responseForNav = await _dashboardService.GetNavBarCount();
                var summaryCount = await _memoService.LoadMemoSummary();
                if (responseForNav.IsSuccessful)
                {
                    ViewData["MemoSummary"] = new MemoSummaryVM
                    {
                        Pending = summaryCount.Pending,
                        CR = summaryCount.CR,
                        Completed = summaryCount.Completed,
                        Rejected = summaryCount.Rejected,
                        Total = summaryCount.Total
                    };
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
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> MyApprovals()
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                var responseForNav = await _dashboardService.GetNavBarCount();
                var summaryCount = await _memoService.LoadApproverMemoSummary();
                if (responseForNav.IsSuccessful)
                {
                    ViewData["MemoSummary"] = new MemoSummaryVM
                    {
                        Pending = summaryCount.Pending,
                        CR = summaryCount.CR,
                        Completed = summaryCount.Completed,
                        Rejected = summaryCount.Rejected,
                        Total = summaryCount.Total
                    };
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
                throw;
            }
        }
        public async Task<IActionResult> LoadApproverMemoByStatus(int statusId, int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var resultSet = await _memoService.LoadApproverMemoByStatus(user.Id, statusId, start, PAGE_SIZE);
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
                    var estimateReferId = d.EstimateReferenceId;
                    str.Add(sl.ToString());
                    str.Add(d.EstimationIdentity);
                    str.Add(d.EstimateType);
                    str.Add(d.Subject);
                    str.Add(d.BudgetPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.AllowableBudget.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.TotalCost.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.TotalDeviation.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    string actionButtons = string.Empty;
                    if (statusId == 2)
                    {
                        actionButtons += "<a href='/BudgetMemo/ApproverAction?id=" + _protector.Protect(id.ToString()) + "' class='btn btn-outline-primary text-decoration-none' >";
                        actionButtons += "<i class='fa fa-edit' data-toggle='tooltip' data-placement='left' title='Check Memo'></i> Check</a>";
                    }
                    else
                    {
                        actionButtons += "<a href='/BudgetMemo/Details?id=" + _protector.Protect(id.ToString()) + "' class='btn btn-outline-primary text-decoration-none' >";
                        actionButtons += "<i class='fa fa-eye' data-toggle='tooltip' data-placement='left' title='View'></i> View</a>";
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
        public async Task<IActionResult> LoadMemoByStatus(int statusId, int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var resultSet = await _memoService.LoadMemoByStatus(user.Id, statusId, start, PAGE_SIZE);
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
                    var estimateReferId = d.EstimateReferenceId;
                    str.Add(sl.ToString());
                    str.Add(d.EstimationIdentity);
                    str.Add(d.EstimateType);
                    str.Add(d.Subject);
                    str.Add(d.BudgetPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.AllowableBudget.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.TotalCost.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.TotalDeviation.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    string actionButtons = string.Empty;
                    if (statusId == -404)
                    {
                        actionButtons += "<a href='/BudgetMemo/Edit?id=" + _protector.Protect(id.ToString()) + "' class='btn btn-outline-primary text-decoration-none' >";
                        actionButtons += "<i class='fa fa-edit' data-toggle='tooltip' data-placement='left' title='Edit Memo'></i> Edit</a>";
                    }
                    else
                    {
                        actionButtons += "<a href='/BudgetMemo/Details?id=" + _protector.Protect(id.ToString()) + "' class='btn btn-outline-primary text-decoration-none' >";
                        actionButtons += "<i class='fa fa-eye' data-toggle='tooltip' data-placement='left' title='View'></i> View</a>";
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
        [HttpPost]
        public async Task<IActionResult> SaveMemoApproval(string requestDto)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var requestData = JsonConvert.DeserializeObject<ApprovalFeedBackDTO>(requestDto);

                var estimateMemo = await _memoService.GetEstimateMemoEntityByIdService(requestData.MemoId);

                if (estimateMemo == null)
                    return RedirectToPage("/Error/401");

                if (estimateMemo.Status != "2")
                {
                    return Json("Estimate Memo Status has been changed.");
                }

                var latestApprovers = await _memoService.GetLatestPendingApproveresOfAMemoService(estimateMemo.Id);
                bool checkFlag = false;
                foreach (var app in latestApprovers)
                {
                    if (app.User_Id == user.Id)
                    {
                        checkFlag = true;
                        break;
                    }
                }
                if (!checkFlag)
                {
                    return RedirectToPage("/Error/401");
                }

                var response = await _memoService.SaveApproverApprovalFeedBack(estimateMemo.Id, requestData.Feedback, requestData.Remarks, requestData.IsFinalApproved);
                if (!response)
                {
                    throw new Exception("Something Went Wrong");
                }

                return Json(1);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoadAllApproverByMemo(int memoId)
        {
            try
            {
                var response = await _memoService.LoadMemoApproverDetailsByMemoIdService(memoId);
                return new JsonResult(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> LoadApproverFeedBackByMemo(int memoId)
        {
            try
            {
                var response = await _memoService.LoadMemoApproverFeedBackDetailsService(memoId);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }
        [HttpGet]
        public async Task<JsonResult> DeleteAttachmentsById(int id)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return (JsonResult)RedirectToHome();

                var response = await _memoService.DeleteAttachmentsById(id);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }
    }
}

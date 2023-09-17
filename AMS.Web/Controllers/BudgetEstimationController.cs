using AMS.Common.Helpers;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.Dashboard;
using AMS.Models.ViewModel;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
using AMS.Services.Managers.Contracts;
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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class BudgetEstimationController : BaseController
    {
        private readonly ILogger<BudgetEstimationController> _logger;
        private readonly IBudgetService _budgetService;
        IDataProtector _protector;
        private readonly ISessionManager _sessionManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment hostingEnv;
        private readonly IDashboardService _dashboardService;

        public BudgetEstimationController(ILoggerFactory loggerFactory, IBudgetService budgetService, IWebHostEnvironment env,
            ISessionManager sessionManager, IDataProtectionProvider provider, IConfiguration configuration, IDashboardService dashboardService)
        {
            _logger = loggerFactory.CreateLogger<BudgetEstimationController>();
            _budgetService = budgetService;
            _sessionManager = sessionManager;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
            _dashboardService = dashboardService;
            hostingEnv = env;
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
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<JsonResult> PostBudgetEstimation(/*AddBudgetEstimation*/string requestDto)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<AddBudgetEstimation>(requestDto);
                var result = await _budgetService.SaveBudgetData(jsonData);
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
        public FileResult DownloadFormatedExcelFile()
        {
            var reportPath = Path.Combine(hostingEnv.WebRootPath, $@"FormatedExcelFile\BudgetItemBulk.xlsx");
            var fileByte = System.IO.File.ReadAllBytes(reportPath);
            return File(fileByte, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BudgetItemBulk.xlsx");
        }

        [HttpPost]
        public async Task<JsonResult> UploadFilesOfEstimation(int estimationId)
        {
            string result;
            try
            {
                long size = 0;
                var files = Request.Form.Files;
                var estimation = await _budgetService.GetEstimateById(estimationId);
                string uploads = Path.Combine(hostingEnv.WebRootPath, $@"uploadedFiles\NewFolder\");
                foreach (var file in files)
                {
                    string visiableFileName = file.FileName;
                    string newFileNameWithEstimationId= estimation.UniqueIdentifier + "_" + file.FileName;

                    string FilePath = Path.Combine(uploads, newFileNameWithEstimationId);

                    size += file.Length;
                    using (FileStream fs = System.IO.File.Create(FilePath))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                    var response = await _budgetService.CreateEstimateAttachment(new CreateAttachmentServiceRequest()
                    {
                        URL = FilePath,
                        FileName = visiableFileName,
                        Estimation_Id = estimationId
                    });
                }

                return Json("Uploaded");
            }

            catch (Exception ex)
            {
                result = ex.Message;
                return Json(result);
            }

        }

        [HttpPost]
        public async Task<JsonResult> DraftBudgetEstimation(/*AddBudgetEstimation*/ string requestDto)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<AddBudgetEstimation>(requestDto);

                var result = await _budgetService.DraftBudgetData(jsonData);
                return Json(result);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<JsonResult> DisableEstimation(int esitmationId)
        {
            try
            {
                var result = await _budgetService.DisabledResponse(esitmationId);
                return Json(result);

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }

        [HttpGet]
        public async Task<JsonResult> LoadEstimateAttachmentsByEstimateId(int estimateId)
        {
            try
            {
                var response = await _budgetService.LoadAttachmentsByEstimateId(estimateId);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DraftList()
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
        public async Task<IActionResult> DraftList(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var resultSet = await _budgetService.LoadDraftEstimation(user.Id, start, PAGE_SIZE);
                int count = resultSet.DraftedBudgest.Count > 0 ? resultSet.DraftedBudgest[0].TotalRow : 0;
                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet.DraftedBudgest)
                {
                    var str = new List<string>();
                    var id = d.EstimationId;

                    str.Add(sl.ToString());
                    str.Add(d.EstimationTypeName);
                    str.Add(d.EstimationIdentity);
                    str.Add(d.EstimationSubject);
                    str.Add(Convert.ToDateTime(d.EstimationPlanStart, cultures).ToString("d"));
                    str.Add(Convert.ToDateTime(d.EstimationPlanEnd, cultures).ToString("d"));
                    var totalPriceWithCurrency = d.EstimationTotalPrice.ToString() + "" +
                                                 Utility.CurrencyTypeConvertToStringFormat(d.CurrencyType);
                    str.Add(totalPriceWithCurrency);

                    string actionButtons = "<a href='/BudgetEstimation/EditDraft?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                    actionButtons += "<i class='fas fa-edit' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
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

        public async Task<IActionResult> EditDraft(string id)
        {
            var value = Convert.ToInt32(_protector.Unprotect(id));
            var estimation = await _budgetService.GetEstimateById(value);
            if (estimation == null)
            {
                return RedirectToPage("/Error/401");
            }
            var sessionUser = await _sessionManager.GetUser();
            if (estimation.Created_By != sessionUser.Id)
            {
                return RedirectToPage("/Error/401");
            }
            var response = await _budgetService.GetOneEstimationWithType(value);
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

        [HttpGet]
        public async Task<IActionResult> RollbackList()
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
        public async Task<IActionResult> RollbackList(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var resultSet = await _budgetService.LoadCREstimation(user.Id, start, PAGE_SIZE);
                int count = resultSet.DraftedBudgest.Count > 0 ? resultSet.DraftedBudgest[0].TotalRow : 0;
                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet.DraftedBudgest)
                {
                    var str = new List<string>();
                    var id = d.EstimationId;

                    str.Add(sl.ToString());
                    str.Add(d.EstimationTypeName);
                    str.Add(d.EstimationIdentity);
                    str.Add(d.EstimationSubject);
                    str.Add(Convert.ToDateTime(d.EstimationPlanStart, cultures).ToString("d"));
                    str.Add(Convert.ToDateTime(d.EstimationPlanEnd, cultures).ToString("d"));
                    var totalPriceWithCurrency = d.EstimationTotalPrice.ToString() + "" +
                                                 Utility.CurrencyTypeConvertToStringFormat(d.CurrencyType);
                    str.Add(totalPriceWithCurrency);

                    string actionButtons = "<a href='/BudgetEstimation/EditRollBack?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                    actionButtons += "<i class='fas fa-edit' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
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

        public async Task<IActionResult> EditRollBack(string id)
        {
            try
            {
                var value = Convert.ToInt32(_protector.Unprotect(id));
                var estimation = await _budgetService.GetEstimateById(value);
                if (estimation == null)
                {
                    return RedirectToPage("/Error/401");
                }
                var sessionUser = await _sessionManager.GetUser();
                if (estimation.Created_By != sessionUser.Id)
                {
                    return RedirectToPage("/Error/401");
                }
                var response = await _budgetService.GetOneEstimationWithType(value);
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
        public async Task<IActionResult> ViewEstimation(string id)
        {
            try
            {
                var value = Convert.ToInt32(_protector.Unprotect(id));
                var response = await _budgetService.GetOneEstimationWithType(value);
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

        public async Task<IActionResult> OnGoingApprovalForInitiator()
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                if(user.CanRollBack == false)
                    return RedirectToPage("/Error/401");

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

        public async Task<IActionResult> OngoinBudgetApprovalList(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var resultSet = await _budgetService.LoadOngoinEstimationByUserService(user.Id);
                int count = resultSet.DraftedBudgest.Count > 0 ? resultSet.DraftedBudgest[0].TotalRow : 0;
                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");
                foreach (var d in resultSet.DraftedBudgest)
                {
                    var str = new List<string>();
                    var id = d.EstimationId;

                    str.Add(sl.ToString());
                    str.Add(d.EstimationTypeName);
                    str.Add(d.EstimationIdentity);
                    str.Add(d.EstimationSubject);
                    str.Add(Convert.ToDateTime(d.EstimationPlanStart, cultures).ToString("d"));
                    str.Add(Convert.ToDateTime(d.EstimationPlanEnd, cultures).ToString("d"));
                    str.Add(d.EstimationTotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")));
                    str.Add(d.EstimateInitiatorWithDept);

                    string actionButtons = "<a href='/BudgetEstimation/RollbackEstimationByInitiator?id=" + _protector.Protect(id.ToString()) + "' class='text-decoration-none'>";
                    actionButtons += "<i class='fas fa-edit' data-toggle='tooltip' data-placement='left' title='Edit'></i>";
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
        public async Task<IActionResult> RollbackEstimationByInitiator(string id)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                if (user.CanRollBack == false)
                    return RedirectToPage("/Error/401");

                var value = Convert.ToInt32(_protector.Unprotect(id));
                var response = await _budgetService.GetOneEstimationWithType(value);
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
                return Json(e); throw;
            }
        }
    }
}

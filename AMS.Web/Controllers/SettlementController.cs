using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AMS.Common.Constants;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.Dashboard;
using AMS.Models.ServiceModels.Settlement;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.SettlementRepo.Models;
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
    public class SettlementController : BaseController
    {
        private readonly ILogger<BudgetEstimationController> _logger;
        private readonly IBudgetService _budgetService;
        IDataProtector _protector;
        private readonly ISessionManager _sessionManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment hostingEnv;
        private readonly IDashboardService _dashboardService;
        private readonly ISettlementService _settlementService;
        private readonly ISettlementItemService _settlementItemService;
        private readonly IBudgetApproverService _budgetApproverService;

        public SettlementController(ILoggerFactory loggerFactory, IBudgetService budgetService, IWebHostEnvironment env,
            ISessionManager sessionManager, IDataProtectionProvider provider, IConfiguration configuration,
            IDashboardService dashboardService,ISettlementItemService settlementItemService
            , ISettlementService settlementService)
        {
            _logger = loggerFactory.CreateLogger<BudgetEstimationController>();
            _budgetService = budgetService;
            _sessionManager = sessionManager;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
            _dashboardService = dashboardService;
            hostingEnv = env;
            _settlementService = settlementService;
            _settlementItemService = settlementItemService;
        }

        [HttpGet]
        public async Task<IActionResult> ReadyToSettlementList()
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
        [HttpGet]
        public async Task<IActionResult> SettlementList()
        {
            try
            {
                var responseForNav = await _dashboardService.GetNavBarCount();
                var summaryCount = await _settlementService.LoadSettlementSummary();
                if (responseForNav.IsSuccessful)
                {
                    ViewData["SettlementSummary"] = new SettlementSummaryVM
                    {
                        Draft = summaryCount.Draft,
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
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetSettlementInitData()
        {
            var response = await _settlementService.GetSettlementInitData();
            return new JsonResult(response);
        }
        [HttpGet]
        public async Task<IActionResult> NewSettlementView(string id)
        {
            try
            {
                var value = Convert.ToInt32(_protector.Unprotect(id));
                var response = await _budgetService.GetOneEstimationWithType(value);
                var settlement = await _settlementService.getDraftSettlement(response.Estimation.EstimateId);
                if (settlement != null)
                settlement.EstimateSettlementAttachments = await _settlementService.LoadSettlementAttachmentsBySettlementId(settlement.Id);
                var responseForNav = await _dashboardService.GetNavBarCount();
                ViewBag.Settlement = settlement;

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
        public async Task<JsonResult> UploadFilesOfSettlement(int estimationSettlementId)
        {
            string result;
            try
            {
                long size = 0;
                var files = Request.Form.Files;
                string uploads = Path.Combine(hostingEnv.WebRootPath, $@"uploadedFiles\SettlementFiles\");
                Directory.CreateDirectory(uploads);
                foreach (var file in files)
                {
                    string FilePath = Path.Combine(uploads, file.FileName);

                    size += file.Length;
                    using (FileStream fs = System.IO.File.Create(FilePath))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                    var response = await _settlementService.SaveEstimateSettlementAttachment(new CreateAttachmentForSettlementRequest()
                    {
                        URL = FilePath,
                        FileName = file.FileName,
                        EstimationSettlement_Id = estimationSettlementId
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
        [HttpGet]
        public async Task<IActionResult> RollbackSettlementView(string id)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var settlementId = Convert.ToInt32(_protector.Unprotect(id));
                var settlement = await _settlementService.getSettlementBySettlementId(settlementId);
                if (settlement is null)
                    return RedirectToPage("/Error/401");
                if (settlement != null)
                    settlement.EstimateSettlementAttachments = await _settlementService.LoadSettlementAttachmentsBySettlementId(settlement.Id);
                if (settlement.CreatedBy != sessionUser.Id && settlement.Status != SettlementStatus.CR)
                {
                    return RedirectToPage("/Error/401");
                }

                var response = await _budgetService.GetOneEstimationWithType(settlement.EstimationId);

                var responseForNav = await _dashboardService.GetNavBarCount();
                ViewBag.Settlement = settlement;

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
        public async Task<IActionResult> SettlementAprroverView(string id)
        {
            try
            {
                var settlementId = Convert.ToInt32(_protector.Unprotect(id));
                var settlement = await _settlementService.getSettlementBySettlementId(settlementId);
                //var a = _settlementService.getSettlementHtmlBodyWithEstimationForPdfReport(settlementId);
                 if (settlement is null)
                    return RedirectToPage("/Error/401");
                var response = await _budgetService.GetOneEstimationWithType(settlement.EstimationId);
                settlement.EstimateSettlementAttachments = await _settlementService.LoadSettlementAttachmentsBySettlementId(settlementId);
                var responseForNav = await _dashboardService.GetNavBarCount();
                ViewBag.Settlement = settlement;
                

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
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SettlementApproval(int settlementId, string feedback, string remarks)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();

                var settlement = await _settlementService.getSettlementBySettlementId(settlementId);
                if (settlement == null)
                    _ = new Exception("Invalid Estimate");
                int intFeedback = 0;
                try
                {
                    intFeedback = Int32.Parse(feedback);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }


                if (settlement?.CurrentApprovalUserId != user.Id)
                {
                    return RedirectToPage("/Error/401");
                }

                await _settlementService.UpdateSettlementStatusAndRelatedData(new SettlementFeedback()
                {
                    SettlementId = settlement.Id, CurrentUserRolePiority = settlement.CurrentApprovalUserRolePiority,
                    UserId = settlement.CurrentApprovalUserId, Feedback = intFeedback, Remarks = remarks
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
        public async Task<JsonResult> DraftSettlement(string requestDto)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<SettlmentDTO>(requestDto);

                var result = await _settlementService.SaveSettlementData(jsonData, SettlementStatus.Draft);

                return Json(result);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
        [HttpPost]
        public async Task<JsonResult> SubmitSettlement(string requestDto)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<SettlmentDTO>(requestDto);

                var result = await _settlementService.SaveSettlementData(jsonData, SettlementStatus.Pending);

                return Json(result);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> SettlementSummaryViewByEstimateId(string id)
        {
            try
            {
                var value = Convert.ToInt32(_protector.Unprotect(id));
                var response = await _budgetService.GetOneEstimationWithType(value);
                var settlement = await _settlementService.getDraftSettlement(response.Estimation.EstimateId);
                var responseForNav = await _dashboardService.GetNavBarCount();
                ViewBag.Settlement = settlement;

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
        public async Task<IActionResult> GetAllApproverBySettlementId(int settlementId)
        {
            var response = await _settlementService.LoadSettlementApproverDetailsBySettlementId(settlementId);
            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetSettlementApproverRemarkList(int settlementId)
        {
            try
            {
                var response = await _settlementService.LoadSettlementApproverRemarks(settlementId);
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Json(0);
                throw;
            }
        }

        public async Task<IActionResult> LoadSettlementByStatus(int statusId, int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                List<SettlementVM> resultSet = await _settlementService.LoadSettlementByStatus(user.Id, statusId, start, PAGE_SIZE);
                int count = resultSet.Count > 0 ? resultSet[0].TotalRow : 0;
                long recordsTotal = count;
                long recordsFiltered = recordsTotal;

                var data = new List<object>();
                int sl = start + 1;
                CultureInfo cultures = new CultureInfo("en-US");

                foreach (var d in resultSet)
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

                    try
                    {
                        var approvalStatusList =
                            JsonConvert.DeserializeObject<List<SettlementApprovalStatus>>(d.SettlementApprovalList);
                        string approvalFlow = "";
                        foreach (var approval in approvalStatusList)
                        {

                            string buttonCSSClass = "";
                            string approvalStatus = "";
                            if (approval.RolePriority != 3)
                            {
                                if (approval.Status.ToLower() == nameof(BaseEntity.EntityStatus.Completed).ToLower())
                                {
                                    buttonCSSClass = "success";
                                    approvalStatus = "The Person Aprroved the Settlement";
                                }
                                else if (approval.Status.ToLower() == nameof(BaseEntity.EntityStatus.Reject).ToLower())
                                {
                                    buttonCSSClass = "danger";
                                    approvalStatus = "The Person Rejected the Settlement";
                                }
                                else if (approval.Status.ToLower() == "RollBack".ToLower())
                                {
                                    buttonCSSClass = "warning";
                                    approvalStatus = "The Person Rollback the Settlement";
                                }
                                else
                                {
                                    buttonCSSClass = "secondary";
                                    approvalStatus = "The Person Still waiting for Approval";
                                }

                                approvalFlow += getApprovalFlowButton(approval.Username, buttonCSSClass, approvalStatus);
                                if (approval.Priority != 1)
                                {
                                    approvalFlow += "<i class=\"fa fa-arrow-circle-right\"></i>";
                                }
                            }

                        }
                        str.Add(approvalFlow);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                    string actionTable = string.Empty, actionButtons = string.Empty;

                    if (statusId == SettlementStatus.CR)
                    {
                        actionButtons = "<a href='/Settlement/SettlementSummaryViewByEstimateId?id=" +
                                           _protector.Protect(d.EstimationId.ToString()) +
                                           "' class='text-decoration-none'>";
                        actionButtons +=
                            "<i class='fas fa-history fa-2x' data-toggle='tooltip' data-placement='left' title='Summary'></i> </a>";


                        if (d.SettlementInitiatorUserId == user.Id)
                        {
                            actionButtons += "<a href='/Settlement/RollbackSettlementView?id=" +
                                             _protector.Protect(d.SettlementId.ToString()) +
                                             "' class='text-decoration-none ml-3' >";
                            actionButtons +=
                                "<i class='fas fa-edit fa-2x' data-toggle='tooltip' data-placement='left' title='Re-Submit'></i>";
                            actionButtons += "</a>";
                        }
                        else
                        {
                            actionButtons += "<a href='/Settlement/SettlementAprroverView?id=" +
                                             _protector.Protect(d.SettlementId.ToString()) +
                                             "' class='text-decoration-none ml-3' >";
                            actionButtons +=
                                "<i class='fas fa-eye fa-2x' data-toggle='tooltip' data-placement='left' title='View'></i>";
                            actionButtons += "</a>";

                        }
                    }
                    else
                    {
                        actionButtons = "<a href='/Settlement/SettlementAprroverView?id=" +
                                           _protector.Protect(id.ToString()) +
                                           "' class='btn btn-outline-primary text-decoration-none'>";
                        actionButtons +=
                            "<i class='fa fa-eye' data-toggle='tooltip' data-placement='left' title='Acknowledge'></i>";
                        actionButtons += "View</a><br/> <br/>" +
                                         "<a href='/Settlement/SettlementSummaryViewByEstimateId?id=" +
                                         _protector.Protect(d.EstimationId.ToString()) +
                                         "' class='btn btn-outline-info text-decoration-none'>";
                        actionButtons +=
                            "<i class='fa fa-list-alt' data-toggle='tooltip' data-placement='left' title='Settlement Summary'></i> ";
                        actionButtons += "Summary</a>";
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
        public async Task<JsonResult> UploadFilesOfEstimation(int estimationId)
        {
            string result;
            try
            {
                long size = 0;
                var files = Request.Form.Files;
                string uploads = Path.Combine(hostingEnv.WebRootPath, $@"uploadedFiles\NewFolder\");
                foreach (var file in files)
                {
                    //var filename = ContentDispositionHeaderValue
                    //            .Parse(file.ContentDisposition)
                    //            .FileName
                    //            .Trim('"');

                    //string FilePath = hostingEnv.WebRootPath + $@"\uploadedFiles\NewFolder\{ filename}";
                    string FilePath = Path.Combine(uploads, file.FileName);

                    size += file.Length;
                    using (FileStream fs = System.IO.File.Create(FilePath))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }

                    var response = await _budgetService.CreateEstimateAttachment(new CreateAttachmentServiceRequest()
                    {
                        URL = FilePath,
                        FileName = file.FileName,
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
        [HttpGet]
        public async Task<IActionResult> GetAllApproverBySettlementIdForRollbackDraft(int settlementId)
        {
            var response = await _settlementService.GetAllApproverBySettlementIdForRollbackDraft(settlementId);
            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetInCompletedSettlementForCheckFinalSettlement(int estimationId)
        {
            var response = await _settlementService.getInCompletedSettlementForCheckFinalSettlement(estimationId);
            return new JsonResult(response);
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


        private string getApprovalFlowButton(string userName , string buttonClass, string approvalStatusMessage)
        {
            return $"<div style =\"padding: 5px;display: inline-block;\">" +
                   $"<button type =\"button\" class=\"btn btn-{buttonClass}\"  data-toggle=\"tooltip\" data-placement=\"top\" title=\"{approvalStatusMessage}\">" +
                   $"{userName}</ button > </div>";
        }


        [HttpGet]
        public async Task<IActionResult> LoadDepartmentWiseSummaryForASettledEstimationWithBudgetData(int estimationId)
        {
            try
            {
                var response = new List<DepartWiseSummaryDetailsForSettledEstimationIncludeTADA>();
                var dept = await _budgetService.LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(estimationId);
                var items = await _settlementItemService.getSettlementItemsByEstimateId(estimationId);
                foreach(var item in dept)
                {
                    if (item.DepartmentId == 7 || item.DepartmentId == 41)
                        continue;
                    var tadaAmount = items.Where(x => x.DepartmentId == item.DepartmentId && x.ItemCategory == "TA/DA").Sum(s => s.TotalPrice);
                    response.Add(new DepartWiseSummaryDetailsForSettledEstimationIncludeTADA
                    {
                        DepartmentId = item.DepartmentId,
                        DepartmentName = item.DepartmentName,
                        TotalCost = item.TotalCost,
                        TotalRunningCost = item.TotalRunningCost,
                        TotalBudget = item.TotalBudget,
                        TotalTADABudget = tadaAmount
                    });
                }
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetParticularSummaryForASettledEstimationWithBudgetData(int estimationId)
        {
            try
            {
                var response = new List<ParticularWiseSummaryDetailsForSettledEstimationIncludeTADA>();
                var particular = await _budgetService.LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(estimationId);
                var items = await _settlementItemService.getSettlementItemsByEstimateId(estimationId);
                foreach (var item in particular)
                {
                    var tadaAmount = items.Where(x => x.ParticularId == item.ParticularId && x.ItemCategory == "TA/DA" && x.DepartmentId !=7 && x.DepartmentId != 41).Sum(s => s.TotalPrice);
                    response.Add(new ParticularWiseSummaryDetailsForSettledEstimationIncludeTADA
                    {
                        ParticularId = item.ParticularId,
                        ParticularName = item.ParticularName,
                        TotalCost = item.TotalCost,
                        TotalRunningCost = item.TotalRunningCost,
                        TotalBudget = item.TotalBudget,
                        TotalTADABudget = tadaAmount
                    });
                }
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadDepartmentWiseSummaryForRunningSettlementBySettlementId(int settlementId, int estimationId)
        {
            try
            {
                var response = new List<DepartWiseSummaryDetailsForSettledEstimationIncludeTADA>();
                var dept = await _budgetService.LoadDeptSummeryForRunningSettlementBySettlementId(settlementId, estimationId);
                var items = await _settlementItemService.getSettlementItemsByEstimateId(estimationId);
                foreach (var item in dept)
                {
                    var tadaAmount = 0.0;
                    if (settlementId != 1022 && settlementId != 1023 && settlementId != 1027 && settlementId != 1029)
                    tadaAmount = items.Where(x => x.DepartmentId == item.DepartmentId && x.ItemCategory == "TA/DA").Sum(s => s.TotalPrice);
                    response.Add(new DepartWiseSummaryDetailsForSettledEstimationIncludeTADA
                    {
                        DepartmentId = item.DepartmentId,
                        DepartmentName = item.DepartmentName,
                        TotalCost = item.TotalCost,
                        TotalRunningCost = item.TotalRunningCost,
                        TotalBudget = item.TotalBudget,
                        TotalTADABudget = tadaAmount
                    });
                }
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadDeptSummeryForRunningSettlementBySettlementId(int settlementId, int estimationId)
        {
            try
            {
                var response = new List<DepartWiseSummaryDetailsForSettledEstimationIncludeTADA>();
                var dept = await _budgetService.LoadDeptSummeryForRunningSettlementBySettlementId(settlementId, estimationId);
                var items = await _settlementItemService.getSettlementItemsBySettlementId(settlementId);
                foreach (var item in dept)
                {
                    var tadaAmount = items.Where(x => x.DepartmentId == item.DepartmentId && x.ItemCategory == "TA/DA").Sum(s => s.TotalPrice);
                    response.Add(new DepartWiseSummaryDetailsForSettledEstimationIncludeTADA
                    {
                        DepartmentId = item.DepartmentId,
                        DepartmentName = item.DepartmentName,
                        TotalCost = item.TotalCost,
                        TotalRunningCost = item.TotalRunningCost,
                        TotalBudget = item.TotalBudget,
                        TotalTADABudget = tadaAmount
                    });
                }
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetParticularSummaryForRunningSettlementBySettlementId(int settlementId, int estimationId)
        {
            try
            {
                var response = new List<ParticularWiseSummaryDetailsForSettledEstimationIncludeTADA>();
                var particular = await _budgetService.LoadParticularWiseSummaryForRunningSettlementBySettlementId(settlementId, estimationId);
                var items = await _settlementItemService.getSettlementItemsByEstimateId(estimationId);
                foreach (var item in particular)
                {
                    var tadaAmount = items.Where(x => x.ParticularId == item.ParticularId && x.ItemCategory == "TA/DA" && x.DepartmentId != 7 && x.DepartmentId != 41).Sum(s => s.TotalPrice);
                    response.Add(new ParticularWiseSummaryDetailsForSettledEstimationIncludeTADA
                    {
                        ParticularId = item.ParticularId,
                        ParticularName = item.ParticularName,
                        TotalCost = item.TotalCost,
                        TotalRunningCost = item.TotalRunningCost,
                        TotalBudget = item.TotalBudget,
                        TotalTADABudget = tadaAmount
                    });
                }
                return Json(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadyToSettlementList(int draw = 0, int start = 0, int length = 0)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return RedirectToHome();
                int perPage = 50;
                List<ReadyForSettlementVM> resultSet = await _settlementService.ReadyToSettlementList(start, perPage);
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
                    str.Add(d.EstimationIdentity);
                    str.Add(d.Subject);

                    str.Add("<b> Start Date : </b> " + Convert.ToDateTime(d.PlanStartDate, cultures).ToString("d")
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

                    string actionButtons = "";

                    // Cig shohidul islam userid = 292 need permission for settlement create.
                    if ((user.Department_Id == 5 || user.Id == 292) && d.IsItAllowableForSettlement > 0)
                    {
                        actionButtons += " <a href='/Settlement/NewSettlementView?id=" +
                                            _protector.Protect(id.ToString()) +
                                            "' class=' btn btn-outline-info'>";

                        actionButtons +=
                        "<i class='fas fa-xs fa-file-invoice-dollar' data-toggle='tooltip' data-placement='left' title='new Settlement'>  &nbsp;New Settlement";
                        if (d.DraftExists > 0)
                            actionButtons += " <i class='badge badge-warning'> Draft </  > ";
                        actionButtons += " </i></a>";
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
        private string getConcateAmount(string totalPrice, string allowableBudget, string FundRequisitionAmount, string fundReceiveAmount, string remainingBudget)
        {
            return
                $"<b style=\"color: LightSeaGreen;\">Total Price:</b>{totalPrice} <br/>" +
                $"<b style=\"color: LightSeaGreen;\">Allowable Budget:</b>{allowableBudget} <br/>" +
                $" <b style=\"color: LightSeaGreen;\">Requisition Amount:</b> {FundRequisitionAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Received Amount:</b> {fundReceiveAmount} <br/>" +
                $"<b style=\"color: LightSeaGreen;\"> Remaining Budget:</b> {remainingBudget} <br/>";
        }
        [HttpGet]
        public async Task<JsonResult> DeleteAttachmentsById(int id)
        {
            try
            {
                var user = await _sessionManager.GetUser();
                if (user == null) return (JsonResult)RedirectToHome();

                var response = await _settlementService.DeleteAttachmentsById(id);
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
using AMS.Common.Helpers;
using AMS.Infrastructure.Email;
using AMS.Models.Enum;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ViewModel;
using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using ClosedXML.Excel;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AMS.Common.Constants;
using AMS.Services.Contracts;
using AMS.Services.Memo.Contracts;
using AMS.Services.SettlementService;
using AMS.Services.Managers.Contracts;
using AMS.Services.Managers;
using AMS.Models.CustomModels;
using AMS.Services.SettlementItem;
using System.Linq;

namespace AMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ReportGenaratorController : Controller
    {
        private readonly IBudgetService _budgetService;
        private readonly IConverter _converter;
        private readonly IUserService _userService;
        private readonly ISettlementService _settlementService;
        private readonly IHtmlGeneratorService _htmlGeneratorService;
        private readonly IMemoService _memoService;
        private readonly ISessionManager _sessionManager;
        private readonly ISettlementItemService _settlementItemService;


        public ReportGenaratorController(IBudgetService budgetService, IConverter converter, ISettlementItemService settlementItemService,
            IUserService userService, ISettlementService settlementService,
            IHtmlGeneratorService htmlGeneratorService, IMemoService memoService,ISessionManager sessionManager)
        {
            _budgetService = budgetService;
            _converter = converter;
            _userService = userService;
            _settlementService = settlementService;
            _htmlGeneratorService = htmlGeneratorService;
            _memoService = memoService;
            _settlementItemService = settlementItemService;
            _sessionManager = sessionManager;
        }

        [HttpGet]
        public async Task<EmptyResult> RunningEstimateReport()
        {
            try
            {
                var user = await _sessionManager.GetUser();
                List<EstimateEditVM> resultFromDB = await _budgetService.LoadAllPendingEstimate(user.Id, 0, 10);
                await this.BuildEstimate(resultFromDB, ReportFileName.Running.ToString());

                return new EmptyResult();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<EmptyResult> CompletedEstimateReport()
        {
            try
            {
                List<EstimateEditVM> resultFromDB = await _budgetService.LoadAllCompleteEstimate(0, 0, 10, "");
                await this.BuildEstimate(resultFromDB, ReportFileName.Completed.ToString());

                return new EmptyResult();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<EmptyResult> RejectEstimateReport()
        {
            try
            {
                List<EstimateEditVM> resultFromDB = await _budgetService.LoadRejectedEstimate(0, 0, 10);
                await this.BuildEstimate(resultFromDB, ReportFileName.Rejected.ToString());

                return new EmptyResult();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<EmptyResult> SingleEstimateReport(int id)
        {
            try
            {
                var estimate = await _budgetService.GetEstimationFullInfo(id);
                if (estimate == null)
                    throw new Exception("Invalid Estimate");

                var deptRunningSummary = new List<DepartWiseSummaryDetailsForSettledEstimationIncludeTADA>();
                var dept = await _budgetService.LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(id);
                var items = await _settlementItemService.getSettlementItemsByEstimateId(id);
                foreach (var item in dept)
                {
                    var tadaAmount = items.Where(x => x.DepartmentId == item.DepartmentId && x.ItemCategory == "TA/DA").Sum(s => s.TotalPrice);
                    deptRunningSummary.Add(new DepartWiseSummaryDetailsForSettledEstimationIncludeTADA
                    {
                        DepartmentId = item.DepartmentId,
                        DepartmentName = item.DepartmentName,
                        TotalCost = item.TotalCost,
                        TotalRunningCost = item.TotalRunningCost,
                        TotalBudget = item.TotalBudget,
                        TotalTADABudget = tadaAmount
                    });
                }
                estimate.DepartmentWiseRunningSummary = deptRunningSummary;

                var partRunningSummary = new List<ParticularWiseSummaryDetailsForSettledEstimationIncludeTADA>();
                var particular = await _budgetService.LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(id);
                foreach (var item in particular)
                {
                    var tadaAmount = items.Where(x => x.ParticularId == item.ParticularId && x.ItemCategory == "TA/DA" && x.DepartmentId != 7 && x.DepartmentId != 41).Sum(s => s.TotalPrice);
                    partRunningSummary.Add(new ParticularWiseSummaryDetailsForSettledEstimationIncludeTADA
                    {
                        ParticularId = item.ParticularId,
                        ParticularName = item.ParticularName,
                        TotalCost = item.TotalCost,
                        TotalRunningCost = item.TotalRunningCost,
                        TotalBudget = item.TotalBudget,
                        TotalTADABudget = tadaAmount
                    });
                }
                estimate.ParticularWiseRunningSummary = partRunningSummary;

                var fullestimateList = new List<AddBudgetEstimation>
                {
                    estimate
                };
                this.ExcelReport(fullestimateList, estimate.Estimation.UniqueIdentifier);


                return new EmptyResult();
            }
            catch (Exception)
            {

                throw;
            }
        }



        //Getting Full Estimate Information
        private async Task BuildEstimate(List<EstimateEditVM> resultFromDB, string fileName)
        {
            try
            {
                var fullestimateList = new List<AddBudgetEstimation>();
                foreach (var item in resultFromDB)
                {
                    var estimate = await _budgetService.GetEstimationFullInfo(item.Id);
                    fullestimateList.Add(estimate);
                }

                this.ExcelReport(fullestimateList, fileName);

            }
            catch (Exception)
            {

                throw;
            }
        }

        // Generating Excel Report 
        private void ExcelReport(IList<AddBudgetEstimation> requestObjList, string fileName)
        {
            using var workbook = new XLWorkbook();

            #region Esimate Basic Information 
            var worksheetSummary = workbook.Worksheets.Add("Summary");
            worksheetSummary.Columns().Width = 25;
            worksheetSummary.Column(3).Width = 35;
            worksheetSummary.Column(4).Width = 45;

            var currentRowForSummary = 1;
            worksheetSummary.Cell(currentRowForSummary, 1).Value = "Approval For";
            worksheetSummary.Cell(currentRowForSummary, 2).Value = "Identification";
            worksheetSummary.Cell(currentRowForSummary, 3).Value = "Subject";
            worksheetSummary.Cell(currentRowForSummary, 4).Value = "Objective";
            worksheetSummary.Cell(currentRowForSummary, 5).Value = "CurrencyType";
            worksheetSummary.Cell(currentRowForSummary, 6).Value = "Total Price";
            worksheetSummary.Cell(currentRowForSummary, 7).Value = "Price Remarks";
            worksheetSummary.Cell(currentRowForSummary, 8).Value = "Plan Start Date";
            worksheetSummary.Cell(currentRowForSummary, 9).Value = "Plan End Date";
            worksheetSummary.Cell(currentRowForSummary, 10).Value = "Created By";
            worksheetSummary.Cell(currentRowForSummary, 11).Value = "Creation Time";
            if (requestObjList[0].Estimation.EstimateType == EstimationType.Factsheet ||
                requestObjList[0].Estimation.EstimateType == EstimationType.PROCUREMENT)
            {
                worksheetSummary.Cell(currentRowForSummary, 12).Value = "PA. Reference No";
                worksheetSummary.Cell(currentRowForSummary, 13).Value = "Title of the PR/RFQ";
                worksheetSummary.Cell(currentRowForSummary, 14).Value = "RFQ Reference No.";
                worksheetSummary.Cell(currentRowForSummary, 15).Value = "PR Reference No.";
                worksheetSummary.Cell(currentRowForSummary, 16).Value = "Name and Cell No. of Requester";
                worksheetSummary.Cell(currentRowForSummary, 17).Value = "Department";
                worksheetSummary.Cell(currentRowForSummary, 18).Value = "RFQ Process";
                worksheetSummary.Cell(currentRowForSummary, 19).Value = "Sourcing Method";
                worksheetSummary.Cell(currentRowForSummary, 20).Value = "Supplier Name";
                worksheetSummary.Cell(currentRowForSummary, 21).Value = "Purchase Value";
                worksheetSummary.Cell(currentRowForSummary, 22).Value = "Saving Amount";
                worksheetSummary.Cell(currentRowForSummary, 23).Value = "Saving Type";
            }

            worksheetSummary.Row(1).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheetSummary.Row(1).Style.Font.Bold = true;

            foreach (var item in requestObjList)
            {
                currentRowForSummary++;

                worksheetSummary.Cell(currentRowForSummary, 1).Value = item.Estimation.EstimateTypeName;
                worksheetSummary.Cell(currentRowForSummary, 2).Value = item.Estimation.UniqueIdentifier;
                worksheetSummary.Cell(currentRowForSummary, 3).Value = item.Estimation.Subject;
                worksheetSummary.Cell(currentRowForSummary, 4).Value = item.Estimation.Objective;
                worksheetSummary.Cell(currentRowForSummary, 5).Value = Utility.CurrencyTypeConvertToStringFormat(item.Estimation.CurrencyType);
                worksheetSummary.Cell(currentRowForSummary, 6).Value = item.Estimation.TotalPrice;
                worksheetSummary.Cell(currentRowForSummary, 7).Value = item.Estimation.TotalPriceRemarks;
                worksheetSummary.Cell(currentRowForSummary, 8).Value = item.Estimation.PlanStartDate;
                worksheetSummary.Cell(currentRowForSummary, 9).Value = item.Estimation.PlanEndDate;
                worksheetSummary.Cell(currentRowForSummary, 10).Value = item.Estimation.CreatedByName;
                worksheetSummary.Cell(currentRowForSummary, 11).Value = item.Estimation.CreationDate.ToString();
                if (requestObjList[0].Estimation.EstimateType == EstimationType.Factsheet ||
                    requestObjList[0].Estimation.EstimateType == EstimationType.PROCUREMENT)
                {
                    worksheetSummary.Cell(currentRowForSummary, 12).Value = item.ProcurementApprovalRequest.PAReferenceNo;
                    worksheetSummary.Cell(currentRowForSummary, 13).Value = item.ProcurementApprovalRequest.TitleOfPRorRFQ;
                    worksheetSummary.Cell(currentRowForSummary, 14).Value = item.ProcurementApprovalRequest.RFQReferenceNo;
                    worksheetSummary.Cell(currentRowForSummary, 15).Value = item.ProcurementApprovalRequest.PRReferenceNo;
                    worksheetSummary.Cell(currentRowForSummary, 16).Value = item.ProcurementApprovalRequest.NameOfRequester;
                    worksheetSummary.Cell(currentRowForSummary, 17).Value = item.ProcurementApprovalRequest.DepartmentName;
                    worksheetSummary.Cell(currentRowForSummary, 18).Value = item.ProcurementApprovalRequest.RFQProcess;
                    worksheetSummary.Cell(currentRowForSummary, 19).Value = item.ProcurementApprovalRequest.SourcingMethod;
                    worksheetSummary.Cell(currentRowForSummary, 20).Value = item.ProcurementApprovalRequest.NameOfRecommendedSupplier;
                    worksheetSummary.Cell(currentRowForSummary, 21).Value = item.ProcurementApprovalRequest.PurchaseValue;
                    worksheetSummary.Cell(currentRowForSummary, 22).Value = item.ProcurementApprovalRequest.SavingAmount;
                    worksheetSummary.Cell(currentRowForSummary, 23).Value = item.ProcurementApprovalRequest.SavingType;
                }
            }

            #endregion

            #region Estimate Approver 
            var worksheetApprover = workbook.Worksheets.Add("Approver List");
            worksheetApprover.Columns(2, 6).Width = 25;
            var currentRowForApprover = 1;

            foreach (var estimate in requestObjList)
            {
                worksheetApprover.Cell(currentRowForApprover, 1).Value = "#";
                worksheetApprover.Cell(currentRowForApprover, 2).Value = "Identification";
                worksheetApprover.Cell(currentRowForApprover, 3).Value = "Approver Name";
                worksheetApprover.Cell(currentRowForApprover, 4).Value = "Approver Type";
                worksheetApprover.Cell(currentRowForApprover, 5).Value = "Approver Department";
                worksheetApprover.Cell(currentRowForApprover, 6).Value = "Expected Time";

                worksheetApprover.Row(currentRowForApprover).Style.Fill.BackgroundColor = XLColor.Gray;
                worksheetApprover.Row(currentRowForApprover).Style.Font.Bold = true;

                var serial = 0;
                foreach (var item in estimate.EstimateApproverList)
                {
                    currentRowForApprover++;
                    serial++;
                    worksheetApprover.Cell(currentRowForApprover, 1).Value = serial;
                    worksheetApprover.Cell(currentRowForApprover, 2).Value = estimate.Estimation.UniqueIdentifier;
                    worksheetApprover.Cell(currentRowForApprover, 3).Value = item.ApproverFullName;
                    worksheetApprover.Cell(currentRowForApprover, 4).Value = item.ApproverRole;
                    worksheetApprover.Cell(currentRowForApprover, 5).Value = item.ApproverDepartment;
                    worksheetApprover.Cell(currentRowForApprover, 6).Value = item.ExpectedTime;
                }

                worksheetApprover.Row(currentRowForApprover).InsertRowsBelow(1);
                currentRowForApprover += 2;
            }

            #endregion

            #region Estimate Item 
            var worksheetitem = workbook.Worksheets.Add("Particular Items");
            worksheetitem.Columns().Width = 5;
            worksheetitem.Columns(2, 4).Width = 30;
            worksheetitem.Columns(5, 12).Width = 15;
            worksheetitem.Columns(13, 17).Width = 30;
            var currentRowForitem = 1;

            foreach (var estimate in requestObjList)
            {
                #region Heading
                worksheetitem.Cell(currentRowForitem, 1).Value = "#";
                worksheetitem.Cell(currentRowForitem, 2).Value = "Identification";
                worksheetitem.Cell(currentRowForitem, 3).Value = "Particular";
                worksheetitem.Cell(currentRowForitem, 4).Value = "Item Category";
                worksheetitem.Cell(currentRowForitem, 5).Value = "Item";
                worksheetitem.Cell(currentRowForitem, 6).Value = "Item Code";
                worksheetitem.Cell(currentRowForitem, 7).Value = "Unit";
                worksheetitem.Cell(currentRowForitem, 8).Value = "No. Of Machine /Usages /Team /Car";
                worksheetitem.Cell(currentRowForitem, 9).Value = "No. Of Day /Total Unit";
                worksheetitem.Cell(currentRowForitem, 10).Value = "Required Quantity";
                worksheetitem.Cell(currentRowForitem, 11).Value = "Unit Price";
                worksheetitem.Cell(currentRowForitem, 12).Value = "Total Price";
                worksheetitem.Cell(currentRowForitem, 13).Value = "Department";
                worksheetitem.Cell(currentRowForitem, 14).Value = "District";
                worksheetitem.Cell(currentRowForitem, 15).Value = "Thana";
                worksheetitem.Cell(currentRowForitem, 16).Value = "Area type";
                worksheetitem.Cell(currentRowForitem, 17).Value = "Remarks";

                worksheetitem.Row(currentRowForitem).Style.Fill.BackgroundColor = XLColor.Gray;
                worksheetitem.Row(currentRowForitem).Style.Font.Bold = true;

                #endregion

                #region For No Item
                if (estimate.EstimateDetails == null)
                {
                    currentRowForitem++;
                    worksheetitem.Cell(currentRowForitem, 1).Value = 1;
                    worksheetitem.Cell(currentRowForitem, 2).Value = estimate.Estimation.UniqueIdentifier;
                    worksheetitem.Cell(currentRowForitem, 3).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 4).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 5).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 6).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 7).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 8).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 9).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 10).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 11).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 12).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 13).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 14).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 15).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 16).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 17).Value = "N/A";

                    worksheetitem.Row(currentRowForitem).InsertRowsBelow(1);
                    currentRowForitem += 2;

                    continue;
                }
                #endregion
                var serial = 0;
                foreach (var item in estimate.EstimateDetails)
                {
                    currentRowForitem++;
                    serial++;
                    worksheetitem.Cell(currentRowForitem, 1).Value = serial;
                    worksheetitem.Cell(currentRowForitem, 2).Value = estimate.Estimation.UniqueIdentifier;
                    worksheetitem.Cell(currentRowForitem, 3).Value = item.Particular;
                    worksheetitem.Cell(currentRowForitem, 4).Value = item.ItemCategory;
                    worksheetitem.Cell(currentRowForitem, 5).Value = item.ItemName;
                    worksheetitem.Cell(currentRowForitem, 6).Value = item.ItemCode;
                    worksheetitem.Cell(currentRowForitem, 7).Value = item.ItemUnit;
                    worksheetitem.Cell(currentRowForitem, 8).Value = item.NoOfMachineAndUsagesAndTeamAndCar;
                    worksheetitem.Cell(currentRowForitem, 9).Value = item.NoOfDayAndTotalUnit;
                    worksheetitem.Cell(currentRowForitem, 10).Value = item.QuantityRequired;
                    worksheetitem.Cell(currentRowForitem, 11).Value = item.UnitPrice;
                    worksheetitem.Cell(currentRowForitem, 12).Value = item.TotalPrice;
                    worksheetitem.Cell(currentRowForitem, 13).Value = item.ResponsibleDepartmentName;
                    worksheetitem.Cell(currentRowForitem, 14).Value = item.DistrictName;
                    worksheetitem.Cell(currentRowForitem, 15).Value = item.ThanaName;
                    worksheetitem.Cell(currentRowForitem, 16).Value = item.AreaType;
                    worksheetitem.Cell(currentRowForitem, 17).Value = item.Remarks;
                }
                currentRowForitem++;
                worksheetitem.Cell(currentRowForitem, 12).Value = estimate.Estimation.TotalPrice;

                worksheetitem.Row(currentRowForitem).InsertRowsBelow(1);
                currentRowForitem += 2;
            }

            #endregion
            #region Estimate Summaray By Department and Particular 
            var worksheetdeptparti = workbook.Worksheets.Add("Budget Summaries");
            worksheetdeptparti.Columns().Width = 5;
            worksheetdeptparti.Columns(2, 4).Width = 20;
            worksheetdeptparti.Columns(7, 9).Width = 20;
            worksheetdeptparti.Columns(2, 5).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheetdeptparti.Columns(7, 10).Style.Fill.BackgroundColor = XLColor.BlueGray;
            var currentRowFordeptParti = 1;

            foreach (var estimate in requestObjList)
            {
                if (estimate.ParticularWiseSummary != null || estimate.DepartmentWiseSummary != null)
                {
                    worksheetdeptparti.Cell(currentRowFordeptParti, 2).Value = "Identification";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 3).Value = "Department Wise Summmary";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 4).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 7).Value = "Identification";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 8).Value = "Particular Wise Summary";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 9).Value = "";

                    worksheetdeptparti.Row(currentRowFordeptParti).Style.Font.Bold = true;

                    currentRowFordeptParti++;
                    worksheetdeptparti.Cell(currentRowFordeptParti, 2).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 3).Value = "Department";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 4).Value = "Total Price";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 7).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 8).Value = "Particular";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 9).Value = "Total Price";

                    var lineForDept = currentRowFordeptParti;
                    foreach (var item in estimate.DepartmentWiseSummary)
                    {
                        lineForDept++;
                        worksheetdeptparti.Cell(lineForDept, 2).Value = estimate.Estimation.UniqueIdentifier;
                        worksheetdeptparti.Cell(lineForDept, 3).Value = item.DepartmentName;
                        worksheetdeptparti.Cell(lineForDept, 4).Value = item.TotalPrice;
                    }
                    lineForDept++;
                    worksheetdeptparti.Cell(lineForDept, 2).Value = "Grand Total";
                    worksheetdeptparti.Cell(lineForDept, 4).Value = estimate.Estimation.TotalPrice;

                    var lineForparti = currentRowFordeptParti;
                    foreach (var item in estimate.ParticularWiseSummary)
                    {
                        lineForparti++;
                        worksheetdeptparti.Cell(lineForparti, 7).Value = estimate.Estimation.UniqueIdentifier;
                        worksheetdeptparti.Cell(lineForparti, 8).Value = item.ParticlarName;
                        worksheetdeptparti.Cell(lineForparti, 9).Value = item.TotalPrice;
                    }
                    lineForparti++;
                    worksheetdeptparti.Cell(lineForparti, 7).Value = "Grand Total";
                    worksheetdeptparti.Cell(lineForparti, 9).Value = estimate.Estimation.TotalPrice;

                    currentRowFordeptParti = (lineForDept >= lineForparti) ? lineForDept : lineForparti;

                    worksheetdeptparti.Row(currentRowFordeptParti).InsertRowsBelow(1);
                    worksheetdeptparti.Row(currentRowFordeptParti + 1).Style.Fill.BackgroundColor = XLColor.White;
                    currentRowFordeptParti += 2;
                }
                else
                {
                    worksheetdeptparti.Cell(currentRowFordeptParti, 2).Value = "Identification";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 3).Value = "Department Wise Summmary";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 4).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 7).Value = "Identification";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 8).Value = "Particular Wise Summary";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 9).Value = "";

                    worksheetdeptparti.Row(currentRowFordeptParti).Style.Font.Bold = true;

                    currentRowFordeptParti++;
                    worksheetdeptparti.Cell(currentRowFordeptParti, 2).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 3).Value = "Department";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 4).Value = "Total Price";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 7).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 8).Value = "Particular";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 9).Value = "Total Price";

                    currentRowFordeptParti++;
                    worksheetdeptparti.Cell(currentRowFordeptParti, 2).Value = estimate.Estimation.UniqueIdentifier;
                    worksheetdeptparti.Cell(currentRowFordeptParti, 3).Value = "N/A";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 4).Value = "N/A";

                    worksheetdeptparti.Cell(currentRowFordeptParti, 7).Value = estimate.Estimation.UniqueIdentifier;
                    worksheetdeptparti.Cell(currentRowFordeptParti, 8).Value = "N/A";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 9).Value = "N/A";

                    worksheetdeptparti.Row(currentRowFordeptParti).InsertRowsBelow(1);
                    worksheetdeptparti.Row(currentRowFordeptParti + 1).Style.Fill.BackgroundColor = XLColor.White;
                    currentRowFordeptParti += 2;
                }
            }

            #endregion


            #region Estimate Running Summaray By Department and Particular 
            var worksheetdeptRunningparti = workbook.Worksheets.Add("Department Running Summmary");
            var currentRowFordeptRunningParti = 0;

            foreach (var estimate in requestObjList)
            {
                currentRowFordeptRunningParti++;
                worksheetdeptRunningparti.Cell(currentRowFordeptRunningParti, 2).Value = "Department";
                worksheetdeptRunningparti.Cell(currentRowFordeptRunningParti, 3).Value = "Total Budget (TK.)";
                worksheetdeptRunningparti.Cell(currentRowFordeptRunningParti, 4).Value = "Total TA/DA Budget (TK.)";
                worksheetdeptRunningparti.Cell(currentRowFordeptRunningParti, 5).Value = "Total Allowed Budget (TK.)";
                worksheetdeptRunningparti.Cell(currentRowFordeptRunningParti, 6).Value = "Total Cost (TK.)";
                worksheetdeptRunningparti.Cell(currentRowFordeptRunningParti, 7).Value = "Deviation";
                worksheetdeptRunningparti.Cell(currentRowFordeptRunningParti, 8).Value = "Percentage";
                worksheetdeptRunningparti.Row(currentRowFordeptRunningParti).Style.Font.Bold = true;

                var tTotalBudget = 0.0;
                var tTotalTADABudget = 0.0;
                var tTotalAllowableBudget = 0.0;
                var tTotalCost = 0.0;
                var tTotalDeviation = 0.0;
                var tTotalParcentage = 0.0;
                var lineForDept = currentRowFordeptRunningParti;
                foreach (var item in estimate.DepartmentWiseRunningSummary)
                {
                    lineForDept++;
                    worksheetdeptRunningparti.Cell(lineForDept, 2).Value = item.DepartmentName;
                    worksheetdeptRunningparti.Cell(lineForDept, 3).Value = item.TotalBudget;
                    worksheetdeptRunningparti.Cell(lineForDept, 4).Value = item.TotalTADABudget;
                    worksheetdeptRunningparti.Cell(lineForDept, 5).Value = item.TotalAllowableBudget;
                    worksheetdeptRunningparti.Cell(lineForDept, 6).Value = item.TotalCost;
                    worksheetdeptRunningparti.Cell(lineForDept, 7).Value = item.Deviation;
                    worksheetdeptRunningparti.Cell(lineForDept, 8).Value = item.Parcentage.ToString("N2");
                    tTotalBudget += item.TotalBudget;
                    tTotalTADABudget += item.TotalTADABudget;
                    tTotalAllowableBudget += item.TotalAllowableBudget;
                    tTotalCost += item.TotalCost;
                    tTotalDeviation += item.Deviation;
                    tTotalParcentage += item.Parcentage;
                }
                lineForDept++;
                worksheetdeptRunningparti.Cell(lineForDept, 2).Value = "Grand Total(Exclued SCM & Regularity Affairs)";
                worksheetdeptRunningparti.Cell(lineForDept, 3).Value = tTotalBudget;
                worksheetdeptRunningparti.Cell(lineForDept, 4).Value = tTotalTADABudget;
                worksheetdeptRunningparti.Cell(lineForDept, 5).Value = tTotalAllowableBudget;
                worksheetdeptRunningparti.Cell(lineForDept, 6).Value = tTotalCost;
                worksheetdeptRunningparti.Cell(lineForDept, 7).Value = tTotalDeviation;
                worksheetdeptRunningparti.Cell(lineForDept, 8).Value = tTotalParcentage.ToString("N2");
                worksheetdeptRunningparti.Row(lineForDept).Style.Font.Bold = true;
            }
            #endregion
            #region Department Running Summaray By Department and Particular 
            var worksheetpartiRunningparti = workbook.Worksheets.Add("Particular Running Summmary");
            var currentRowForPartRunningParti = 0;

            foreach (var estimate in requestObjList)
            {
                currentRowForPartRunningParti++;
                worksheetpartiRunningparti.Cell(currentRowForPartRunningParti, 2).Value = "Particular";
                worksheetpartiRunningparti.Cell(currentRowForPartRunningParti, 3).Value = "Total Budget (TK.)";
                worksheetpartiRunningparti.Cell(currentRowForPartRunningParti, 4).Value = "Total TA/DA Budget (TK.)";
                worksheetpartiRunningparti.Cell(currentRowForPartRunningParti, 5).Value = "Total Allowed Budget (TK.)";
                worksheetpartiRunningparti.Cell(currentRowForPartRunningParti, 6).Value = "Total Cost (TK.)";
                worksheetpartiRunningparti.Cell(currentRowForPartRunningParti, 7).Value = "Deviation";
                worksheetpartiRunningparti.Cell(currentRowForPartRunningParti, 8).Value = "Percentage";
                worksheetpartiRunningparti.Row(currentRowForPartRunningParti).Style.Font.Bold = true;

                var pTotalBudget = 0.0;
                var pTotalTADABudget = 0.0;
                var pTotalAllowableBudget = 0.0;
                var pTotalCost = 0.0;
                var pTotalDeviation = 0.0;
                var pTotalParcentage = 0.0;
                var lineForParti = currentRowForPartRunningParti;
                foreach (var item in estimate.ParticularWiseRunningSummary)
                {
                    lineForParti++;
                    worksheetpartiRunningparti.Cell(lineForParti, 2).Value = item.ParticularName;
                    worksheetpartiRunningparti.Cell(lineForParti, 3).Value = item.TotalBudget;
                    worksheetpartiRunningparti.Cell(lineForParti, 4).Value = item.TotalTADABudget;
                    worksheetpartiRunningparti.Cell(lineForParti, 5).Value = item.TotalAllowableBudget;
                    worksheetpartiRunningparti.Cell(lineForParti, 6).Value = item.TotalCost;
                    worksheetpartiRunningparti.Cell(lineForParti, 7).Value = item.Deviation;
                    worksheetpartiRunningparti.Cell(lineForParti, 8).Value = item.Parcentage.ToString("N2");
                    pTotalBudget += item.TotalBudget;
                    pTotalTADABudget += item.TotalTADABudget;
                    pTotalAllowableBudget += item.TotalAllowableBudget;
                    pTotalCost += item.TotalCost;
                    pTotalDeviation += item.Deviation;
                    pTotalParcentage += item.Parcentage;
                }
                lineForParti++;
                worksheetpartiRunningparti.Cell(lineForParti, 2).Value = "Grand Total(Exclued SCM & Regularity Affairs)";
                worksheetpartiRunningparti.Cell(lineForParti, 3).Value = pTotalBudget;
                worksheetpartiRunningparti.Cell(lineForParti, 4).Value = pTotalTADABudget;
                worksheetpartiRunningparti.Cell(lineForParti, 5).Value = pTotalAllowableBudget;
                worksheetpartiRunningparti.Cell(lineForParti, 6).Value = pTotalCost;
                worksheetpartiRunningparti.Cell(lineForParti, 7).Value = pTotalDeviation;
                worksheetpartiRunningparti.Cell(lineForParti, 8).Value = pTotalParcentage.ToString("N2");
                worksheetpartiRunningparti.Row(lineForParti).Style.Font.Bold = true;
            }
            #endregion
            #region Estimate Approver 
            var worksheetfeed = workbook.Worksheets.Add("Approvers FeedBacks");
            worksheetfeed.Columns(2, 6).Width = 20;
            var currentRowForfeed = 1;

            foreach (var estimate in requestObjList)
            {
                if (estimate.EstimateApproverFeedBacks != null)
                {
                    worksheetfeed.Cell(currentRowForfeed, 1).Value = "#";
                    worksheetfeed.Cell(currentRowForfeed, 2).Value = "Identification";
                    worksheetfeed.Cell(currentRowForfeed, 3).Value = "Approver Name";
                    worksheetfeed.Cell(currentRowForfeed, 4).Value = "Status";
                    worksheetfeed.Cell(currentRowForfeed, 5).Value = "Feedback Date";
                    worksheetfeed.Cell(currentRowForfeed, 6).Value = "Remarks";

                    worksheetfeed.Row(currentRowForfeed).Style.Fill.BackgroundColor = XLColor.Gray;
                    worksheetfeed.Row(currentRowForfeed).Style.Font.Bold = true;

                    var serial = 0;
                    foreach (var item in estimate.EstimateApproverFeedBacks)
                    {
                        currentRowForfeed++;
                        serial++;
                        worksheetfeed.Cell(currentRowForfeed, 1).Value = serial;
                        worksheetfeed.Cell(currentRowForfeed, 2).Value = estimate.Estimation.UniqueIdentifier;
                        worksheetfeed.Cell(currentRowForfeed, 3).Value = item.ApproverFullName;
                        worksheetfeed.Cell(currentRowForfeed, 4).Value = item.EstimateApproverStatusString;
                        worksheetfeed.Cell(currentRowForfeed, 5).Value = item.FeedBackDate;
                        worksheetfeed.Cell(currentRowForfeed, 6).Value = item.FeedBack;
                    }

                    worksheetfeed.Row(currentRowForfeed).InsertRowsBelow(1);
                    currentRowForfeed += 2;
                }
            }

            #endregion
            #region estimateDetails
            //var estimateDetailsWorksheet = workbook.Worksheets.Add("Estimate_Details");

            //estimateDetailsWorksheet.Columns().Width = 65;
            //var currentRowForDetails = 1;
            //estimateDetailsWorksheet.Cell(currentRowForDetails, 1).Value = "Estimate Details";
            //foreach (var item in requestObjList)
            //{
            //    currentRowForDetails++;
            //    var esDetails = item.Estimation.Details;
            //    estimateDetailsWorksheet.Cell(currentRowForDetails, 1).Value = esDetails;
            //}
            #endregion

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            Response.Clear();
            Response.Headers.Add("content-disposition", "attachment;filename=" + fileName + "_Budget_Esimate_Report_" + DateTime.Now.ToString("MM/dd/yyyy") + ".xlsx");
            Response.ContentType = "application/xlsx";
            Response.Body.WriteAsync(content);
            Response.Body.Flush();
        }


        public async Task<ActionResult> SinglePdfEstimateReport(int id)
        {
            try
            {
                var estimate = await _budgetService.GetEstimationFullInfo(id);
                if (estimate == null)
                    throw new Exception("Invalid Estimate");
                var fullestimateList = new List<AddBudgetEstimation>
                {
                    estimate
                };


                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "Estimate Pdf Report"
                    //Out = @"wwwroot\pdf\" + fileName + ".pdf"
                };

                var objectSettings = new ObjectSettings
                {

                    PagesCount = true,
                    HtmlContent = await GetStringHtmlForPdf(fullestimateList[0], ""),
                    WebSettings = { DefaultEncoding = "utf-8", LoadImages = true },

                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Approval Managment System" }

                };
                var pdf = new HtmlToPdfDocument
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }

                };
                var file = _converter.Convert(pdf);
                return File(file, "application/pdf", estimate.Estimation.UniqueIdentifier + ".pdf");





            }
            catch (Exception)
            {

                throw;
            }
        }


        private async Task<string> GetStringHtmlForPdf(AddBudgetEstimation requestBody, string approverUser)
        {
            try
            {
                #region Parameter
                string body = "";
                string procurementApprovalPart = "";
                string itemDetails = "";
                string itemDetailsSingleRow = "";
                string itemDetailsTable = "";
                string itemApprovers = "";
                string itemApproversFeedback = "";
                string itemApproversFeedbackInSingleRow = "";
                string itemApproversInSingleRow = "";
                string deptSummaryInSingleRow = "";
                string deptSummary = "";
                string partiSummaryInSingleRow = "";
                string partiSummary = "";
                #endregion

                if (requestBody.Estimation.EstimateType == EstimationType.PROCUREMENT || requestBody.Estimation.EstimateType == EstimationType.Factsheet)
                {
                    #region ProcurementApproval
                    procurementApprovalPart =
                        @"<tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>PA. Reference No.</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.PAReferenceNo + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Title of the PR/RFQ</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.TitleOfPRorRFQ + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>RFQ Reference No.</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.RFQReferenceNo + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>PR Reference No.</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.PRReferenceNo + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Name and Cell No. of Requester</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.NameOfRequester + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Department/Division</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.DepartmentName + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>RFQ Process</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.RFQProcess + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Sourcing Method</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.SourcingMethod + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Name of the Recommended Supplied</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.NameOfRecommendedSupplier + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Purchase Value</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.PurchaseValue + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Saving Amount</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.SavingAmount + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Saving Type</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + requestBody.ProcurementApprovalRequest.SavingType + @"</td>
                    </tr>";
                    #endregion
                }

                //adding itemDetailsBodyPart on EmailBody
                if (Utility.IsEmpty(requestBody.EstimateDetails))
                {
                    itemDetailsTable = "";
                }
                else
                {
                    #region BuildingItemDetailsHTML
                    foreach (var item in requestBody.EstimateDetails)
                    {
                        itemDetailsSingleRow =
                        @"<tr>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.Particular +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.ItemCategory +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.ItemName +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.ItemCode +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.ItemUnit +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.NoOfMachineAndUsagesAndTeamAndCar +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.NoOfDayAndTotalUnit +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.QuantityRequired +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.UnitPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")) +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")) +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.ResponsibleDepartmentName +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.DistrictName +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.ThanaName +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.AreaType +
                            @"</td>
                            <td style='padding:5px; width:10%; background-color:#ededed;font-family:Calibri; font-size:small;' align='left' valign='top'>"
                                + item.Remarks +
                            @"</td>
                        </tr>";

                        itemDetails = itemDetails + itemDetailsSingleRow;
                    }
                    itemDetailsTable =
                        @"<tr>
                            <td style='width:100%;padding:5px'>
                                <table align='center' border='0' cellspacing='0' cellpadding='0' style='width:100%'>
                                    <tbody>
                                    <tr>
                                        <td style='background-color:rgba(0,0,0,0);'><br><br></td>
                                    </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                        <tr>        
                            <td style='padding:5px; background-color:#ededed; width:100%;font-family:Calibri; font-size:100%;' align='center'>
                                <b>Particular Items and Details</b>
                                <hr>
                            </td>
                        </tr>
                        <tr>
                            <td style='padding-bottom:30px 0px 0px 0px; width: 100%; background-color:#ededed;'>
                                <!--Estimation's Item Details Table-->
                                <table align='left' border='0' cellspacing='0' cellpadding='0' style='width:100%;'>
                                    <!--HEADER-->
                                    <thead>
                                    <tr>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Particular</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Item Category</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Item</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Item Code</u></b>
                                        </td>
                                        <td style='padding:5px; width:5%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Unit</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b>Machine /Usages <br />/Team <br />/Car<br /><u>Number</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>No. Of Day /Total Unit</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Required Quantity</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Unit Price</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Total Price (TK.)</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Responsible Department</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>District</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Thana</u></b>
                                        </td>
                                        <td style='padding:5px; width:10%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Area Type</u></b>
                                        </td>
                                        <td style='padding:5px; width:20%; background-color:#e1e6de;font-family:Calibri; font-size:small;' align='left' valign='bottom'>
                                            <b><u>Remarks</u></b>
                                        </td>
                                    </tr>
                                    </thead>
                                    <tbody>
                                    " + itemDetails +
                                    @"</tbody>
                                    <tfoot>
                                        <tr>
                                            <td align='right' colspan='9' style='padding: 0 10px 0 0;'>Grand Total</td>
                                            <td colspan='6'><strong>" + requestBody.Estimation.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")) + @"</strong></td>
                                    </tr></tfoot>
                                </table>
                            </td>
                        </tr>";
                    #endregion
                }

                //adding departmentSummaryPart in EmailBody
                #region BuildingDeptSummary 
                if (Utility.IsEmpty(requestBody.DepartmentWiseSummary))
                {
                    deptSummary = "";
                }
                else
                {
                    foreach (var item in requestBody.DepartmentWiseSummary)
                    {
                        deptSummaryInSingleRow = @"<tr align='center'><td>"
                            + item.DepartmentName
                            + "</td><td>"
                            + item.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))
                            + "</td></tr>";
                        deptSummary = deptSummary + deptSummaryInSingleRow;
                    }
                }

                #endregion

                //adding particularSummaryPart in EmailBody
                #region BuildingPartiSummary 
                if (Utility.IsEmpty(requestBody.ParticularWiseSummary))
                {
                    partiSummary = "";
                }
                else
                {
                    foreach (var item in requestBody.ParticularWiseSummary)
                    {
                        partiSummaryInSingleRow = @"<tr align='center'><td>"
                            + item.ParticlarName
                            + "</td><td>"
                            + item.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))
                            + "</td></tr>";
                        partiSummary = partiSummary + partiSummaryInSingleRow;
                    }
                }
                #endregion

                ///adding approverListPart in EmailBody
                #region BuildingApproverListHTML
                var userWithDeptInfo = await _userService.GetUserAndDepartmentByIdService(requestBody.Estimation.CreatedBy);

                itemApprovers = @"<tr align='center'><td>" + userWithDeptInfo.First_Name + " " + userWithDeptInfo.Last_Name + "</td><td>Creator</td><td>" + userWithDeptInfo.DepartmentName + "</td><td></td></tr>";
                foreach (var item in requestBody.EstimateApproverList)
                {
                    itemApproversInSingleRow = @"<tr align='center'><td>" + item.ApproverFullName + "</td><td>" + item.ApproverRole + "</td><td>" + item.ApproverDepartment + "</td><td>" + item.ExpectedTime + "</td></tr>";
                    itemApprovers = itemApprovers + itemApproversInSingleRow;
                }
                #endregion
                #region ApproversFeedBack 
                if (Utility.IsEmpty(requestBody.EstimateApproverFeedBacks))
                {
                    itemApproversFeedback = "";
                }
                else
                {
                    foreach (var item in requestBody.EstimateApproverFeedBacks)
                    {
                        itemApproversFeedbackInSingleRow = @"<tr align='center'><td>" +
                            item.ApproverFullName + "</td><td>" +
                            item.EstimateApproverStatusString +
                            "</td><td>" + item.FeedBackDate + "</td><td>" + item.FeedBack + "</td></tr>";
                        itemApproversFeedback = itemApproversFeedback + itemApproversFeedbackInSingleRow;
                    }
                }
                #endregion


                string estimateType = "";
                if (requestBody.Estimation.EstimateType == 2)
                {
                    estimateType = "New Budget Estimate";
                }
                else if (requestBody.Estimation.EstimateType == 3)
                {
                    estimateType = "Memo";
                }
                else if (requestBody.Estimation.EstimateType == 7)
                {
                    estimateType = "Procurement Approval";
                }
                // string upperBodyPart = @"<p>A <b>" + estimateType + @"</b> has been initiated by " + userWithDeptInfo.First_Name + " " + userWithDeptInfo.Last_Name + "/" + userWithDeptInfo.DepartmentName + ".Please see the below information.</p>";
                string upperBodyPart = " ";

                body = PdfReportHelper.GenericPdfHtmlBody(upperBodyPart, estimateType, requestBody.Estimation.UniqueIdentifier, requestBody.Estimation.Subject, requestBody.Estimation.Objective,
                     requestBody.Estimation.Details, requestBody.Estimation.PlanStartDate, requestBody.Estimation.PlanEndDate,
                     itemDetailsTable, requestBody.Estimation.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")), deptSummary, partiSummary, itemApprovers, itemApproversFeedback, procurementApprovalPart);
                return body;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ActionResult> SingleSettlementPdfReport(int id)
        {
            try
            {


                var settlement = await _settlementService.getSettlementBySettlementId(id);
                var estimate = await _budgetService.GetEstimateById(settlement.EstimationId);
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "Settlement Pdf Report"
                    //Out = @"wwwroot\pdf\" + fileName + ".pdf"
                };

                var objectSettings = new ObjectSettings
                {

                    PagesCount = true,
                    HtmlContent = _htmlGeneratorService.getSettlementHtmlBodyWithEstimationForPdfReport(id).Result,
                    WebSettings = { DefaultEncoding = "utf-8", LoadImages = true },

                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings =
                        {FontName = "Arial", FontSize = 9, Line = true, Center = "Approval Managment System"}

                };
                var pdf = new HtmlToPdfDocument
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }

                };
                var file = _converter.Convert(pdf);
                string todayDate = DateTime.Now.Date.ToString("ddMMyyyy");
                return File(file, "application/pdf", "Settlement -[" + id.ToString() + "]-[" + estimate.UniqueIdentifier + "] - " + todayDate + ".pdf");

            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);

                throw;
            }



        }



        public async Task<ActionResult> SingleMemoPdfReport(int id)
        {
            try
            {

                var memo = await _memoService.GetEstimateMemoEntityByIdService(id);
                string todayDate = DateTime.Now.Date.ToString("ddMMyyyy");
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "Memo Pdf Report"
                    //Out = @"wwwroot\pdf\" + fileName + ".pdf"
                };

                var objectSettings = new ObjectSettings
                {

                    PagesCount = true,
                    HtmlContent = _htmlGeneratorService.getMemoHtmlBodyWithEstimationForPdfReport(id).Result,
                    WebSettings = { DefaultEncoding = "utf-8", LoadImages = true },

                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings =
                        {FontName = "Arial", FontSize = 9, Line = true, Center = "Approval Managment System"}

                };
                var pdf = new HtmlToPdfDocument
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }

                };
                var file = _converter.Convert(pdf);
                return File(file, "application/pdf", "Memo-[" + id.ToString() + "]-[" + memo.EstimateIdentifier + "]-" + todayDate + ".pdf");

            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);

                throw;
            }



        }

        [HttpGet]
        public async Task<EmptyResult> SettlementBulkTempleteDownload(int id)
        {
            try
            {
                var estimate = await _budgetService.GetEstimationFullInfo(id);
                if (estimate == null)
                    throw new Exception("Invalid Estimate");
                var fullestimateList = new List<AddBudgetEstimation>
                {
                    estimate
                };
                // this.ExcelReport(fullestimateList, estimate.Estimation.UniqueIdentifier);
                this.BulkSettlementExcelTemplete(fullestimateList, estimate.Estimation.UniqueIdentifier);

                return new EmptyResult();
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void BulkSettlementExcelTemplete(IList<AddBudgetEstimation> requestObjList, string fileName)
        {
            using var workbook = new XLWorkbook();

            #region Esimate Basic Information 
            var worksheetSummary = workbook.Worksheets.Add("Summary");
            worksheetSummary.Columns().Width = 25;
            worksheetSummary.Column(3).Width = 35;
            worksheetSummary.Column(4).Width = 45;

            var currentRowForSummary = 1;
            worksheetSummary.Cell(currentRowForSummary, 1).Value = "Approval For";
            worksheetSummary.Cell(currentRowForSummary, 2).Value = "Identification";
            worksheetSummary.Cell(currentRowForSummary, 3).Value = "Subject";
            worksheetSummary.Cell(currentRowForSummary, 4).Value = "Objective";
            worksheetSummary.Cell(currentRowForSummary, 5).Value = "Total Price";
            worksheetSummary.Cell(currentRowForSummary, 6).Value = "Plan Start Date";
            worksheetSummary.Cell(currentRowForSummary, 7).Value = "Plan End Date";
            worksheetSummary.Cell(currentRowForSummary, 8).Value = "Created By";
            worksheetSummary.Cell(currentRowForSummary, 9).Value = "Creation Time";

            worksheetSummary.Row(1).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheetSummary.Row(1).Style.Font.Bold = true;

            foreach (var item in requestObjList)
            {
                currentRowForSummary++;

                worksheetSummary.Cell(currentRowForSummary, 1).Value = item.Estimation.EstimateTypeName;
                worksheetSummary.Cell(currentRowForSummary, 2).Value = item.Estimation.UniqueIdentifier;
                worksheetSummary.Cell(currentRowForSummary, 3).Value = item.Estimation.Subject;
                worksheetSummary.Cell(currentRowForSummary, 4).Value = item.Estimation.Objective;
                worksheetSummary.Cell(currentRowForSummary, 5).Value = item.Estimation.TotalPrice;
                worksheetSummary.Cell(currentRowForSummary, 6).Value = item.Estimation.PlanStartDate;
                worksheetSummary.Cell(currentRowForSummary, 7).Value = item.Estimation.PlanEndDate;
                worksheetSummary.Cell(currentRowForSummary, 8).Value = item.Estimation.CreatedByName;
                worksheetSummary.Cell(currentRowForSummary, 9).Value = item.Estimation.CreationDate.ToString();
            }
            #endregion

            #region Estimate Approver 
            var worksheetApprover = workbook.Worksheets.Add("Approver List");
            worksheetApprover.Columns(2, 6).Width = 25;
            var currentRowForApprover = 1;

            foreach (var estimate in requestObjList)
            {
                worksheetApprover.Cell(currentRowForApprover, 1).Value = "#";
                worksheetApprover.Cell(currentRowForApprover, 2).Value = "Identification";
                worksheetApprover.Cell(currentRowForApprover, 3).Value = "Approver Name";
                worksheetApprover.Cell(currentRowForApprover, 4).Value = "Approver Type";
                worksheetApprover.Cell(currentRowForApprover, 5).Value = "Approver Department";
                worksheetApprover.Cell(currentRowForApprover, 6).Value = "Expected Time";

                worksheetApprover.Row(currentRowForApprover).Style.Fill.BackgroundColor = XLColor.Gray;
                worksheetApprover.Row(currentRowForApprover).Style.Font.Bold = true;

                var serial = 0;
                foreach (var item in estimate.EstimateApproverList)
                {
                    currentRowForApprover++;
                    serial++;
                    worksheetApprover.Cell(currentRowForApprover, 1).Value = serial;
                    worksheetApprover.Cell(currentRowForApprover, 2).Value = estimate.Estimation.UniqueIdentifier;
                    worksheetApprover.Cell(currentRowForApprover, 3).Value = item.ApproverFullName;
                    worksheetApprover.Cell(currentRowForApprover, 4).Value = item.ApproverRole;
                    worksheetApprover.Cell(currentRowForApprover, 5).Value = item.ApproverDepartment;
                    worksheetApprover.Cell(currentRowForApprover, 6).Value = item.ExpectedTime;
                }

                worksheetApprover.Row(currentRowForApprover).InsertRowsBelow(1);
                currentRowForApprover += 2;
            }

            #endregion

            #region Estimate Item 
            var worksheetitem = workbook.Worksheets.Add("Particular Items");
            worksheetitem.Columns().Width = 5;
            worksheetitem.Columns(2, 4).Width = 30;
            worksheetitem.Columns(5, 12).Width = 15;
            worksheetitem.Columns(13, 17).Width = 30;
            var currentRowForitem = 1;
            worksheetitem.Column(18).Hide();
            worksheetitem.Unprotect();
            //worksheetitem.Protect("scl@123456"); 
            //worksheetitem.Column(18).Style.Protection.SetLocked(false);
            workbook.Style.Protection.SetLocked(false);


            foreach (var estimate in requestObjList)
            {
                #region Heading
                worksheetitem.Cell(currentRowForitem, 1).Value = "#";
                worksheetitem.Cell(currentRowForitem, 2).Value = "Identification";
                worksheetitem.Cell(currentRowForitem, 3).Value = "Particular";
                worksheetitem.Cell(currentRowForitem, 4).Value = "Item Category";
                worksheetitem.Cell(currentRowForitem, 5).Value = "Item";
                worksheetitem.Cell(currentRowForitem, 6).Value = "Item Code";
                worksheetitem.Cell(currentRowForitem, 7).Value = "Unit";
                worksheetitem.Cell(currentRowForitem, 8).Value = "No. Of Machine /Usages /Team /Car";
                worksheetitem.Cell(currentRowForitem, 9).Value = "No. Of Day /Total Unit";
                worksheetitem.Cell(currentRowForitem, 10).Value = "Required Quantity";
                worksheetitem.Cell(currentRowForitem, 11).Value = "Unit Price";
                worksheetitem.Cell(currentRowForitem, 12).Value = "Total Price";
                worksheetitem.Cell(currentRowForitem, 13).Value = "Department";
                worksheetitem.Cell(currentRowForitem, 14).Value = "District";
                worksheetitem.Cell(currentRowForitem, 15).Value = "Thana";
                worksheetitem.Cell(currentRowForitem, 16).Value = "Area type";
                worksheetitem.Cell(currentRowForitem, 17).Value = "Remarks";
                worksheetitem.Cell(currentRowForitem, 18).Value = "EstimateSettleItemId";
                worksheetitem.Cell(currentRowForitem, 19).Value = "ActualQuantity";
                worksheetitem.Cell(currentRowForitem, 20).Value = "actualUnitPrice";
                worksheetitem.Cell(currentRowForitem, 21).Value = "SettlementRemarks";


                worksheetitem.Row(currentRowForitem).Style.Fill.BackgroundColor = XLColor.Gray;
                worksheetitem.Row(currentRowForitem).Style.Font.Bold = true;

                #endregion

                #region For No Item
                if (estimate.EstimateDetails == null)
                {
                    currentRowForitem++;
                    worksheetitem.Cell(currentRowForitem, 1).Value = 1;
                    worksheetitem.Cell(currentRowForitem, 2).Value = estimate.Estimation.UniqueIdentifier;
                    worksheetitem.Cell(currentRowForitem, 3).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 4).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 5).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 6).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 7).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 8).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 9).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 10).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 11).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 12).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 13).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 14).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 15).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 16).Value = "N/A";
                    worksheetitem.Cell(currentRowForitem, 17).Value = "N/A";

                    worksheetitem.Row(currentRowForitem).InsertRowsBelow(1);
                    currentRowForitem += 2;

                    continue;
                }
                #endregion
                var serial = 0;
                foreach (var item in estimate.EstimateDetails)
                {
                    if (item.ResponsibleDepartment_Id != 41)
                    {
                        currentRowForitem++;
                        serial++;

                        worksheetitem.Cell(currentRowForitem, 1).Value = serial;
                        worksheetitem.Cell(currentRowForitem, 2).Value = estimate.Estimation.UniqueIdentifier;
                        worksheetitem.Cell(currentRowForitem, 3).Value = item.Particular;
                        worksheetitem.Cell(currentRowForitem, 4).Value = item.ItemCategory;
                        worksheetitem.Cell(currentRowForitem, 5).Value = item.ItemName;
                        worksheetitem.Cell(currentRowForitem, 6).Value = item.ItemCode;
                        worksheetitem.Cell(currentRowForitem, 7).Value = item.ItemUnit;
                        worksheetitem.Cell(currentRowForitem, 8).Value = item.NoOfMachineAndUsagesAndTeamAndCar;
                        worksheetitem.Cell(currentRowForitem, 9).Value = item.NoOfDayAndTotalUnit;
                        worksheetitem.Cell(currentRowForitem, 10).Value = item.QuantityRequired;
                        worksheetitem.Cell(currentRowForitem, 11).Value = item.UnitPrice;
                        worksheetitem.Cell(currentRowForitem, 12).Value = item.TotalPrice;
                        worksheetitem.Cell(currentRowForitem, 13).Value = item.ResponsibleDepartmentName;
                        worksheetitem.Cell(currentRowForitem, 14).Value = item.DistrictName;
                        worksheetitem.Cell(currentRowForitem, 15).Value = item.ThanaName;
                        worksheetitem.Cell(currentRowForitem, 16).Value = item.AreaType;
                        worksheetitem.Cell(currentRowForitem, 17).Value = item.Remarks;
                        worksheetitem.Cell(currentRowForitem, 18).Value = item.EstimateSettleItemId;
                        worksheetitem.Cell(currentRowForitem, 19).Value = "";
                        worksheetitem.Cell(currentRowForitem, 20).Value = "";
                        worksheetitem.Cell(currentRowForitem, 21).Value = "";
                        //if (item.ResponsibleDepartment_Id != 41)
                        //{
                        //    worksheetitem.Row(currentRowForitem).Style.Protection.SetLocked(false);
                        //}

                        //if (item.ResponsibleDepartment_Id  == 41)
                        //{
                        //    worksheetitem.Row(currentRowForitem).Style.Fill.BackgroundColor = XLColor.GrayAsparagus;
                        //}


                    }
                }
                currentRowForitem++;
                worksheetitem.Cell(currentRowForitem, 12).Value = estimate.Estimation.TotalPrice;

                worksheetitem.Row(currentRowForitem).InsertRowsBelow(1);
                currentRowForitem += 2;
            }

            #endregion

            #region Estimate Summaray By Department and Particular 
            var worksheetdeptparti = workbook.Worksheets.Add("Budget Summaries");
            worksheetdeptparti.Columns().Width = 5;
            worksheetdeptparti.Columns(2, 4).Width = 20;
            worksheetdeptparti.Columns(7, 9).Width = 20;
            worksheetdeptparti.Columns(2, 5).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheetdeptparti.Columns(7, 10).Style.Fill.BackgroundColor = XLColor.BlueGray;
            var currentRowFordeptParti = 1;

            foreach (var estimate in requestObjList)
            {
                if (estimate.ParticularWiseSummary != null || estimate.DepartmentWiseSummary != null)
                {
                    worksheetdeptparti.Cell(currentRowFordeptParti, 2).Value = "Identification";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 3).Value = "Department Wise Summmary";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 4).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 7).Value = "Identification";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 8).Value = "Particular Wise Summary";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 9).Value = "";

                    worksheetdeptparti.Row(currentRowFordeptParti).Style.Font.Bold = true;

                    currentRowFordeptParti++;
                    worksheetdeptparti.Cell(currentRowFordeptParti, 2).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 3).Value = "Department";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 4).Value = "Total Price";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 7).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 8).Value = "Particular";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 9).Value = "Total Price";

                    var lineForDept = currentRowFordeptParti;
                    foreach (var item in estimate.DepartmentWiseSummary)
                    {
                        lineForDept++;
                        worksheetdeptparti.Cell(lineForDept, 2).Value = estimate.Estimation.UniqueIdentifier;
                        worksheetdeptparti.Cell(lineForDept, 3).Value = item.DepartmentName;
                        worksheetdeptparti.Cell(lineForDept, 4).Value = item.TotalPrice;
                    }
                    lineForDept++;
                    worksheetdeptparti.Cell(lineForDept, 2).Value = "Grand Total";
                    worksheetdeptparti.Cell(lineForDept, 4).Value = estimate.Estimation.TotalPrice;

                    var lineForparti = currentRowFordeptParti;
                    foreach (var item in estimate.ParticularWiseSummary)
                    {
                        lineForparti++;
                        worksheetdeptparti.Cell(lineForparti, 7).Value = estimate.Estimation.UniqueIdentifier;
                        worksheetdeptparti.Cell(lineForparti, 8).Value = item.ParticlarName;
                        worksheetdeptparti.Cell(lineForparti, 9).Value = item.TotalPrice;
                    }
                    lineForparti++;
                    worksheetdeptparti.Cell(lineForparti, 7).Value = "Grand Total";
                    worksheetdeptparti.Cell(lineForparti, 9).Value = estimate.Estimation.TotalPrice;

                    currentRowFordeptParti = (lineForDept >= lineForparti) ? lineForDept : lineForparti;

                    worksheetdeptparti.Row(currentRowFordeptParti).InsertRowsBelow(1);
                    worksheetdeptparti.Row(currentRowFordeptParti + 1).Style.Fill.BackgroundColor = XLColor.White;
                    currentRowFordeptParti += 2;
                }
                else
                {
                    worksheetdeptparti.Cell(currentRowFordeptParti, 2).Value = "Identification";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 3).Value = "Department Wise Summmary";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 4).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 7).Value = "Identification";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 8).Value = "Particular Wise Summary";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 9).Value = "";

                    worksheetdeptparti.Row(currentRowFordeptParti).Style.Font.Bold = true;

                    currentRowFordeptParti++;
                    worksheetdeptparti.Cell(currentRowFordeptParti, 2).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 3).Value = "Department";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 4).Value = "Total Price";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 7).Value = "";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 8).Value = "Particular";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 9).Value = "Total Price";

                    currentRowFordeptParti++;
                    worksheetdeptparti.Cell(currentRowFordeptParti, 2).Value = estimate.Estimation.UniqueIdentifier;
                    worksheetdeptparti.Cell(currentRowFordeptParti, 3).Value = "N/A";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 4).Value = "N/A";

                    worksheetdeptparti.Cell(currentRowFordeptParti, 7).Value = estimate.Estimation.UniqueIdentifier;
                    worksheetdeptparti.Cell(currentRowFordeptParti, 8).Value = "N/A";
                    worksheetdeptparti.Cell(currentRowFordeptParti, 9).Value = "N/A";

                    worksheetdeptparti.Row(currentRowFordeptParti).InsertRowsBelow(1);
                    worksheetdeptparti.Row(currentRowFordeptParti + 1).Style.Fill.BackgroundColor = XLColor.White;
                    currentRowFordeptParti += 2;
                }
            }

            #endregion

            #region Estimate Approver 
            var worksheetfeed = workbook.Worksheets.Add("Approvers FeedBacks");
            worksheetfeed.Columns(2, 6).Width = 20;
            var currentRowForfeed = 1;

            foreach (var estimate in requestObjList)
            {
                if (estimate.EstimateApproverFeedBacks != null)
                {
                    worksheetfeed.Cell(currentRowForfeed, 1).Value = "#";
                    worksheetfeed.Cell(currentRowForfeed, 2).Value = "Identification";
                    worksheetfeed.Cell(currentRowForfeed, 3).Value = "Approver Name";
                    worksheetfeed.Cell(currentRowForfeed, 4).Value = "Status";
                    worksheetfeed.Cell(currentRowForfeed, 5).Value = "Feedback Date";
                    worksheetfeed.Cell(currentRowForfeed, 6).Value = "Remarks";

                    worksheetfeed.Row(currentRowForfeed).Style.Fill.BackgroundColor = XLColor.Gray;
                    worksheetfeed.Row(currentRowForfeed).Style.Font.Bold = true;

                    var serial = 0;
                    foreach (var item in estimate.EstimateApproverFeedBacks)
                    {
                        currentRowForfeed++;
                        serial++;
                        worksheetfeed.Cell(currentRowForfeed, 1).Value = serial;
                        worksheetfeed.Cell(currentRowForfeed, 2).Value = estimate.Estimation.UniqueIdentifier;
                        worksheetfeed.Cell(currentRowForfeed, 3).Value = item.ApproverFullName;
                        worksheetfeed.Cell(currentRowForfeed, 4).Value = item.EstimateApproverStatusString;
                        worksheetfeed.Cell(currentRowForfeed, 5).Value = item.FeedBackDate;
                        worksheetfeed.Cell(currentRowForfeed, 6).Value = item.FeedBack;
                    }

                    worksheetfeed.Row(currentRowForfeed).InsertRowsBelow(1);
                    currentRowForfeed += 2;
                }
            }

            #endregion
            #region estimateDetails
            //var estimateDetailsWorksheet = workbook.Worksheets.Add("Estimate_Details");

            //estimateDetailsWorksheet.Columns().Width = 65;
            //var currentRowForDetails = 1;
            //estimateDetailsWorksheet.Cell(currentRowForDetails, 1).Value = "Estimate Details";
            //foreach (var item in requestObjList)
            //{
            //    currentRowForDetails++;
            //    var esDetails = item.Estimation.Details;
            //    estimateDetailsWorksheet.Cell(currentRowForDetails, 1).Value = esDetails;
            //}
            #endregion

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            Response.Clear();
            Response.Headers.Add("content-disposition", "attachment;filename=" + fileName + "_Budget_Esimate_Report_" + DateTime.Now.ToString("MM/dd/yyyy") + ".xlsx");
            Response.ContentType = "application/xlsx";
            Response.Body.WriteAsync(content);
            Response.Body.Flush();
        }

        public async Task<ActionResult> SinglePdfEstimateReportV2(int id)
        {
            try
            {



                var estimate = await _budgetService.GetEstimateById(id);
                string EstimationTypeName = "";

                if (estimate.EstimateType_Id == EstimationType.NEW_BUDGET)
                {
                    EstimationTypeName = "New Budget";
                }
                else if (estimate.EstimateType_Id == EstimationType.PROCUREMENT)
                {
                    EstimationTypeName = "Procurment";
                }
                else if (estimate.EstimateType_Id == EstimationType.Factsheet)
                {
                    EstimationTypeName = "Factsheet";
                }
                else if (estimate.EstimateType_Id == EstimationType.MEMO)
                {
                    EstimationTypeName = "Memo";
                }

                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = EstimationTypeName + " Pdf Report"

                };

                var objectSettings = new ObjectSettings
                {

                    PagesCount = true,
                    HtmlContent = await _htmlGeneratorService.getEstimateHtmlBodyWithEstimationForPdfReportV2(id),
                    WebSettings = { DefaultEncoding = "utf-8", LoadImages = true },

                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings =
                        {FontName = "Arial", FontSize = 9, Line = true, Center = "Approval Managment System"}

                };
                var pdf = new HtmlToPdfDocument
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }

                };
                var file = _converter.Convert(pdf);
                string todayDate = DateTime.Now.Date.ToString("ddMMyyyy");
                return File(file, "application/pdf", EstimationTypeName + "-[" + estimate.UniqueIdentifier + "] - " + todayDate + ".pdf");

            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);

                throw;
            }



        }



    }
}

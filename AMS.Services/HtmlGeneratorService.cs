using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AMS.Common.Constants;
using AMS.Common.Helpers;
using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.FundDisburse;
using AMS.Models.ServiceModels.FundRequisition;
using AMS.Models.ServiceModels.Settlement;
using AMS.Models.ViewModel;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Contracts;
using AMS.Services.Managers.Contracts;
using HTML.Generator.Helper;
using HtmlGenerate;
using Newtonsoft.Json;

namespace AMS.Services
{
    public class HtmlGeneratorService : IHtmlGeneratorService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionManager _sessionManager;

        public HtmlGeneratorService(IUnitOfWorkFactory uowFactory, ISessionManager sessionManager)
        {
            _uowFactory = uowFactory;
            _sessionManager = sessionManager;
        }

        public async Task<string> getSettlementHtmlBodyWithEstimationForPdfReport(int settlementId)
        {

            string body = "";
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");
                //var setttlement = _settlementService.getSettlementBySettlementId(settlementId).Result;
              
                
                   var setttlement = await uow.SettlementRepo.getSettlementById(settlementId);



                   //body += GetHtmlEstimationPartForPDFReport(setttlement.EstimationId, uow, sessionUser).Result;
                   body += getSummitLogoTable();
                   body += "</br>" + @"<p></p>";
                body +=" </br>" + getEstimationInfo(setttlement.EstimationId, uow);
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Line Items", getEstimatationLineItemsData(setttlement.EstimationId, uow));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Approvers", getEstimationApprovers(setttlement.EstimationId, uow));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Approvers Feedback", getEstimateApproverFeedBaCK(setttlement.EstimationId, uow));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Department Wise Summary", getDepartmentWiseSummaryByEstimateId(setttlement.EstimationId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Particular Wise Summary", getParticularWiseSummaryByEstimateId(setttlement.EstimationId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Fund Requisition & Disburse History", getTotalFundRequisitionDisburseHistory(setttlement.EstimationId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData(" Settlement", getSettlementInfo(setttlement));
                //body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Total Settle Amount(BDT)", "<span style='padding-left : 30px; font-weight : bold ; font-size : 25px;' >" + setttlement.TotalAmount.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")) + "</span>");
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Settlement Items", this.getSettlementItemData(settlementId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Settlement Approver", this.getSettlementApprover(settlementId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Settlement Approver Feedback", this.getSettlementApproverFeedBaCK(settlementId));
       

            }

            return body;
        }
        public string getDepartmentWiseSummaryWithCostBudgetFinalCardHtml(int estimationId)
        {
            return HtmlGenerate.HtmlGenerate.divWrappperForTableData("Department wise Summary",
                this.getDepartmentWiseSummaryByEstimateId(estimationId));
        }

        public string getPerticularWiseSummaryWithCostBudgetFinalCardHtml(int estimationId)
        {
            return HtmlGenerate.HtmlGenerate.divWrappperForTableData("Particular wise Summary",
                this.getParticularWiseSummaryByEstimateId(estimationId));
        }
        public string getTotalFundRequisitionFinalCardHtml(int estimationId)
        {
            return HtmlGenerate.HtmlGenerate.divWrappperForTableData("Fund Requisition & Disburse History",
                this.getTotalFundRequisitionDisburseHistory(estimationId));
        }
        public string getFundRequisitionFinalCardHtml(FundRequisitionVM fundRequisitionVm)
        {
            return HtmlGenerate.HtmlGenerate.divWrappperForTableData("Fund Requisition Info :",
                this.getFundRequisitionInfoHtmlTable(fundRequisitionVm));
        }
        public string getFundRequisitionRejectFinalCardHtml(FundRequisitionVM fundRequisitionVm)
        {
            return HtmlGenerate.HtmlGenerate.divWrappperForTableData("Fund Requisition Info :",
                this.getFundRequisitionRejectInfoHtmlTable(fundRequisitionVm));
        }

        public string getFundDisburseFinalCardHtml(FundDisburseVM fundRequisitionVm)
        {
            return HtmlGenerate.HtmlGenerate.divWrappperForTableData("Fund Requisition Info :",
                this.getFundDisburseInfoHtmlTable(fundRequisitionVm));
        }

        public string getFundDisburseReceivedOrRollbackFinalCardHtml(FundDisburseVM fundRequisitionVm)
        {
            return HtmlGenerate.HtmlGenerate.divWrappperForTableData("Fund Requisition Info :",
                this.getFundDisburseReceivedRollbackInfoHtmlTable(fundRequisitionVm));
        }
        public string getSettlementItemData(int settlementId)
        {


            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @" ",
                TableHeader = new List<string>
                {
                     "Item Description",
                    "Estimate Details", "Location", "Settlement Details", "Remarks"
                },
                items = getSettleItemData(settlementId)


            });

            return htmltable;
        }
        public string getEstimateSettlementItemSummaryData(int estimationId , IUnitOfWork uow)
        {


            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @" ",
                ExtraTableProperty = @"class='table table-responsive-md table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Item Description",
                    "Estimate Details", "Location", "Settlement Details"
                },
                items = getEstimateSettleItemSummaryData(estimationId , uow)


            });

            return htmltable;
        }
        public string getEstimationInfo(int estimationId, IUnitOfWork uow)
        {
            var estimateInfo = uow.EstimationRepo.SingleEstimationWithType(estimationId).Result;
            var procurementApprovalOrFactsheet  = uow.ProcurementApprovalRepo.GetProcurementApprovalByEstimateId(estimationId).Result;

            var htmlTableParameter = new HtmlTableParameter()
            {
                TableStyle = @" ",
                ExtraTableProperty = @"class='table table-responsive-md table-hover table-bordered' ",
                TableHeader = null,
                items = new List<ItemParam>()
            };
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {

                        Style = @"font-weight: bold; width : 15%; ",
                        Data = @"Approval For"
                    },
                    new TdParam
                    {
                        Data = @"" + estimateInfo.EstimationTypeName
                    }
                }
            });
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {
                        Style = @"font-weight: bold; ",
                        Data = @"Identification "
                    },
                    new TdParam
                    {
                        Style = @"font-weight: bold; ",
                        Data = @"" + estimateInfo.EstimationIdentifier
                    }
                }
            });
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {
                        Style = @"font-weight: bold; ",
                        Data = @"Subject "
                    },
                    new TdParam
                    {
                        Data = @"" + estimateInfo.EstimationSubject
                    }
                }
            });
            if (estimateInfo.EstimationTypeId == EstimationType.PROCUREMENT ||
                estimateInfo.EstimationTypeId == EstimationType.Factsheet)
            {
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"PA. Reference No. "
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.PAReferenceNo
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"Title of the PR/RFQ "
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.TitleOfPRorRFQ
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"RFQ Reference No. "
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.RFQReferenceNo
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"PR Reference No. "
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.PRReferenceNo
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"Name and Cell No. of Requester "
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.NameOfRequester
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"Department "
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.DepartmentName
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"RFQ Process "
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.RFQProcess
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"Sourcing Method"
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.SourcingMethod
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"Supplier Name"
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.NameOfRecommendedSupplier
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"Purchase Value
"
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.PurchaseValue
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"Saving Amount"
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.SavingAmount
                        }
                    }
                });
                htmlTableParameter.items.Add(new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {
                            Style = @"font-weight: bold; ",
                            Data = @"Saving Type"
                        },
                        new TdParam
                        {
                            Data = @"" + procurementApprovalOrFactsheet.SavingType
                        }
                    }
                });



            }
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {
                        Style = @"font-weight: bold; ",
                        Data = @"Objective "
                    },
                    new TdParam
                    {
                        Data = @"" + estimateInfo.EstimationObjective
                    }
                }
            });
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {
                        Style = @"border : none !important; font-weight: bold; ",
                        Data = @"Details :"
                    },
                    new TdParam
                    {
                        Style = @"border : none !important;",
                        Data = @"" + estimateInfo.EstimationDetails
                    }
                }
            });
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {
                        Style = @"font-weight: bold; ",
                        Data = @"Time Period:"
                    },
                    new TdParam
                    {
                        Data = @"Form : " + estimateInfo.EstimationPlanStartDate.ToString("MM/dd/yyyy") +
                               " To : " + estimateInfo.EstimationPlanEndDate.ToString("MM/dd/yyyy")
                    }
                }
            });
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {
                        Style = @"font-weight: bold; ",
                        Data = @"Created on :"
                    },
                    new TdParam
                    {
                        Data = @"" + estimateInfo.CreatedDate.ToString("MM/dd/yyyy")
                    }
                }
            });
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {
                        Style = @"font-weight: bold; ",
                        Data = @"Created By:"
                    },
                    new TdParam
                    {
                        Data = @" " + estimateInfo.CreateorFullName
                    }
                }
            });
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {
                        Style = @"font-weight: bold; ",
                        Data = @"Creator Department:"
                    },
                    new TdParam
                    {
                        Data = @"" + estimateInfo.CreatorDepartment
                    }
                }
            });
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {
                        Style = @"font-weight: bold; ",
                        Data = @"Total Price :"
                    },
                    new TdParam
                    {
                        Data = @"" + estimateInfo.EstimaionTotalPrice.ToString() +" "+ Utility.CurrencyTypeConvertToStringFormat(estimateInfo.CurrencyType)
                              
                    }
                }
            });
            htmlTableParameter.items.Add(new ItemParam
            {

                Data = new List<TdParam>
                {

                    new TdParam
                    {
                        Style = @"font-weight: bold; ",
                        Data = @" Price Remarks :"
                    },
                    new TdParam
                    {
                        Data = @"" + estimateInfo.TotalPriceRemarks
                    }
                }
            });

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(htmlTableParameter);
            

            return htmltable;
        }
        public string getEstimatationLineItemsData(int estimationId, IUnitOfWork uow)
        {


            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @" ",
                ExtraTableProperty = @"class='table table-responsive-md table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Item Description",
                    "Estimate Details", "Location", "Remarks"
                },
                items = getEstimateLineItems(estimationId, uow)


            });

            return htmltable;
        }
        public string getDepartmentWiseSummaryByEstimateId(int estimateId)
        {


            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @" ",
                ExtraTableProperty = @"class='table table-responsive-md table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Departments",
                    "Total Allowed Budget (TK.)", "Total Cost (TK.)", "Deviation", "Percentage"
                },
                items = getDepartmentWiseSummaryRowByEstimate(estimateId)


            });

            return htmltable;
        }
        public string getDepartmentWiseSummaryByEstimateIdForEstimate(int estimateId)
        {


            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @" ",
                ExtraTableProperty = @"class='table table-responsive-md table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Departments",
                    "Total Allowed Budget (TK.)"
                },
                items = getDepartmentWiseSummaryRowByEstimateForEstimate(estimateId)


            });

            return htmltable;
        }
        public string getTotalFundRequisitionDisburseHistory(int estimateId)
        {


            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @" ",
                ExtraTableProperty = @"class='table table-responsive-md table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Req. SL", "Department", "Fund Type", "Date",
                    "Amount", " Disburse SL" , "Date", "Amount"
                },
                items = getTotalFundRequisitionDisburseHistoryRowByEstimate(estimateId)


            });

            return htmltable;
        }
        public string getParticularWiseSummaryByEstimateId(int estimateId)
        {


            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @" ",
                ExtraTableProperty = @"class='table table-responsive-md table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Particulars",
                    "Total Budget (TK.)", "Total Cost (TK.)", "Deviation", "Percentage"
                },
                items = getParticularsWiseSummaryRowByEstimate(estimateId)


            });

            return htmltable;
        }
        public string getParticularWiseSummaryByEstimateIdForEstimate(int estimateId)
        {


            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @" ",
                ExtraTableProperty = @"class='table table-responsive-md table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Particulars",
                    "Total Budget (TK.)"/*, "Total Cost (TK.)", "Deviation", "Percentage"*/
                },
                items = getParticularsWiseSummaryRowByEstimateForEstimate(estimateId)


            });

            return htmltable;
        }
        private string getSettlementInfo(CreateSettlementRequest settlement)
        {

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                TableHeader = null,
                items = new List<ItemParam>()
                {
                    
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {
                                new TdParam
                                {

                                    Data ="<b>Is It Final Settlement</b>"
                                },
                                new TdParam
                                {

                                    Data =settlement.IsItFinalSetttlement == 1 ?"Yes" : "No"
                                },

                                new TdParam
                                {

                                    Data ="<b>Total Settle Amount</b>"
                                },
                                new TdParam
                                {

                                    Data =settlement.TotalAmount.ToString("N", new CultureInfo("hi-IN")) + "&#2547;"
                                },


                            }
                        }

                }


            });


            return htmltable;

        }
        private string getSettlementApproverFeedBaCK(int settlementId)
        {

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Approver", "Status", "Feedback Date", "Feedback"
                },
                items = getSettlementApproverFeedBackItemsItemParamsData(settlementId)


            });


            return htmltable;

        }
        private string getMemoApproverFeedBaCK(int memoId)
        {
            int isFeedbackNull = 0;

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Approver", "Status", "Feedback Date", "Feedback"
                },
                items = getMemoApproverFeedBackItemsItemParamsData(memoId)


            });
            

            return htmltable;

        }
        private string getEstimateApproverFeedBaCK(int estimationId , IUnitOfWork uow)
        {
            int isFeedbackNull = 0;

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Approver", "Status", "Feedback Date", "Feedback"
                },
                items = getEstimationApproversFeedbackParamData(estimationId, uow)


            });


            return htmltable;

        }
        private string getSettlementApprover(int settlementId)
        {

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Approver Name", "Approver Type", "Approver Department", "Expected Date"
                },
                items = getSettlementApproverItemsItemParamsData(settlementId)


            });


            return htmltable;

        }
        private string getEstimationApprovers(int estimationId, IUnitOfWork uow)
        {

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Approver Name", "Approver Type", "Approver Department", "Expected Date"
                },
                items = getEstimationApproverItemsItemParamsData(estimationId, uow)


            });


            return htmltable;

        }
        private string getMemoApprover(int memoId)
        {

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Approver Name", "Approver Type", "Approver Department", "Expected Date"
                },
                items = getMemoApproverItemsItemParamsData(memoId)


            });


            return htmltable;

        }
        private string getBudgetSettlementSummaryForMemo(int memoId)
        {

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                TableHeader = new List<string>
                {
                    "Total Budget", "Total Allowed Budget", "Total Cost (TK.)", "Deviation", "Percentage(%)"
                },
                items = getBudgetSettlementItemParamsData(memoId)


            });


            return htmltable;

        }
        private List<ItemParam> getDepartmentWiseSummaryRowByEstimate(int estimateId)
        {
            var itemParams = new List<ItemParam>();
            using var uow = _uowFactory.GetUnitOfWork();
            var deptWiseSummaries = uow.SettlementDepartmentWiseSummaryRepo
                .LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(estimateId).Result;
            uow.Commit();
            var tTotalCost = 0.0;
            var totalBudgetExclueScm = 0.0;
            var tTotalBudget = 0.0;
            var tTotalDeviation = 0.0;

            var rowDepartmentWiseSummaryTableIndex = 1;
            CultureInfo cultures = new CultureInfo("en-US");

            foreach (var deptWiseSummary in deptWiseSummaries)
            {

                var deviation = 0.0;
                var percentage = 0.0;
                if (deptWiseSummary.TotalCost > deptWiseSummary.TotalBudget)
                {
                    deviation = deptWiseSummary.TotalCost - deptWiseSummary.TotalBudget;
                    var divide = deptWiseSummary.TotalCost / deptWiseSummary.TotalBudget;
                    percentage = divide * 100;
                    tTotalDeviation += deviation;
                }
                tTotalCost += deptWiseSummary.TotalCost;
                tTotalBudget += deptWiseSummary.TotalBudget;

                if (deptWiseSummary.DepartmentId != 41)
                {
                    totalBudgetExclueScm += deptWiseSummary.TotalBudget;
                }

                if (deptWiseSummary.DepartmentId == 41)
                {
                    itemParams.Add(
                        new ItemParam
                        {
                            Style =@"color:#B2BEB5; ",
                            
                            Data = new List<TdParam>
                            {

                                new TdParam
                                {
                                    Data = @"<span> " + deptWiseSummary.DepartmentName + "</span>"

                                },
                                new TdParam
                                {
                                    Data = @"<span> " + deptWiseSummary.TotalBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                new TdParam
                                {
                                    Data = @"<span> " + deptWiseSummary.TotalCost.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                new TdParam
                                {
                                    Data = @"<span> " + deviation.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                new TdParam
                                {
                                    Data = @"<span> " + percentage.ToString("#.##") + "%" + "</span>"

                                }
                            }
                        });

                }
                else
                {
                    itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {

                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + deptWiseSummary.DepartmentName + "</span>"

                                },
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + deptWiseSummary.TotalBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + deptWiseSummary.TotalCost.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + deviation.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + percentage.ToString("#.##") + "%"+ "</span>"

                                }
                            }
                        });
                }



            }

            itemParams.Add(
                new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {

                            Data = @"<span style='color: #23098B '>Grand Total(<span style='color: blue '>Exclude SCM</span>)</span>"

                        },
                        new TdParam
                        {

                            Data = @"<span style='color: #23098B '> "+
                            tTotalBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "(<span style='color: #B2BEB5;'>" + totalBudgetExclueScm.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>)</ span >"

                        },
                        new TdParam
                        {

                            Data = @"<span style='color: #23098B'>" +
                            tTotalCost.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                        },
                        new TdParam
                        {
                            Style = @"color : #000000; ",
                            Data = @"<span style='color: #23098B'>" + tTotalDeviation.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;" + "</span>"

                        },
                        new TdParam
                        {
                            Style = @"color : #000000; ",
                            Data = @"<span></span>"

                        }
                    }
                });

            return itemParams;
        }
        private List<ItemParam> getDepartmentWiseSummaryRowByEstimateForEstimate(int estimateId)
        {
            var itemParams = new List<ItemParam>();
            using var uow = _uowFactory.GetUnitOfWork();
            var deptWiseSummaries = uow.SettlementDepartmentWiseSummaryRepo
                .LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(estimateId).Result;
            uow.Commit();
            //var deptWiseSummaries = _budgetService.LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(estimateId).Result;

            //var estimateSettleItems = _settlementItemService.getSettlementItemsBySettlementId(settlementId).Result;
            var tTotalCost = 0.0;
            var totalBudgetExclueScm = 0.0;
            var tTotalBudget = 0.0;
            var tTotalDeviation = 0.0;

            var rowDepartmentWiseSummaryTableIndex = 1;
            CultureInfo cultures = new CultureInfo("en-US");

            foreach (var deptWiseSummary in deptWiseSummaries)
            {

                var deviation = 0.0;
                var percentage = 0.0;
                if (deptWiseSummary.TotalCost > deptWiseSummary.TotalBudget)
                {
                    deviation = deptWiseSummary.TotalCost - deptWiseSummary.TotalBudget;
                    var divide = deptWiseSummary.TotalCost / deptWiseSummary.TotalBudget;
                    percentage = divide * 100;
                    tTotalDeviation += deviation;
                }
                tTotalCost += deptWiseSummary.TotalCost;
                tTotalBudget += deptWiseSummary.TotalBudget;

                if (deptWiseSummary.DepartmentId != 41)
                {
                    totalBudgetExclueScm += deptWiseSummary.TotalBudget;
                }

                if (deptWiseSummary.DepartmentId == 41)
                {
                    itemParams.Add(
                        new ItemParam
                        {
                            Style = @"color:#B2BEB5; ",

                            Data = new List<TdParam>
                            {

                                new TdParam
                                {
                                    Data = @"<span> " + deptWiseSummary.DepartmentName + "</span>"

                                },
                                new TdParam
                                {
                                    Data = @"<span> " + deptWiseSummary.TotalBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                
                            }
                        });

                }
                else
                {
                    itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {

                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + deptWiseSummary.DepartmentName + "</span>"

                                },
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + deptWiseSummary.TotalBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                
                            }
                        });
                }



            }

            itemParams.Add(
                new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {

                            Data = @"<span style='color: #23098B '>Grand Total(<span style='color: blue '>Exclude SCM</span>)</span>"

                        },
                        new TdParam
                        {

                            Data = @"<span style='color: #23098B '> "+
                            tTotalBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "(<span style='color: #B2BEB5;'>" + totalBudgetExclueScm.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>)</ span >"

                        },
                        
                    }
                });

            return itemParams;
        }
        private List<ItemParam> getTotalFundRequisitionDisburseHistoryRowByEstimate(int estimateId)
        {
            var itemParams = new List<ItemParam>();
           // var fundRequisitionDisburseHistories = _fundRequisitionService.getTotalFundRequisitionDisburseHistory(estimateId).Result;


           
                var sessionUser = _sessionManager.GetUser();
               
                using var uow = _uowFactory.GetUnitOfWork();
                var fundRequisitionDisburseHistories = uow.FundRequisitionRepo
                    .getTotalFundRequisitionDisburseHistory(sessionUser.Result.Id, sessionUser.Result.Department_Id,
                        estimateId).Result;
                //uow.Commit();
              
           

            var totalDisbuseAmount = 0.0;
            var totalRequisitionAmount = 0.0;

            var fundReqRow = 0;
            CultureInfo cultures = new CultureInfo("en-US");

            foreach (var fundRequisitionDisburseHistory in fundRequisitionDisburseHistories)
            {

                string paymentTypeClass = fundRequisitionDisburseHistory.FundType == "Payment"
                    ? "label-success"
                    : "label-primary";

                itemParams.Add(
                    new ItemParam
                    {

                        Data = new List<TdParam>
                        {

                            new TdParam
                            {
                                Style = @"color : #000000; ",
                                Data = @"Requisition-" + fundRequisitionDisburseHistory.Sl

                            },
                            new TdParam
                            {
                                Style = @"color : #000000; ",
                                Data = fundRequisitionDisburseHistory.DepartmentName

                            },
                            new TdParam
                            {
                                Style = @"color : #000000; ",
                                ExtraColumnProperty = @"class='label " + paymentTypeClass + "'",
                                Data = fundRequisitionDisburseHistory.FundType

                            },
                            new TdParam
                            {
                                Style = @"color : #000000; ",
                                Data = fundRequisitionDisburseHistory.ProposedDisburseDate

                            },
                            new TdParam
                            {
                                Style = @"color : #000000; ",
                                Data = fundRequisitionDisburseHistory.RequisitionAmount.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" 


                            }
                        }
                    });

                totalRequisitionAmount += fundRequisitionDisburseHistory.RequisitionAmount;
                
                if (fundRequisitionDisburseHistory.DisburseHistory != null)
                {
                    var disburseHistories = JsonConvert.DeserializeObject<List<FundDisburseHistoryVM>>(fundRequisitionDisburseHistory.DisburseHistory);
                    int isItFirstDisburse = 1;
                    foreach (var disburseHistory in disburseHistories)
                    {


                        if (isItFirstDisburse == 1)
                        {

                            itemParams[fundReqRow].Data.Add(
                                    new TdParam
                                    {
                                        Style = @"color : #000000; ",
                                        Data = @"Disburse- " + disburseHistory.SL

                                    }

                                );

                            itemParams[fundReqRow].Data.Add(
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"" + disburseHistory.DisburseDate

                                }

                            );
                            itemParams[fundReqRow].Data.Add(
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"" +
                                    disburseHistory.DisburseAmount.ToString("N", new CultureInfo("hi-IN")) + "&#2547;"

                                }

                            );
                            totalDisbuseAmount += disburseHistory.DisburseAmount;
                            isItFirstDisburse = 0;
                        }
                        else
                        {
                            itemParams.Add(
                                new ItemParam
                                {

                                    Data = new List<TdParam>
                                    {

                                        new TdParam
                                        {
                                            Style = @"color : #000000; ",
                                            ExtraColumnProperty = @"colspan='5'",
                                            Data = @" "

                                        },
                                        new TdParam
                                        {
                                            Style = @"color : #000000; ",
                                            Data = @"Disburse- " + disburseHistory.SL

                                        },
                                        new TdParam
                                        {
                                            Style = @"color : #000000; ",
                                            Data =  disburseHistory.DisburseDate

                                        },
                                        new TdParam
                                        {
                                            Style = @"color : #000000; ",
                                            Data = @"" +
                                                   disburseHistory.DisburseAmount.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" 

                                        }
                                    }
                                });

                            totalDisbuseAmount += disburseHistory.DisburseAmount;

                            fundReqRow += 1;

                        }

                    }


                }

                if (String.IsNullOrEmpty(fundRequisitionDisburseHistory.DisburseHistory))
                {
                    itemParams[fundReqRow].Data.Add(
                        new TdParam
                        {
                            Style = @"color : #000000; ",
                            Data = @"--"

                        }

                    );

                    itemParams[fundReqRow].Data.Add(
                        new TdParam
                        {
                            Style = @"color : #000000; ",
                            Data = @"--"

                        }

                    );
                    itemParams[fundReqRow].Data.Add(
                        new TdParam
                        {
                            Style = @"color : #000000; ",
                            Data = @"--"

                        }

                    );

                }

                fundReqRow += 1;
            }

            itemParams.Add(
                new ItemParam
                {

                    Data = new List<TdParam>
                    {
                       
                            
                           
                        new TdParam
                        {
                            
                            ExtraColumnProperty = @"colspan='5' align='right'",
                            Style = @"font-weight: bold; color : #000000;",
                            Data = @" Total Requisition :" + totalRequisitionAmount.ToString("N", new CultureInfo("hi-IN")) + "&#2547;"

                        },
                        new TdParam
                        {
                            Style = @"color : #000000; ",
                            Data = @""

                        },

                        new TdParam
                        {
                            Style = @"font-weight: bold; color : #000000;",
                            ExtraColumnProperty = @"colspan='2' align='right'",
                            Data = @" Total Disburse :" + totalDisbuseAmount.ToString("N", new CultureInfo("hi-IN")) + "&#2547;"

                        },

                    }
                });

            return itemParams;
        }

        private List<ItemParam> getParticularsWiseSummaryRowByEstimate(int estimateId)
        {
            var itemParams = new List<ItemParam>();
            using var uow = _uowFactory.GetUnitOfWork();
            var partWiseSummaries =  uow.SettlementParticularWiseSummaryRepo.LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(estimateId).Result;
            uow.Commit();
            //var partWiseSummaries = _budgetService.LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(estimateId).Result;

            //var estimateSettleItems = _settlementItemService.getSettlementItemsBySettlementId(settlementId).Result;
            var tTotalCost = 0.0;
            var totalBudgetExclueScm = 0.0;
            var tTotalBudget = 0.0;
            var tTotalDeviation = 0.0;

            var rowDepartmentWiseSummaryTableIndex = 1;
            CultureInfo cultures = new CultureInfo("en-US");

            foreach (var particularWiseSummary in partWiseSummaries)
            {

                var deviation = 0.0;
                var percentage = 0.0;
                if (particularWiseSummary.TotalCost > particularWiseSummary.TotalBudget)
                {
                    deviation = particularWiseSummary.TotalCost - particularWiseSummary.TotalBudget;
                    var divide = particularWiseSummary.TotalCost / particularWiseSummary.TotalBudget;
                    percentage = divide * 100;
                    tTotalDeviation += deviation;
                }
                tTotalCost += particularWiseSummary.TotalCost;
                tTotalBudget += particularWiseSummary.TotalBudget;





                itemParams.Add(
                    new ItemParam
                    {

                        Data = new List<TdParam>
                        {

                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + particularWiseSummary.ParticularName + "</span>"

                                },
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + particularWiseSummary.TotalBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + particularWiseSummary.TotalCost.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + deviation.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + percentage.ToString("#.##") + "</span>"

                                }
                        }
                    });




            }

            itemParams.Add(
                new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {

                            Data = @"<span style='color: #23098B '>Grand Total</span>"

                        },
                        new TdParam
                        {

                            Data = @"<span style='color: #23098B '> "+
                            tTotalBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</ span >"

                        },
                        new TdParam
                        {

                            Data = @"<span style='color: #23098B'>" +
                            tTotalCost.ToString("N", new CultureInfo("hi-IN")) + "&#2547;"  + "</span>"

                        },
                        new TdParam
                        {
                            Style = @"color : #000000; ",
                            Data = @"<span style='color: #23098B'>" + tTotalDeviation.ToString("N", new CultureInfo("hi-IN")) + "&#2547;"  + "</span>"

                        },
                        new TdParam
                        {  Style = @"color : #000000; ",
                            Data = @"<span></span>"

                        }
                    }
                });

            return itemParams;
        }

        private List<ItemParam> getParticularsWiseSummaryRowByEstimateForEstimate(int estimateId)
        {
            var itemParams = new List<ItemParam>();
            using var uow = _uowFactory.GetUnitOfWork();
            var partWiseSummaries = uow.SettlementParticularWiseSummaryRepo.LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(estimateId).Result;
            uow.Commit();
            
            var tTotalCost = 0.0;
            var totalBudgetExclueScm = 0.0;
            var tTotalBudget = 0.0;
            var tTotalDeviation = 0.0;

            var rowDepartmentWiseSummaryTableIndex = 1;
            CultureInfo cultures = new CultureInfo("en-US");

            foreach (var particularWiseSummary in partWiseSummaries)
            {

                var deviation = 0.0;
                var percentage = 0.0;
                if (particularWiseSummary.TotalCost > particularWiseSummary.TotalBudget)
                {
                    deviation = particularWiseSummary.TotalCost - particularWiseSummary.TotalBudget;
                    var divide = particularWiseSummary.TotalCost / particularWiseSummary.TotalBudget;
                    percentage = divide * 100;
                    tTotalDeviation += deviation;
                }
                tTotalCost += particularWiseSummary.TotalCost;
                tTotalBudget += particularWiseSummary.TotalBudget;





                itemParams.Add(
                    new ItemParam
                    {

                        Data = new List<TdParam>
                        {

                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + particularWiseSummary.ParticularName + "</span>"

                                },
                                new TdParam
                                {
                                    Style = @"color : #000000; ",
                                    Data = @"<span> " + particularWiseSummary.TotalBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</span>"

                                },
                                
                        }
                    });




            }

            itemParams.Add(
                new ItemParam
                {

                    Data = new List<TdParam>
                    {

                        new TdParam
                        {

                            Data = @"<span style='color: #23098B '>Grand Total</span>"

                        },
                        new TdParam
                        {

                            Data = @"<span style='color: #23098B '> "+
                            tTotalBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" + "</ span >"

                        },
                        //new TdParam
                        //{

                        //    Data = @"<span style='color: #23098B'>" +
                        //    tTotalCost.ToString() + "&#2547;"  + "</span>"

                        //},
                        //new TdParam
                        //{
                        //    Style = @"color : #000000; ",
                        //    Data = @"<span style='color: #23098B'>" + tTotalDeviation.ToString() + "&#2547;"  + "</span>"

                        //},
                        //new TdParam
                        //{  Style = @"color : #000000; ",
                        //    Data = @"<span></span>"

                        //}
                    }
                });

            return itemParams;
        }

        private List<ItemParam> getSettleItemData(int settlementId)
        {
            var itemParams = new List<ItemParam>();
            //var estimateSettleItems = _settlementItemService.getSettlementItemsBySettlementId(settlementId).Result;
            var sessionUser =  _sessionManager.GetUser().Result;
            var response = new List<EstimateSettleCompleteItem>();
            using var uow = _uowFactory.GetUnitOfWork();
            var estimateSettleItems =  uow.SettlementItemRepo.getSettlementItemsBySettlementId(sessionUser.Id,
                sessionUser.Department_Id, settlementId).Result;
            uow.Commit();

            foreach (var settleItem in estimateSettleItems)
            {
                if (settleItem.SettleItemId > 0)
                    itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {

                            new TdParam
                            {
                                ExtraColumnProperty = "width='30%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b> Particular: </b> " + settleItem.Particular , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item Category: </b>" + settleItem.ItemCategory , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item : </b>" + settleItem.ItemName , " padding-bottom: 2px;" , "") 
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item Code: </b>" + settleItem.ItemCode , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item Unit: </b>" + settleItem.ItemUnit , "" , "")


                            },
                            new TdParam
                            {
                                ExtraColumnProperty = "width='30%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b>No.Of Mach./Usages/ Team/Car: </b> " + (settleItem.NoOfMachineAndUsagesAndTeamAndCar ==-1 ? "N/A" : settleItem.NoOfMachineAndUsagesAndTeamAndCar.ToString() )  , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>No.Of Day/ Total Unit: </b>" +  (settleItem.NoOfDayAndTotalUnit ==-1 ? "N/A" : settleItem.NoOfDayAndTotalUnit.ToString() )  , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Req.Quantity: </b>" +  (settleItem.QuantityRequired ==-1 ? "N/A" : settleItem.QuantityRequired.ToString() ) , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Estimation Unit Price: </b>" + (settleItem.UnitPrice ==-1 ? "N/A" : settleItem.UnitPrice.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" )     , " padding-bottom: 2px;" , "") + @"<i class='fa-solid fa-bangladeshi-taka-sign'></i>"
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Total Estimation Price: </b>" + (settleItem.TotalPrice ==-1 ? "N/A" : settleItem.TotalPrice.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" ) , " " , "")
                            },
                            new TdParam
                            {
                                ExtraColumnProperty = "width='18%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b>District: </b> " + settleItem.DistrictName , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Area Type: </b>" + settleItem.AreaType , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Department: </b>" + settleItem.DepartmentName , " " , "")
                                       
                            },
                            new TdParam
                            {
                                ExtraColumnProperty = "width='28%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b> Settled Actual Quantity : </b> " + settleItem.SettleActualQuantity , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Already Settled : </b>" + settleItem.AlreadySettle.ToString("N", new CultureInfo("hi-IN"))   + "&#2547;", " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Actual.Quantity : </b>" + settleItem.ActualQuantity.ToString("N", new CultureInfo("hi-IN")) , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Actual  Unit Price : </b>" + settleItem.ActualUnitPrice.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;" , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Actual Total Price : </b>" + settleItem.ActualTotalPrice.ToString("N", new CultureInfo("hi-IN"))   + "&#2547;", "" , "")


                            },

                           
                            new TdParam
                            {
                                Data =  HtmlGenerate.HtmlGenerate.divWrappper(  settleItem.SettleItemRemarks , "" , "")
                            },
                            }
                        }
                    );

            }

            return itemParams;
        }

        private List<ItemParam> getEstimateLineItems(int estimationId,IUnitOfWork uow)
        {
            var itemParams = new List<ItemParam>();
            var estimateLineItems =
                uow.EstimateDetailsRepo.LoadEstimationDetailsWithOtherInformationsByEstimationId(estimationId).Result;


            foreach (var settleItem in estimateLineItems)
            {
             
                    itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {

                            new TdParam
                            {
                                ExtraColumnProperty = "width='30%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b> Particular: </b> " + settleItem.Particular , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item Category: </b>" + settleItem.ItemCategory , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item : </b>" + settleItem.ItemName , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item Code: </b>" + settleItem.ItemCode , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item Unit: </b>" + settleItem.ItemUnit , "" , "")


                            },
                            new TdParam
                            {
                                ExtraColumnProperty = "width='30%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b>No.Of Mach./Usages/ Team/Car: </b> " + settleItem.NoOfMachineAndUsagesAndTeamAndCar , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>No.Of Day/ Total Unit: </b>" + settleItem.NoOfDayAndTotalUnit , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Req.Quantity: </b>" + settleItem.QuantityRequired.ToString("N", new CultureInfo("hi-IN")) , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Estimation Unit Price: </b>" + settleItem.UnitPrice.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" , " padding-bottom: 2px;" , "") + @"<i class='fa-solid fa-bangladeshi-taka-sign'></i>"
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Total Estimation Price: </b>" + settleItem.TotalPrice.ToString("N", new CultureInfo("hi-IN")) + "&#2547;" , " " , "")
                            },
                            new TdParam
                            {
                                ExtraColumnProperty = "width='18%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b>District: </b> " + settleItem.DistrictName , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Area Type: </b>" + settleItem.AreaType , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Department: </b>" + settleItem.DepartmentName , " " , "")

                            },
                            


                            new TdParam
                            {
                                Data =  HtmlGenerate.HtmlGenerate.divWrappper(  settleItem.Remarks , "" , "")
                            },
                            }
                        }
                    );

            }

            return itemParams;
        }

        private List<ItemParam> getEstimateSettleItemSummaryData(int estimationId , IUnitOfWork uow)
        {
            var itemParams = new List<ItemParam>();
            //var estimateSettleItems = _settlementItemService.getSettlementItemsBySettlementId(settlementId).Result;
            var estimateSettleItems = uow.EstimateMemoEntityRepo
                .LoadEstimationSettleItemDetailsByEstimationId(estimationId).Result.ToList();


            foreach (var settleItem in estimateSettleItems)
            {

                itemParams.Add(
                    new ItemParam
                    {

                        Data = new List<TdParam>
                            {

                            new TdParam
                            {
                                ExtraColumnProperty = "width='30%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b> Particular: </b> " + settleItem.Particular , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item Category: </b>" + settleItem.ItemCategory , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item : </b>" + settleItem.ItemName , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item Code: </b>" + settleItem.ItemCode , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Item Unit: </b>" + settleItem.ItemUnit , "" , "")


                            },
                            new TdParam
                            {
                                ExtraColumnProperty = "width='30%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b>No.Of Mach./Usages/ Team/Car: </b> " + settleItem.NoOfMachineAndUsagesAndTeamAndCar , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>No.Of Day/ Total Unit: </b>" + settleItem.NoOfDayAndTotalUnit , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Req.Quantity: </b>" + settleItem.QuantityRequired.ToString("N", new CultureInfo("hi-IN")) , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Estimation Unit Price: </b>" + settleItem.UnitPrice.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;"  , " padding-bottom: 2px;" , "") + @"<i class='fa-solid fa-bangladeshi-taka-sign'></i>"
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Total Estimation Price: </b>" + settleItem.TotalPrice.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;" , " " , "")
                            },
                            new TdParam
                            {
                                ExtraColumnProperty = "width='18%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b>District: </b> " + settleItem.DistrictName , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Area Type: </b>" + settleItem.AreaType , " padding-bottom: 2px;" , "")
                                       + " </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Department: </b>" + settleItem.DepartmentName , " " , "")

                            },
                            new TdParam
                            {
                                ExtraColumnProperty = "width='28%'",
                                Data = HtmlGenerate.HtmlGenerate.divWrappper(@"<b> Settled Total Amount : </b> " + settleItem.SettleAmount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;" , "padding-top : 1px padding-bottom: 2px; ","" )
                                       +" </br>"
                                       + HtmlGenerate.HtmlGenerate.divWrappper(" <b>Already Settled Quantity : </b>" + settleItem.SettleQuantity.ToString("N", new CultureInfo("hi-IN")) , " padding-bottom: 2px;" , "")
                                      
                            


                            },


                            
                            }
                    
                    }
                );

            }

            return itemParams;
        }
        private List<ItemParam> getSettlementApproverFeedBackItemsItemParamsData(int settlementId)
        {
            var commentIcon = @"<i class=""fa fa-quote-left fa-2x fa-pull-left"" aria-hidden=""true""></i>";
            var itemParams = new List<ItemParam>();
            //var settlementApproverRemarks = _settlementService.LoadSettlementApproverRemarks(settlementId).Result;
            var uow = _uowFactory.GetUnitOfWork();
            var settlementApproverRemarks = uow.SettlementRepo.LoadSettlementApproverFeedBackDetailsBySettlementId(settlementId).Result;

            foreach (var settlementApproverRemark in settlementApproverRemarks)
            {
                var statusTag = "";
                if (settlementApproverRemark.SettlementStatus.ToString() == "-500")
                {
                    statusTag = @"<span class='btn btn-danger'>Rejected</span>";
                }
                else if (settlementApproverRemark.SettlementStatus.ToString() == "100")
                {
                    statusTag = @"<span class='btn btn-success'>Acknowledged</span>";
                }
                else if (settlementApproverRemark.SettlementStatus.ToString() == "-404")
                {
                    statusTag = @"<span class='btn btn-warning'>Rollbacked</span>";
                }
                else if (settlementApproverRemark.SettlementStatus.ToString() == "2")
                {
                    statusTag = @"<span class='btn btn-info'>Pending</span>";
                }
                if (settlementApproverRemark.FeedBack == null)
                    settlementApproverRemark.FeedBack = @"N/A";
                itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {
                                new TdParam
                                {

                                    Data = settlementApproverRemark.ApproverFullName
                                },
                                new TdParam
                                {
                                   Data = statusTag
                                },
                                new TdParam
                                {
                                    Data = settlementApproverRemark.FeedBackDate.ToString()
                                },
                                new TdParam
                                {
                                    Data = commentIcon + settlementApproverRemark.FeedBack
                                },

                            }
                        }
                    );

            }

            return itemParams;
        }
        private List<ItemParam> getMemoApproverFeedBackItemsItemParamsData(int memoId)
        {
            var commentIcon = @"<i class=""fa fa-quote-left fa-2x fa-pull-left"" aria-hidden=""true""></i>";
            var itemParams = new List<ItemParam>();
            //var settlementApproverRemarks = _settlementService.LoadSettlementApproverRemarks(settlementId).Result;
            var uow = _uowFactory.GetUnitOfWork();
            var settlementApproverRemarks = uow.MemoApproverRepo.LoadMemoApproverFeedBackDetails(memoId).Result;

            foreach (var settlementApproverRemark in settlementApproverRemarks)
            {
                var statusTag = "";
                if (settlementApproverRemark.ApproverFeedBackStatus.ToString() == "2")
                {
                    statusTag = @"<span style='border-radius: 5px; background: yellow; padding: 2px; color: #4B0082;'>Pending</span>";
                }
                else if (settlementApproverRemark.ApproverFeedBackStatus.ToString() == "-500")
                {
                    statusTag = @"<span style='border-radius: 5px; background: red; padding: 2px; color: white;'>Rejected</span>";
                }
                else if (settlementApproverRemark.ApproverFeedBackStatus.ToString() == "100")
                {
                    statusTag = @"<span style='border-radius: 5px; background: green; padding: 2px; color: white;'>Approved</span>";
                }
                else if (settlementApproverRemark.ApproverFeedBackStatus.ToString() == "-404")
                {
                    statusTag = @"<span style='border-radius: 5px; background: orange; padding: 2px; color: white;'>Rollbacked</span>";
                }
                if (settlementApproverRemark.FeedBack == null)
                    settlementApproverRemark.FeedBack = @"N/A";
                itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {
                                new TdParam
                                {

                                    Data = settlementApproverRemark.ApproverFullName
                                },
                                new TdParam
                                {
                                   Data = statusTag
                                },
                                new TdParam
                                {
                                    Data = settlementApproverRemark.FeedBackDate.ToString()
                                },
                                new TdParam
                                {
                                    Data = commentIcon + settlementApproverRemark.FeedBack
                                },

                            }
                        }
                    );

            }

            return itemParams;
        }
        private List<ItemParam> getEstimationApproversFeedbackParamData(int estimationId ,IUnitOfWork uow)
        {
            var commentIcon = @"<i class=""fa fa-quote-left fa-2x fa-pull-left"" aria-hidden=""true""></i>";
            var itemParams = new List<ItemParam>();
            //var settlementApproverRemarks = _settlementService.LoadSettlementApproverRemarks(settlementId).Result;

            var settlementApproverRemarks =
                 uow.EstimateApproverFeedbackRepo.LoadApproverFeedBackDetails(estimationId).Result;

            foreach (var settlementApproverRemark in settlementApproverRemarks)
            {
                var statusTag = "";
                if (settlementApproverRemark.EstimateStatus.ToString() == "2")
                {
                    statusTag = @"<span style='border-radius: 5px; background: yellow; padding: 2px; color: #4B0082;'>Pending</span>";
                }
                else if (settlementApproverRemark.EstimateStatus.ToString() == "-500")
                {
                    statusTag = @"<span style='border-radius: 5px; background: red; padding: 2px; color: white;'>Rejected</span>";
                }
                else if (settlementApproverRemark.EstimateStatus.ToString() == "100")
                {
                    statusTag = @"<span style='border-radius: 5px; background: green; padding: 2px; color: white;'>Approved</span>";
                }
                else if (settlementApproverRemark.EstimateStatus.ToString() == "-404")
                {
                    statusTag = @"<span style='border-radius: 5px; background: orange; padding: 2px; color: white;'>Rollbacked</span>";
                }
                if (settlementApproverRemark.FeedBack == null)
                    settlementApproverRemark.FeedBack = @"N/A";
                itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {
                                new TdParam
                                {

                                    Data = settlementApproverRemark.ApproverFullName
                                },
                                new TdParam
                                {
                                   Data = statusTag
                                },
                                new TdParam
                                {
                                    Data = settlementApproverRemark.FeedBackDate.ToString()
                                },
                                new TdParam
                                {
                                    Data = commentIcon + settlementApproverRemark.FeedBack
                                },

                            }
                        }
                    );

            }

            return itemParams;
        }

        private List<ItemParam> getSettlementApproverItemsItemParamsData(int settlementId)
        {
            var commentIcon = @"<i class=""fa fa-quote-left fa-2x fa-pull-left"" aria-hidden=""true""></i>";
            var itemParams = new List<ItemParam>();

            // var settlementApprovers = _settlementService.LoadSettlementApproverDetailsBySettlementId(settlementId).Result;
            using var uow = _uowFactory.GetUnitOfWork();

             var settlementApprovers =  uow.SettlementRepo.LoadSettlementApproverDetailsBySettlementId(settlementId).Result;


            foreach (var settlementApprover in settlementApprovers)
            {
                var statusTag = "";
                if (settlementApprover.ApproverStatus.ToString() == "2")
                {
                    statusTag = @"<span style='border-radius: 5px; background: yellow; padding: 2px; color: #4B0082;'>Pending</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "-500")
                {
                    statusTag = @"<span style='border-radius: 5px; background: red; padding: 2px; color: white;'>Rejected</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "100" && settlementApprover.ApproverRoleId == 3)
                {
                    statusTag = @"<span style='border-radius: 5px; background: #BB8FCE; padding: 2px; color: white;'>Informed</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "100")
                {
                    statusTag = @"<span style='border-radius: 5px; background: green; padding: 2px; color: white;'>Acknowledged</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "-404")
                {
                    statusTag = @"<span style='border-radius: 5px; background: orange; padding: 2px; color: white;'>Rollbacked</span>";
                }

                itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {
                                new TdParam
                                {

                                    Data = settlementApprover.ApproverFullName + ' ' + statusTag
                                },
                                new TdParam
                                {
                                    Data = settlementApprover.ApproverRoleName
                                },
                                new TdParam
                                {
                                   Data = settlementApprover.ApproverDepartment
                                },
                                new TdParam
                                {
                                    Data = settlementApprover.ApproverPlanDate.ToString()
                                },


                            }
                        }
                    );

            }

            return itemParams;
        }
        private List<ItemParam> getEstimationApproverItemsItemParamsData(int estimationId , IUnitOfWork uow)
        {
            var commentIcon = @"<i class=""fa fa-quote-left fa-2x fa-pull-left"" aria-hidden=""true""></i>";
            var itemParams = new List<ItemParam>();

            // var settlementApprovers = _settlementService.LoadSettlementApproverDetailsBySettlementId(settlementId).Result;
            //using var uow = _uowFactory.GetUnitOfWork();

            var settlementApprovers =
                uow.EstimateApproverRepo.LoadEstimateApproverDetailsByEstimationId(estimationId).Result;


            foreach (var settlementApprover in settlementApprovers)
            {
              
                var statusTag = "";
                if (settlementApprover.ApproverStatus.ToString() == "2")
                {
                    statusTag = @"<span style='border-radius: 5px; background: yellow; padding: 2px; color: #4B0082;'>Pending</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "-500")
                {
                    statusTag = @"<span style='border-radius: 5px; background: red; padding: 2px; color: white;'>Rejected</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "100" && settlementApprover.ApproverRoleId == 3)
                {
                    statusTag = @"<span style='border-radius: 5px; background: #BB8FCE; padding: 2px; color: white;'>Informed</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "100")
                {
                    statusTag = @"<span style='border-radius: 5px; background: green; padding: 2px; color: white;'>Approved</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "-404")
                {
                    statusTag = @"<span style='border-radius: 5px; background: orange; padding: 2px; color: white;'>Rollbacked</span>";
                }


                itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {
                                new TdParam
                                {

                                    Data = settlementApprover.ApproverFullName + ' ' + statusTag
                                },
                                new TdParam
                                {
                                    Data = settlementApprover.ApproverRoleName
                                },
                                new TdParam
                                {
                                   Data = settlementApprover.ApproverDepartment
                                },
                                new TdParam
                                {
                                    Data = settlementApprover.ApproverPlanDate.ToString()
                                },


                            }
                        }
                    );

            }

            return itemParams;
        }
        private List<ItemParam> getMemoApproverItemsItemParamsData(int memoId)
        {
            var commentIcon = @"<i class=""fa fa-quote-left fa-2x fa-pull-left"" aria-hidden=""true""></i>";
            var itemParams = new List<ItemParam>();

            // var settlementApprovers = _settlementService.LoadSettlementApproverDetailsBySettlementId(settlementId).Result;
            using var uow = _uowFactory.GetUnitOfWork();

            var settlementApprovers = uow.MemoApproverRepo.LoadMemoApproverDetailsByMemoId(memoId).Result;


            foreach (var settlementApprover in settlementApprovers)
            {
                var statusTag = "";
                if (settlementApprover.ApproverStatus.ToString() == "2")
                {
                    statusTag = @"<span style='border-radius: 5px; background: yellow; padding: 2px; color: #4B0082;'>Pending</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "-500")
                {
                    statusTag = @"<span style='border-radius: 5px; background: red; padding: 2px; color: white;'>Rejected</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "100" && settlementApprover.ApproverRoleId == 3)
                {
                    statusTag = @"<span style='border-radius: 5px; background: #BB8FCE; padding: 2px; color: white;'>Informed</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "100")
                {
                    statusTag = @"<span style='border-radius: 5px; background: green; padding: 2px; color: white;'>Approved</span>";
                }
                else if (settlementApprover.ApproverStatus.ToString() == "-404")
                {
                    statusTag = @"<span style='border-radius: 5px; background: orange; padding: 2px; color: white;'>Rollbacked</span>";
                }

                itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {
                                new TdParam
                                {

                                    Data = settlementApprover.ApproverFullName + ' ' + statusTag
                                },
                                new TdParam
                                {
                                    Data = settlementApprover.ApproverRoleName
                                },
                                new TdParam
                                {
                                   Data = settlementApprover.ApproverDepartment
                                },
                                new TdParam
                                {
                                    Data = settlementApprover.ApproverPlanDate.ToString()
                                },


                            }
                        }
                    );

            }

            return itemParams;
        }
        private List<ItemParam> getBudgetSettlementItemParamsData(int memoId)
        {
           
            var itemParams = new List<ItemParam>();

            
            using var uow = _uowFactory.GetUnitOfWork();

            var memo = uow.EstimateMemoEntityRepo.EstimationMemoInfoDetailsById(memoId).Result;

           

                  itemParams.Add(
                        new ItemParam
                        {

                            Data = new List<TdParam>
                            {
                                new TdParam
                                {

                                    Data = memo.EstimaionTotalPrice.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;"
                                 },
                                new TdParam
                                {
                                    Data = memo.AllowableBudget.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;"
                                },
                                new TdParam
                                {
                                   Data = memo.TotalCost.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;"
                                },
                                new TdParam
                                {
                                    Data = memo.Deviation.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;"
                                },
                                new TdParam
                                {
                                    Data = memo.Percentage.ToString("#.##") + "%"
                                }

                            }
                        }
                    );

            

            return itemParams;
        }

        public string getRejectFundRequisitionEmailBody(FundRequisitionVM fundRequisitionVm)
        {
            var emailFooterContent = new EmailFooterContent
            {
                bestRegards = "Best Regards",
                CompanyName = "Summit Communications Limited",
                CompanyAddress = "Summit Centre, Kawran Bazar, Dhaka-1215",
                FooterContent = @"This is an auto - generated mail,
                please do not reply"
            };
            string htmltable = "";
            htmltable += HtmlGenerate.HtmlGenerate.getBodyHeaderContent();
            htmltable += HtmlGenerate.HtmlGenerate.getBodyHeaderMessage("This is to inform you that following "+ fundRequisitionVm.RequisitionType.ToLower() == "Fund".ToLower()
                ? " Fund Requisition"   : "Payment Requisition " + " have been rejected by  " + fundRequisitionVm.FundRejectorName);
            htmltable += getDepartmentWiseSummaryWithCostBudgetFinalCardHtml(fundRequisitionVm.EstimatationId);
            htmltable += getPerticularWiseSummaryWithCostBudgetFinalCardHtml(fundRequisitionVm.EstimatationId);
            htmltable += getTotalFundRequisitionFinalCardHtml(fundRequisitionVm.EstimatationId);
            htmltable += getFundRequisitionRejectFinalCardHtml(fundRequisitionVm);
            htmltable += HtmlGenerate.HtmlGenerate.getBodyFooterContent(emailFooterContent);
            return htmltable;
        }
        public string getNewFundRequisitionEmailBody(FundRequisitionVM fundRequisitionVm)
        {
            var emailFooterContent = new EmailFooterContent
            {
                bestRegards = "Best Regards",
                CompanyName = "Summit Communications Limited",
                CompanyAddress = "Summit Centre, Kawran Bazar, Dhaka-1215",
                FooterContent = @"This is an auto - generated mail,
                please do not reply"
            };
            string htmltable = "";
            htmltable += HtmlGenerate.HtmlGenerate.getBodyHeaderContent();
            htmltable += HtmlGenerate.HtmlGenerate.getBodyHeaderMessage("This is to inform you that following Fund  has been requested. Please arrange disburse accordingly as per below information");
            htmltable += getDepartmentWiseSummaryWithCostBudgetFinalCardHtml(fundRequisitionVm.EstimatationId);
            htmltable += getPerticularWiseSummaryWithCostBudgetFinalCardHtml(fundRequisitionVm.EstimatationId);
            htmltable += getTotalFundRequisitionFinalCardHtml(fundRequisitionVm.EstimatationId);
            htmltable += getFundRequisitionFinalCardHtml(fundRequisitionVm);
            htmltable += HtmlGenerate.HtmlGenerate.getBodyFooterContent(emailFooterContent);
            return htmltable;
        }
        private string getFundRequisitionInfoHtmlTable(FundRequisitionVM fundRequisitionVm)
        {

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                //TableHeader = new List<string>
                //{
                //    "Approver Name", "Approver Type", "Approver Department", "Expected Date"
                //},
                items = new List<ItemParam>
                {

                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Estimation ID",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.EstimateIdentifier, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Fund Requestor Name :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.CreateorFullName, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Proposed Disburse Date",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.ProposedDisburseDate, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Requisition Type",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.RequisitionType, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Requisition Amount",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.Amount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Total Allowable Budget",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.AllowableBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Total Disburse Amount :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.AlreadyDisburseAmount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Total Requisition Amount(Except Rejected Amount) :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.FundRequested.ToString("N", new CultureInfo("hi-IN")) + "&#2547;", Style = ""}
                        }
                    },
                    //new ItemParam
                    //{
                    //    Style = "font-weight: bold;",
                    //    Data = new List<TdParam>
                    //    {
                    //        new TdParam
                    //        {
                    //            Data = "Remaining Budget :",
                    //            Style =
                    //                "padding:5px; background-color:#ededed;"
                    //        },
                    //        new TdParam {Data = fundRequisitionVm.RemainingBudget.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                    //    }
                    //}
                }


            });


            return htmltable;

        }
        private string getFundRequisitionRejectInfoHtmlTable(FundRequisitionVM fundRequisitionVm)
        {

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                //TableHeader = new List<string>
                //{
                //    "Approver Name", "Approver Type", "Approver Department", "Expected Date"
                //},
                items = new List<ItemParam>
                {

                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Estimation ID",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.EstimateIdentifier, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Fund Requestor Name :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.CreateorFullName, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Proposed Disburse Date",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.ProposedDisburseDate, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Requisition Type",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.RequisitionType, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Requisition Amount",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.Amount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Total Allowable Budget",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.AllowableBudget.ToString("N", new CultureInfo("hi-IN"))   + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Total Disburse Amount :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.AlreadyDisburseAmount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Total Requisition Amount(Except Reject) :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.FundRequested.ToString("N", new CultureInfo("hi-IN")) + "&#2547;", Style = ""}
                        }
                    },
                    //new ItemParam
                    //{
                    //    Style = "font-weight: bold;",
                    //    Data = new List<TdParam>
                    //    {
                    //        new TdParam
                    //        {
                    //            Data = "Remaining Budget :",
                    //            Style =
                    //                "padding:5px; background-color:#ededed;"
                    //        },
                    //        new TdParam {Data = fundRequisitionVm.RemainingBudget.ToString("N", new CultureInfo("hi-IN")) + "&#2547;", Style = ""}
                    //    }
                    //},
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Fund Rejected By :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundRequisitionVm.FundRejectorName, Style = ""}
                        }
                    }
                }


            });


            return htmltable;

        }
        private string getFundDisburseInfoHtmlTable(FundDisburseVM fundDisburseVm)
        {

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                //TableHeader = new List<string>
                //{
                //    "Approver Name", "Approver Type", "Approver Department", "Expected Date"
                //},
                items = new List<ItemParam>
                {

                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Estimation ID",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.EstimateIdentifier, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Fund Sender Name :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.FundSenderName, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Fund Available Date",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.FundAvailableDate.ToString(), Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Requisition Type",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.RequisitionType, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Requisition Amount",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.FundRequisitionAmount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Total AllowableBudget",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.AllowableBudget.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;" , Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Total Disburse Amount :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.AlreadyDisburseAmount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Total Requisition Amount :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.FundRequested.ToString("N", new CultureInfo("hi-IN")) + "&#2547;", Style = ""}
                        }
                    },
                    //new ItemParam
                    //{
                    //    Style = "font-weight: bold;",
                    //    Data = new List<TdParam>
                    //    {
                    //        new TdParam
                    //        {
                    //            Data = "Remaining Budget :",
                    //            Style =
                    //                "padding:5px; background-color:#ededed;"
                    //        },
                    //        new TdParam {Data = fundDisburseVm.RemainingBudget.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                    //    }
                    //},
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Disburse Amount :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.FundDisburseAmount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    }
                }


            });


            return htmltable;

        }

        public string getNewFundDisburseEmailBody(FundDisburseVM fundDisburseVm)
        {
            
            var emailFooterContent = new EmailFooterContent
            {
                bestRegards = "Best Regards",
                CompanyName = "Summit Communications Limited",
                CompanyAddress = "Summit Centre, Kawran Bazar, Dhaka-1215",
                FooterContent = @"This is an auto - generated mail,
                please do not reply"
            };
            string htmltable = "";
            htmltable += HtmlGenerate.HtmlGenerate.getBodyHeaderContent();
            htmltable += HtmlGenerate.HtmlGenerate.getBodyHeaderMessage("This is to inform you that following Fund has been disburse to the respective account. Please check and acknowledge accordingly to the system ");
            htmltable += getDepartmentWiseSummaryWithCostBudgetFinalCardHtml(fundDisburseVm.EstimatationId);
            htmltable += getPerticularWiseSummaryWithCostBudgetFinalCardHtml(fundDisburseVm.EstimatationId);
            htmltable += getTotalFundRequisitionFinalCardHtml(fundDisburseVm.EstimatationId);
            htmltable += getFundDisburseFinalCardHtml(fundDisburseVm);
            htmltable += HtmlGenerate.HtmlGenerate.getBodyFooterContent(emailFooterContent);
            return htmltable;
        }
        public string getNewFundReceivedOrRollbackEmailBody(FundDisburseVM fundDisburseVm)
        {
            var emailFooterContent = new EmailFooterContent
            {
                bestRegards = "Best Regards",
                CompanyName = "Summit Communications Limited",
                CompanyAddress = "Summit Centre, Kawran Bazar, Dhaka-1215",
                FooterContent = @"This is an auto - generated mail,
                please do not reply"
            };
            string StatusMessage = fundDisburseVm.DisburseStatus == 100
                ? "Received successfully By Fund requestor "
                : "Rollback to you. Please check the AMS System and update accordingly.";
            string message = "This is to inform you that following Fund  has been  " + StatusMessage;
            string htmltable = "";
            htmltable += HtmlGenerate.HtmlGenerate.getBodyHeaderContent();
            htmltable += HtmlGenerate.HtmlGenerate.getBodyHeaderMessage(message);
            htmltable += getDepartmentWiseSummaryWithCostBudgetFinalCardHtml(fundDisburseVm.EstimatationId);
            htmltable += getPerticularWiseSummaryWithCostBudgetFinalCardHtml(fundDisburseVm.EstimatationId);
            htmltable += getTotalFundRequisitionFinalCardHtml(fundDisburseVm.EstimatationId);
            htmltable += getFundDisburseReceivedOrRollbackFinalCardHtml(fundDisburseVm);
            htmltable += HtmlGenerate.HtmlGenerate.getBodyFooterContent(emailFooterContent);
            return htmltable;
        }

        private string getFundDisburseReceivedRollbackInfoHtmlTable(FundDisburseVM fundDisburseVm)
        {

            string htmltable = HtmlGenerate.HtmlGenerate.getHtmlTable(new HtmlTableParameter()
            {
                TableStyle = @"",
                ExtraTableProperty = @"class='table table-sm table-hover table-bordered' ",
                //TableHeader = new List<string>
                //{
                //    "Approver Name", "Approver Type", "Approver Department", "Expected Date"
                //},
                items = new List<ItemParam>
                {

                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Estimation ID",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.EstimateIdentifier, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Fund Sender Name :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.FundSenderName, Style = ""}
                        }
                    },
                 
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Requisition Type",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.RequisitionType, Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Requisition Amount",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.FundRequisitionAmount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "AllowableBudget",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.AllowableBudget.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Already Disburse Amount :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.AlreadyDisburseAmount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Already Requisition Amount :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.FundRequested.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Remaining Budget :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.RemainingBudget.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Disburse Amount :",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.FundDisburseAmount.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                        }
                    },

                    new ItemParam
                    {
                    Style = "font-weight: bold;",
                    Data = new List<TdParam>
                    {
                        new TdParam
                        {
                            Data = "Fund Receive Amount",
                            Style =
                                "padding:5px; background-color:#ededed;"
                        },
                        new TdParam {Data = fundDisburseVm.ReceivedAmountByRequestor.ToString("N", new CultureInfo("hi-IN"))  + "&#2547;", Style = ""}
                    }
                },
                    new ItemParam
                    {
                        Style = "font-weight: bold;",
                        Data = new List<TdParam>
                        {
                            new TdParam
                            {
                                Data = "Fund Receive Remakrs",
                                Style =
                                    "padding:5px; background-color:#ededed;"
                            },
                            new TdParam {Data = fundDisburseVm.RemarksByFundReceiver, Style = ""}
                        }
                    }
                }


            });


            return htmltable;

        }

        public async Task<string> getNewSettlementInitEmailBody(int settlementId, string message)
        {
            var emailFooterContent = new EmailFooterContent
            {
                bestRegards = "Best Regards",
                CompanyName = "Summit Communications Limited",
                CompanyAddress = "Summit Centre, Kawran Bazar, Dhaka-1215",
                FooterContent = @"This is an auto - generated mail,
                please do not reply"
            };

            string body = "";
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");
                var setttlement = await uow.SettlementRepo.getSettlementById(settlementId);
                body += HtmlGenerate.HtmlGenerate.getBodyHeaderContent();
                body += HtmlGenerate.HtmlGenerate.getBodyHeaderMessage(message);
                body += HtmlGenerate.HtmlGenerate.getBodyFooterContent(emailFooterContent);
            }

            return body;
        }
        public async Task<string> getMemoInitEmailBody(int memoId, string message)
        {
            var emailFooterContent = new EmailFooterContent
            {
                bestRegards = "Best Regards",
                CompanyName = "Summit Communications Limited",
                CompanyAddress = "Summit Centre, Kawran Bazar, Dhaka-1215",
                FooterContent = @"This is an auto - generated mail,please do not reply"
            };
            string body = "";
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");
                body += HtmlGenerate.HtmlGenerate.getBodyHeaderContent();
                body += HtmlGenerate.HtmlGenerate.getBodyHeaderMessage(message);
                body += HtmlGenerate.HtmlGenerate.getBodyFooterContent(emailFooterContent);
            }
            return body;
        }
        public string getSummitLogoTable()
       {
           return
               @"<table id='email-header' width='100%' align='center' border='0' cellspacing='0' cellpadding='0' style=''>
                        <tbody>
                        <tr>
                            <td>
                                <!-- SComm Logo-->
					            <table id='email-header-body' width='100%' align='center' border='0' cellspacing='0' cellpadding='0'>
						            <tbody>
							        <tr>
								        <td height='5' colspan='2' style='background-color:rgba(2,1,1,0.76)'></td>
							        </tr>
							        <tr>
								        <td id='top-left' width='50%' height='80' style='text-align:left;padding:10px 0 40px 20px'>
									        <img src='http://www.summitcommunications.net/images/logo.png' alt='Logo' border='0' width='200px' id='logo' class='CToWUd'>
									        </td>
									    <td id='top-right' width='50%' style='text-align:left;padding:0 30px 0 0'></td>
								    </tr>
							        </tbody>
						        </table>
                            </td>
				        </tr>
                        <tbody>
                    </table>";
       }
        public async Task<string> getMemoHtmlBodyWithEstimationForPdfReport(int memoId)
        {

            string body = "";
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");
                //var setttlement = _settlementService.getSettlementBySettlementId(settlementId).Result;


                var memo =  uow.EstimateMemoEntityRepo.GetEstimateMemoEntityById(memoId).Result;



                //body += GetHtmlEstimationPartForPDFReport(setttlement.EstimationId, uow, sessionUser).Result;
                body += getSummitLogoTable();
                body += "</br>" + @"<p></p>";
                body += " </br>" + getEstimationInfo(memo.EstimateId, uow);
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Line Items", getEstimatationLineItemsData(memo.EstimateId, uow));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Approvers", getEstimationApprovers(memo.EstimateId, uow));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Approvers Feedback", getEstimateApproverFeedBaCK(memo.EstimateId, uow));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Department Wise Summary", getDepartmentWiseSummaryByEstimateId(memo.EstimateId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Particular Wise Summary", getParticularWiseSummaryByEstimateId(memo.EstimateId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Fund Requisition & Disburse History", getTotalFundRequisitionDisburseHistory(memo.EstimateId));
                
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Total Deviation Amount(BDT)", "<span style='padding-left : 30px; font-weight : bold ; font-size : 25px;' >" +  memo.TotalDeviation.ToString("N", new CultureInfo("hi-IN")) + " &#2547;" + "</span>");
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Memo Justification", memo.Justification);
                
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Settlement Items Summary", this.getEstimateSettlementItemSummaryData(memo.EstimateId, uow));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Budget-Settlement Summary", this.getBudgetSettlementSummaryForMemo(memoId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Memo Approver", this.getMemoApprover(memoId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Memo Approver Feedback", this.getMemoApproverFeedBaCK(memoId));


            }

            return body;
        }
        public async Task<string> getNewEstimateInitEmailBody(int estimateId, string message)
        {
            var emailFooterContent = new EmailFooterContent
            {
                bestRegards = "Best Regards",
                CompanyName = "Summit Communications Limited",
                CompanyAddress = "Summit Centre, Kawran Bazar, Dhaka-1215",
                FooterContent = @"This is an auto - generated mail,
                please do not reply"
            };

            string body = "";
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");
                

                var estimation =  uow.EstimationRepo.SingleEstimationWithType(estimateId).Result;


                body += HtmlGenerate.HtmlGenerate.getBodyHeaderContent();
                body += HtmlGenerate.HtmlGenerate.getBodyHeaderMessage(message);
                body += "</br>" + @"<p></p>";
                body += " </br>" + getEstimationInfo(estimateId, uow);
                if (estimation.isLineItemAvaiable > 0)
                {
                    body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Line Items",
                        getEstimatationLineItemsData(estimateId, uow));
                    body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Department Wise Summary",
                        getDepartmentWiseSummaryByEstimateIdForEstimate(estimateId));
                    body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Particular Wise Summary",
                        getParticularWiseSummaryByEstimateIdForEstimate(estimateId));
                }

                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Approvers", getEstimationApprovers(estimateId, uow));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Approvers Feedback", getEstimateApproverFeedBaCK(estimateId, uow));
               
                body += HtmlGenerate.HtmlGenerate.getBodyFooterContent(emailFooterContent);

            }

            return body;
        }

        public async Task<string> getEstimateHtmlBodyWithEstimationForPdfReportV2(int estimationId)
        {

            string body = "";
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");
                


                



                
                body += getSummitLogoTable();
                body += "</br>" + @"<p></p>";
                body += " </br>" + getEstimationInfo(estimationId, uow);
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Line Items", getEstimatationLineItemsData(estimationId, uow));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Department Wise Summary", getDepartmentWiseSummaryByEstimateId(estimationId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Particular Wise Summary", getParticularWiseSummaryByEstimateId(estimationId));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Approvers", getEstimationApprovers(estimationId, uow));
                body += HtmlGenerate.HtmlGenerate.divWrappperForTableData("Estimate Approvers Feedback", getEstimateApproverFeedBaCK(estimationId, uow));
                
               
                


            }

            return body;
        }

        public async Task<string> getPasswordResetEmailBody(string password)
        {
            var emailFooterContent = new EmailFooterContent
            {
                bestRegards = "Best Regards",
                CompanyName = "Summit Communications Limited",
                CompanyAddress = "Summit Centre, Kawran Bazar, Dhaka-1215",
                FooterContent = @"This is an auto - generated mail,please do not reply"
            };

            string body = "";
            body += HtmlGenerate.HtmlGenerate.getBodyHeaderContent();
            body += "</br>" + @"<label>Your New Password is: </label>" + password;
            body += HtmlGenerate.HtmlGenerate.getBodyFooterContent(emailFooterContent);
            return body;
        }
    }
}

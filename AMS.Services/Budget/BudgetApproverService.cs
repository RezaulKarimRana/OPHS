using AMS.Common.Notifications;
using AMS.Infrastructure.Email;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate.BudgetApprover;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using AMS.Repositories.DatabaseRepos.ProcurementApproval.Models;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using AMS.Services.Managers.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AMS.Common.Constants;
using AMS.Services.Contracts;

namespace AMS.Services.Budget
{
    public class BudgetApproverService : IBudgetApproverService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IUserService _userService;
        private readonly ISessionManager _sessionManager;
        private readonly IHtmlGeneratorService _htmlGeneratorService;

        public BudgetApproverService(IUnitOfWorkFactory uowFactory, 
            ISessionManager sessionManager, IUserService userService , IHtmlGeneratorService htmlGeneratorService)
        {
            _uowFactory = uowFactory;
            _userService = userService;
            _sessionManager = sessionManager;
            _htmlGeneratorService = htmlGeneratorService;
        }

        public async Task<int> CreateApproverFeedBack(CreateApproverFeedBackServiceRequest request)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                    throw new Exception("user session expired");

                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.EstimateApproverFeedbackRepo.CreateEstimateApproverFeedback(new Repositories.DatabaseRepos.EstimateApproverFeedbackRepo.Models.CreateApproverFeedbackRequest()
                {
                    EstimateApprover_Id = request.EstimateApprover_Id,
                    Estimation_Id = request.Estimation_Id,
                    FeedbackRemarks = request.FeedbackRemarks,
                    Status = request.Status,
                    Created_By = sessionUser.Id
                });

                var estimate = await uow.EstimationRepo.GetById(request.Estimation_Id);
                if (estimate == null)
                    throw new Exception("Estimate Not found");

                

                uow.Commit();
                using (var uowT = _uowFactory.GetUnitOfWork())
                {
                    if (estimate.Status == BaseEntity.EntityStatus.Pending.ToString() ||
                        estimate.Status == BaseEntity.EntityStatus.CR.ToString() ||
                        estimate.Status == BaseEntity.EntityStatus.Reject.ToString())
                    {
                        await this.SaveEmailOnDbForApproval(request.Estimation_Id, estimate.Status, uowT, sessionUser);
                    }
                    else if (estimate.Status == BaseEntity.EntityStatus.Completed.ToString())
                    {
                        await SaveEmailOnDbForCompletedApproval(request.Estimation_Id, uowT, sessionUser);
                    }


                    uowT.Commit();

                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task DeleteAllApproversByEstimateId(int EstimateId)
        {
            try
            {
                var sessionUser = _sessionManager.GetUser();
                if (sessionUser == null)
                    throw new Exception("user session expired");

                using var uow = _uowFactory.GetUnitOfWork();
                await uow.EstimateApproverRepo.DeleteApproverByEstimate(EstimateId);
                uow.Commit();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<EstimateApproverEntity> GetApproverByEstimateANDUser(GetApproverByEstimateAndUser request)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                    throw new Exception("user session expired");

                if (sessionUser.Id != request.UserId)
                    throw new Exception("Invaild user request");

                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.EstimateApproverRepo.GetApproverByEstimateIdAndUserId(request.EstimationId, request.UserId);
                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<EstimateApproverFeedbackEntity> GetFeedbackByEstimationandUserId(int estimate_Id, int userId, int completed)
        {
            try
            {
                EstimateApproverFeedbackEntity response = null;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimateApproverFeedbackRepo.GetFeedbackByEstimationandUserId(estimate_Id, userId, completed);
                }
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<EstimateApproverEntity>> GetLatestPendingApprovers(int estimationId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                    throw new Exception("user session expired");

                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.EstimateApproverRepo.GetLatestPendingApproveresOfAEstimation(estimationId);
                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> GetUpcomingPendingApproverLevel(int estimateId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var level = await uow.EstimateApproverRepo.GetLatestPendingApproverLevel(estimateId);

                return level;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<LoadApproverFeedBackDetails>> LoadApproverRemarksService(int estimationId)
        {
            try
            {
                var sessionUser = _sessionManager.GetUser();
                if (sessionUser == null)
                    throw new Exception("user session expired");

                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.EstimateApproverFeedbackRepo.LoadApproverFeedBackDetails(estimationId);

                return response;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<UpdateApproverStatusResponse> UpdateEstimateApproverStatusById(int id, string status, string remarks)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                    throw new Exception("user session expired");
                var response = new UpdateApproverStatusResponse();

                using var uow = _uowFactory.GetUnitOfWork();
                await uow.EstimateApproverRepo.UpdateApproverStatus(

                    new Repositories.DatabaseRepos.EstimateApproverRepo.Models.UpdateApproverStatusRequest()
                    {
                        status = status,
                        remarks = remarks,
                        id = id,
                        updatedby = sessionUser.Id
                    }
                 );
                uow.Commit();

                response.Notifications.Add($"Approver status has been updated", NotificationTypeEnum.Success);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UpdateApproverStatusResponse> FinalApproverConvertToInformer(int estimateId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                    throw new Exception("user session expired");
                var response = new UpdateApproverStatusResponse();

                using var uow = _uowFactory.GetUnitOfWork();
                await uow.EstimateApproverRepo.FinalApproverConvertToInformer(estimateId);
                uow.Commit();

                response.Notifications.Add($"Final Approver Convert to informer", NotificationTypeEnum.Success);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task SaveEmailOnDbForApproval(int estimationId, string estimateStatus, IUnitOfWork uow, UserEntity sessionUser)
        {
            try
            {
                var estimateInfo = await uow.EstimationRepo.SingleEstimationWithType(estimationId);
                var precurementApproval = await uow.ProcurementApprovalRepo.GetProcurementApprovalByEstimateId(estimationId);
                var estimateItemDetails = await uow.EstimateDetailsRepo.LoadEstimationDetailsWithOtherInformationsByEstimationId(estimationId);
                var estimateDeptSummaryList = await uow.DepartmentWiseSummaryRepo.LoadDepartmentWiseSummaryDetailsByEstimationId(estimationId);
                var estimatePartiSummaryList = await uow.ParticularWiseSummaryRepo.LoadParticularWiseSummaryDetailsByEstimationId(estimationId);
                var estimateApproverList = await uow.EstimateApproverRepo.LoadEstimateApproverDetailsByEstimationId(estimationId);
                var estimateApproverFeedBackList = await uow.EstimateApproverFeedbackRepo.LoadApproverFeedBackDetails(estimationId);

                if (estimateInfo.EstimationStatus == BaseEntity.EntityStatus.Completed.ToString())
                    return;

                var lowestPriority = 0;
                var lastCompletedApproverPriority = 150;
                foreach (var item in estimateApproverList)
                {
                    if (item.ApproverPriority > lowestPriority && item.ApproverStatus == BaseEntity.EntityStatus.Pending.ToString() && item.ApproverPriority != 400)
                        lowestPriority = item.ApproverPriority;
                }
                foreach (var item in estimateApproverList)
                {
                    if (item.ApproverId == sessionUser.Id && item.ApproverStatus == BaseEntity.EntityStatus.Completed.ToString() && item.ApproverPriority < lastCompletedApproverPriority)
                    {
                        lastCompletedApproverPriority = item.ApproverPriority;
                    }
                }
                if (lowestPriority == lastCompletedApproverPriority && estimateInfo.EstimationStatus == EstimationEntity.EntityStatus.Pending.ToString())
                {
                    return;
                }

                var estimationStackHolders = await this.GetLowestPendingApproversForRunningApproval(estimateApproverList, sessionUser, lowestPriority);

                var requestTOSave = new CreateEmailContantRequest();
                string message = "";
                if (estimateStatus == BaseEntity.EntityStatus.CR.ToString())
                {
                    message = "This is inform you that , Following "+ EstimationType.GetEstimationTypeByTypeId(estimateInfo.EstimationTypeId) +" has been rollback to the initiator.";
                    requestTOSave.ToEmail = estimateInfo.CreatorEmail;
                    requestTOSave.ToCc = sessionUser.Email_Address; // estimationStackHolders[0] + "," + estimationStackHolders[2];
                    requestTOSave.Subject = "AMS- Approval has Change Request [ "+ estimateInfo.EstimationIdentifier +"] :" + estimateInfo.EstimationSubject;
                    requestTOSave.Body = await _htmlGeneratorService.getNewEstimateInitEmailBody(estimationId, message);
                }
                else if (estimateStatus == BaseEntity.EntityStatus.Reject.ToString())
                {
                    message = "This is inform you that , Following "+ EstimationType.GetEstimationTypeByTypeId(estimateInfo.EstimationTypeId) + " has been rejected by " +
                              sessionUser.First_Name + " "
                              + sessionUser.Last_Name + ".";
                    requestTOSave.ToEmail = estimateInfo.CreatorEmail;
                    requestTOSave.ToCc = sessionUser.Email_Address; // estimationStackHolders[0] + "," + estimationStackHolders[2];
                    requestTOSave.Subject = "AMS- Approval has been Rejected [ " + estimateInfo.EstimationIdentifier + "] :"+ estimateInfo.EstimationSubject;
                    requestTOSave.Body = await _htmlGeneratorService.getNewEstimateInitEmailBody(estimationId, message); 
                }
                else if (estimateStatus == BaseEntity.EntityStatus.Pending.ToString())
                {
                    message = "This is inform you that , Following "+ EstimationType.GetEstimationTypeByTypeId(estimateInfo.EstimationTypeId) + " has been approved by " +
                              sessionUser.First_Name + " "
                              + sessionUser.Last_Name + ". Please check and update the system accordingly.";
                    requestTOSave.ToEmail = estimationStackHolders[0];
                    requestTOSave.ToCc ="" + sessionUser.Email_Address + "," + estimateInfo.CreatorEmail /*+ "," + estimationStackHolders[1]*/;
                    requestTOSave.Subject = "AMS- In-Process Approval [ " + estimateInfo.EstimationIdentifier + "] :"  + estimateInfo.EstimationSubject;
                    requestTOSave.Body = await _htmlGeneratorService.getNewEstimateInitEmailBody(estimationId, message);
                }
                requestTOSave.CreatedBy = sessionUser.Id;
                requestTOSave.ModifiedBy = sessionUser.Id;
                requestTOSave.AMSID = estimateInfo.EstimationIdentifier;
                requestTOSave.Department = sessionUser.Department_Id;

                var response = await uow.EmailRepo.SaveForEmailServer(requestTOSave);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task<List<string>> GetLowestPendingApproversForRunningApproval(List<EstimateApproverByEstimateId> approverList, UserEntity sessionUser, int lowestPriority)
        {
            var responseList = new List<string?>();
            var response = "";
            var responseToCC = "";
            var responseApprover = "";

            foreach (var item in approverList)
            {
                using var uowUnder = _uowFactory.GetUnitOfWork();
                var user = await uowUnder.UserRepo.GetUserById(new Repositories.DatabaseRepos.UserRepo.Models.GetUserByIdRequest() { Id = item.ApproverId });
                if (item.ApproverPriority == lowestPriority)
                {
                    if (response == "")
                        response += user.Email_Address;

                    else
                    {
                        if (!String.IsNullOrEmpty(user.Email_Address))
                        {
                            response = response + "," + user.Email_Address;
                        }
                    }
                }
                else
                {
                    if (responseToCC == "")
                        responseToCC = user.Email_Address;
                    else
                    {
                        if (!String.IsNullOrEmpty(user.Email_Address))
                        {
                            responseToCC = responseToCC + "," + user.Email_Address;
                        }
                    }
                    //if (item.ApproverPriority == 1)
                    //{
                    //    if(responseToCC == "")
                    //        responseToCC = "asif.ahmed@summitcommunications.net";//"sayed.nazmul@summitcommunications.net";
                    //    else
                    //        responseToCC = responseToCC + "," + "asif.ahmed@summitcommunications.net";//"sayed.nazmul@summitcommunications.net";
                    //}
                    //else
                    //{
                    //    if (responseToCC == "")
                    //        responseToCC += user.Email_Address;

                    //    else
                    //        responseToCC = responseToCC + "," + user.Email_Address;
                    //}
                }

                if (item.ApproverStatus == "100" || item.ApproverStatus == "-404" || item.ApproverStatus == "-500")
                {
                    if (responseApprover == "")
                        responseApprover = user.Email_Address;
                    else
                    {
                        if (!String.IsNullOrEmpty(user.Email_Address))
                        {
                            responseApprover = responseApprover + "," + user.Email_Address;
                        }
                    }
                        
                }
            }
            responseList.Add(response);
            responseList.Add(responseToCC);
            responseList.Add(responseApprover);
            return responseList;
        }

        private async Task<string> GetEmailBodyForOnGoingApproval(EstimationWithEstimationType estimation, List<EstimationDetailsWithJoiningOtherTables> itemDetailList,
            List<DepartWiseSummaryDetailsByEstimationId> deptSummmary, List<ParticularWiseSummaryDetailsByEstimationId> partiSummary, List<EstimateApproverByEstimateId> approverList,
            List<LoadApproverFeedBackDetails> approverFeedbackList, string estimateStatus, GetProcurementApprovalResponse procurement)
        {
            try
            {
                #region Variables
                string body = "";
                string procurementApprovalPart = "";
                string itemDetails = "";
                string itemDetailsSingleRow = "";
                string itemDetailsTable = "";
                string itemApprovers = "";
                string itemApproversInSingleRow = "";
                string deptSummaryInSingleRow = "";
                string deptSummaryRows = "";
                string partiSummaryInSingleRow = "";
                string partiSummaryRows = "";
                string itemApproversfeedback = "";
                string itemApproversfeedbackInSingleRow = "";
                #endregion

                #region NEW MAIL HTML BODY
                //adding itemDetailsBodyPart on EmailBody
                if (IsEmpty(itemDetailList))
                {
                    itemDetailsTable = "";
                }
                else
                {
                    #region BuildingItemDetailsHTML
                    foreach (var item in itemDetailList)
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
                                + item.DepartmentName +
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
                                            <td colspan='6'><strong>" + estimation.EstimaionTotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")) + @"</strong></td>
                                    </tr></tfoot>
                                </table>
                            </td>
                        </tr>";
                    #endregion
                }

                //adding Procurement Part if available
                if (estimation.EstimationTypeId == 7 && procurement != null)
                {
                    #region ProcurementApproval
                    procurementApprovalPart =
                        @"<tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>PA. Reference No.</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.PAReferenceNo + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Title of the PR/RFQ</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.TitleOfPRorRFQ + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>RFQ Reference No.</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.RFQReferenceNo + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>PR Reference No.</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.PRReferenceNo + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Name and Cell No. of Requester</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.NameOfRequester + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Department/Division</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.DepartmentName + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>RFQ Process</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.RFQProcess + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Sourcing Method</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.SourcingMethod + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Name of the Recommended Supplied</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.NameOfRecommendedSupplier + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Purchase Value</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.PurchaseValue + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Saving Amount</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.SavingAmount + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Saving Type</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.SavingType + @"</td>
                    </tr>";
                    #endregion
                }

                //adding departmentSummaryPart in EmailBody
                #region BuildingDeptSummary 
                if (IsEmpty(deptSummmary))
                {
                    deptSummaryRows = "";
                }
                else
                {
                    foreach (var item in deptSummmary)
                    {
                        deptSummaryInSingleRow = @"<tr align='center'><td>"
                            + item.DepartmentName
                            + "</td><td>"
                            + item.Price.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))
                            + "</td></tr>";
                        deptSummaryRows = deptSummaryRows + deptSummaryInSingleRow;
                    }
                }

                #endregion

                //adding particularSummaryPart in EmailBody
                #region BuildingPartiSummary 
                if (IsEmpty(partiSummary))
                {
                    partiSummaryRows = "";
                }
                else
                {
                    foreach (var item in partiSummary)
                    {
                        partiSummaryInSingleRow = @"<tr align='center'><td>"
                            + item.ParticularName
                            + "</td><td>"
                            + item.Price.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))
                            + "</td></tr>";
                        partiSummaryRows = partiSummaryRows + partiSummaryInSingleRow;
                    }
                }
                #endregion

                ///adding approverListPart in EmailBody
                #region BuildingApproverListHTML
                var userWithDeptInfo = await _userService.GetUserAndDepartmentByIdService(estimation.CreatorID);

                itemApprovers = @"<tr align='center' style='background-color:khaki;'><td>" + userWithDeptInfo.First_Name + " " + userWithDeptInfo.Last_Name + "</td><td>Creator</td><td>" + userWithDeptInfo.DepartmentName + "</td><td></td></tr>";
                foreach (var item in approverList)
                {
                    itemApproversInSingleRow = @"<tr align='center'><td>" + item.ApproverFullName + "</td><td>" + item.ApproverRoleName + "</td><td>" + item.ApproverDepartment + "</td><td>" + item.ApproverPlanDate + "</td></tr>";
                    itemApprovers = itemApprovers + itemApproversInSingleRow;
                }
                #endregion
                #endregion

                var approverStatusDetail = "";
                //adding approverFeedbackList in EmailBody
                #region BuildingApproverFeebBackListHTML
                foreach (var item in approverFeedbackList)
                {
                    if (item.EstimateStatus == BaseEntity.EntityStatus.Completed)
                        approverStatusDetail = "Completed";
                    else if (item.EstimateStatus == BaseEntity.EntityStatus.Reject)
                        approverStatusDetail = "Rejected";
                    else if (item.EstimateStatus == BaseEntity.EntityStatus.CR)
                        approverStatusDetail = "Rollbacked";
                    if (item.EstimateStatus == BaseEntity.EntityStatus.Pending)
                        approverStatusDetail = "Pending";
                    itemApproversfeedbackInSingleRow =
                        @"<tr align='center'><td>" + item.ApproverFullName + "</td><td>" + approverStatusDetail + "</td><td>" + item.FeedBackDate + "</td><td>" + item.FeedBack + "</td></tr>";
                    itemApproversfeedback = itemApproversfeedback + itemApproversfeedbackInSingleRow;
                }
                #endregion

                string estimateType = "";
                if (estimation.EstimationTypeId == 2)
                {
                    estimateType = "New Budget Estimate";
                }
                else if (estimation.EstimationTypeId == 3)
                {
                    estimateType = "Memo";
                }
                else if (estimation.EstimationTypeId == 7)
                {
                    estimateType = "Procurement Approval";
                }

                //    string upperBodyPart, string estimateType, string Identification, string subject, string objective, string details, string fromTime, string toTime, string itemDetailsBodyPart,
                //string totalPrice, string departmentSummaryPart, string particularSummaryPart, string approverListPart, string approverFeedBacks

                //string upperBodyPart = @"<p>A <b>" + estimateType + @"</b> has been completed.Please see the below information.</p>";
                var upperBodyPart = "";
                if (estimateStatus == BaseEntity.EntityStatus.Pending.ToString())
                {
                    upperBodyPart = @"<p>A <b>" + estimateType + @"</b> has been initiated for approval which was initiated by " + userWithDeptInfo.First_Name + " " + userWithDeptInfo.Last_Name + "/" + userWithDeptInfo.DepartmentName + ". Now your approval is needed to process .Please see the below information.</p>";
                }
                else if (estimateStatus == BaseEntity.EntityStatus.CR.ToString())
                {
                    upperBodyPart = @"<p>A <b>" + estimateType + @"</b> has been requested for changes which was initiated by you.Please see the below information.</p>";
                }
                else if (estimateStatus == BaseEntity.EntityStatus.Reject.ToString())
                {
                    upperBodyPart = @"<p>A <b>" + estimateType + @"</b> has been rejected which was initiated by you.Please see the below information.</p>";
                }

                body = GetEmailBodyForCreatingApproval.GenericEmailBody(upperBodyPart, estimateType, estimation.EstimationIdentifier, estimation.EstimationSubject, estimation.EstimationObjective,
                    estimation.EstimationDetails, estimation.EstimationPlanStartDate.ToString("MM/dd/yyyy"), estimation.EstimationPlanEndDate.ToString("MM/dd/yyyy"),
                    itemDetailsTable, estimation.EstimaionTotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")), deptSummaryRows, partiSummaryRows, itemApprovers, itemApproversfeedback, procurementApprovalPart);
                return body;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SaveEmailOnDbForCompletedApproval(int estimationId, IUnitOfWork uow, UserEntity sessionUser)
        {
            try
            {
                var estimateInfo = await uow.EstimationRepo.SingleEstimationWithType(estimationId);
                var precurementApproval = await uow.ProcurementApprovalRepo.GetProcurementApprovalByEstimateId(estimationId);
                var estimateItemDetails = await uow.EstimateDetailsRepo.LoadEstimationDetailsWithOtherInformationsByEstimationId(estimationId);
                var estimateDeptSummaryList = await uow.DepartmentWiseSummaryRepo.LoadDepartmentWiseSummaryDetailsByEstimationId(estimationId);
                var estimatePartiSummaryList = await uow.ParticularWiseSummaryRepo.LoadParticularWiseSummaryDetailsByEstimationId(estimationId);
                var estimateApproverList = await uow.EstimateApproverRepo.LoadEstimateApproverDetailsByEstimationId(estimationId);
                var estimateApproverFeedBackList = await uow.EstimateApproverFeedbackRepo.LoadApproverFeedBackDetails(estimationId);
                string message = "This is infom you that , Following "+ estimateInfo.EstimationTypeName +" has been approve Completed by " +
                                 sessionUser.First_Name + " " + sessionUser.Last_Name + " .";
                var requestTOSave = new CreateEmailContantRequest();
                requestTOSave.ToEmail = estimateInfo.CreatorEmail;
                requestTOSave.ToCc = "" + sessionUser.Email_Address + "," + await this.GetAllApproversMail(estimateApproverList);
               // requestTOSave.ToCc = "devteam.it@summitcommunications.net" + await this.GetAllApproversMail(estimateApproverList);
                requestTOSave.Subject = "AMS- Estimate Approval Completion [ "+ estimateInfo.EstimationIdentifier +"] : " + estimateInfo.EstimationSubject;
                //requestTOSave.Body = await this.GetEmailBodyForCompletedApproval(estimateInfo, estimateItemDetails, estimateDeptSummaryList, estimatePartiSummaryList, estimateApproverList, estimateApproverFeedBackList, precurementApproval);
                requestTOSave.Body = await _htmlGeneratorService.getNewEstimateInitEmailBody(estimationId, message);
                requestTOSave.CreatedBy = sessionUser.Id;
                requestTOSave.ModifiedBy = sessionUser.Id;
                requestTOSave.AMSID = estimateInfo.EstimationIdentifier;
                requestTOSave.Department = sessionUser.Department_Id;

                var response = await uow.EmailRepo.SaveForEmailServer(requestTOSave);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> GetAllApproversMail(List<EstimateApproverByEstimateId> approverList)
        {
            try
            {
                var approvers = approverList.GroupBy(x => x.ApproverEmail).Select(y => y.First());
                var allApproversMail = "";
                foreach (var item in approvers)
                {
                    if (item.ApproverPriority == 1)
                    {
                        allApproversMail =
                            allApproversMail; // "," + "asif.ahmed@summitcommunications.net","sayed.nazmul@summitcommunications.net";
                    }
                    else
                    {
                        if (allApproversMail == "")
                            allApproversMail += item.ApproverEmail;

                        else
                            allApproversMail = allApproversMail + "," + item.ApproverEmail;
                    }
                }

                return allApproversMail;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<string> GetEmailBodyForCompletedApproval(EstimationWithEstimationType estimation, List<EstimationDetailsWithJoiningOtherTables> itemDetailList,
            List<DepartWiseSummaryDetailsByEstimationId> deptSummmary, List<ParticularWiseSummaryDetailsByEstimationId> partiSummary, List<EstimateApproverByEstimateId> approverList,
            List<LoadApproverFeedBackDetails> approverFeedbackList, GetProcurementApprovalResponse procurement)
        {
            try
            {
                #region Variables
                string body = "";
                string procurementApprovalPart = "";
                string itemDetails = "";
                string itemDetailsSingleRow = "";
                string itemDetailsTable = "";
                string itemApprovers = "";
                string itemApproversInSingleRow = "";
                string deptSummaryInSingleRow = "";
                string deptSummaryRows = "";
                string partiSummaryInSingleRow = "";
                string partiSummaryRows = "";
                string itemApproversfeedback = "";
                string itemApproversfeedbackInSingleRow = "";
                #endregion

                #region NEW MAIL HTML BODY
                //adding itemDetailsBodyPart on EmailBody
                if (IsEmpty(itemDetailList))
                {
                    itemDetailsTable = "";
                }
                else
                {
                    #region BuildingItemDetailsHTML
                    foreach (var item in itemDetailList)
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
                                + item.DepartmentName +
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
                                            <td colspan='6'><strong>" + estimation.EstimaionTotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")) + @"</strong></td>
                                    </tr></tfoot>
                                </table>
                            </td>
                        </tr>";
                    #endregion
                }

                //adding Procurement Part if available
                if (estimation.EstimationTypeId == 7 && procurement != null)
                {
                    #region ProcurementApproval
                    procurementApprovalPart =
                        @"<tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>PA. Reference No.</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.PAReferenceNo + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Title of the PR/RFQ</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.TitleOfPRorRFQ + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>RFQ Reference No.</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.RFQReferenceNo + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>PR Reference No.</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.PRReferenceNo + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Name and Cell No. of Requester</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.NameOfRequester + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Department/Division</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.DepartmentName + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>RFQ Process</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.RFQProcess + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Sourcing Method</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.SourcingMethod + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Name of the Recommended Supplied</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.NameOfRecommendedSupplier + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Purchase Value</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.PurchaseValue + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Saving Amount</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.SavingAmount + @"</td>
                    </tr>
                    <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Saving Type</b></td>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + procurement.SavingType + @"</td>
                    </tr>";
                    #endregion
                }

                //adding departmentSummaryPart in EmailBody
                #region BuildingDeptSummary 
                if (IsEmpty(deptSummmary))
                {
                    deptSummaryRows = "";
                }
                else
                {
                    foreach (var item in deptSummmary)
                    {
                        deptSummaryInSingleRow = @"<tr align='center'><td>"
                            + item.DepartmentName
                            + "</td><td>"
                            + item.Price.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))
                            + "</td></tr>";
                        deptSummaryRows = deptSummaryRows + deptSummaryInSingleRow;
                    }
                }

                #endregion

                //adding particularSummaryPart in EmailBody
                #region BuildingPartiSummary 
                if (IsEmpty(partiSummary))
                {
                    partiSummaryRows = "";
                }
                else
                {
                    foreach (var item in partiSummary)
                    {
                        partiSummaryInSingleRow = @"<tr align='center'><td>"
                            + item.ParticularName
                            + "</td><td>"
                            + item.Price.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD"))
                            + "</td></tr>";
                        partiSummaryRows = partiSummaryRows + partiSummaryInSingleRow;
                    }
                }
                #endregion

                ///adding approverListPart in EmailBody
                #region BuildingApproverListHTML
                var userWithDeptInfo = await _userService.GetUserAndDepartmentByIdService(estimation.CreatorID);

                itemApprovers = @"<tr align='center' style='background-color:khaki;'><td>" + userWithDeptInfo.First_Name + " " + userWithDeptInfo.Last_Name + "</td><td>Creator</td><td>" + userWithDeptInfo.DepartmentName + "</td><td></td></tr>";
                foreach (var item in approverList)
                {
                    itemApproversInSingleRow = @"<tr align='center'><td>" + item.ApproverFullName + "</td><td>" + item.ApproverPriority + "</td><td>" + item.ApproverDepartment + "</td><td>" + item.ApproverPlanDate + "</td></tr>";
                    itemApprovers = itemApprovers + itemApproversInSingleRow;
                }
                #endregion
                #endregion

                var approverStatusDetail = "";
                //adding approverFeedbackList in EmailBody
                #region BuildingApproverFeebBackListHTML
                foreach (var item in approverFeedbackList)
                {
                    if (item.EstimateStatus == BaseEntity.EntityStatus.Completed)
                        approverStatusDetail = "Completed";
                    else if (item.EstimateStatus == BaseEntity.EntityStatus.Reject)
                        approverStatusDetail = "Rejected";
                    else if (item.EstimateStatus == BaseEntity.EntityStatus.CR)
                        approverStatusDetail = "Rollbacked";
                    if (item.EstimateStatus == BaseEntity.EntityStatus.Pending)
                        approverStatusDetail = "Pending";
                    itemApproversfeedbackInSingleRow =
                        @"<tr align='center'><td>" + item.ApproverFullName + "</td><td>" + approverStatusDetail + "</td><td>" + item.FeedBackDate + "</td><td>" + item.FeedBack + "</td></tr>";
                    itemApproversfeedback = itemApproversfeedback + itemApproversfeedbackInSingleRow;
                }
                #endregion

                string estimateType = "";
                if (estimation.EstimationTypeId == 2)
                {
                    estimateType = "New Budget Estimate";
                }
                else if (estimation.EstimationTypeId == 3)
                {
                    estimateType = "Memo";
                }
                else if (estimation.EstimationTypeId == 7)
                {
                    estimateType = "Procurement Approval";
                }

                string upperBodyPart = @"<p>A <b>" + estimateType + @"</b> has been completed which was initiated by " + userWithDeptInfo.First_Name + " " + userWithDeptInfo.Last_Name + "/" + userWithDeptInfo.DepartmentName + ".Please see the below information.</p>";

                body = GetEmailBodyForCreatingApproval.GenericEmailBody(upperBodyPart, estimateType, estimation.EstimationIdentifier, estimation.EstimationSubject, estimation.EstimationObjective,
                    estimation.EstimationDetails, estimation.EstimationPlanStartDate.ToString("MM/dd/yyyy"), estimation.EstimationPlanEndDate.ToString("MM/dd/yyyy"),
                    itemDetailsTable, estimation.EstimaionTotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")), deptSummaryRows, partiSummaryRows, itemApprovers, itemApproversfeedback, procurementApprovalPart);
                return body;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static bool IsEmpty<T>(List<T> list)
        {
            if (list == null)
            {
                return true;
            }

            return !list.Any();
        }
    }
}

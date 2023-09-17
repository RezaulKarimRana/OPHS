using System;
using System.Threading.Tasks;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.FundDisburse;
using AMS.Models.ServiceModels.FundRequisition;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using AMS.Services.Contracts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace AMS.Services
{
    public class EmailHandlerService : IEmailHandlerService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IHtmlGeneratorService _htmlGeneratorService;

        public EmailHandlerService(
            IUnitOfWorkFactory uowFactory
            , IUserService userService
            , IHtmlGeneratorService htmlGeneratorService,
            IConfiguration configuration,
            IDataProtectionProvider provider

        )
        {
            _uowFactory = uowFactory;
            _userService = userService;
            _htmlGeneratorService = htmlGeneratorService;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
        }

        public async Task<int> SaveNewFundRequisitionEmail(FundRequisitionVM requestDto, IUnitOfWork uow,UserEntity loggedUser)
        {
            try
            {
                var requestTOSave = new CreateEmailContantRequest();
                var allStackHolder = await _userService.getUserEmailAddressByDepartmentId(19);

                requestTOSave.ToEmail = allStackHolder;
                requestTOSave.ToCc = loggedUser.Email_Address /*+ "," + allStackHolder[1]*/;
                requestTOSave.Subject = requestDto.RequisitionType.ToLower() == "Fund".ToLower()
                    ? "New Fund Requisition - : " + requestDto.EstimateIdentifier + " [" +
                      requestDto.FundRequisitionId + "]"
                    : "New Payment Requisition - : " + requestDto.EstimateIdentifier + " [" +
                      requestDto.FundRequisitionId + "]";
                requestTOSave.Body = _htmlGeneratorService.getNewFundRequisitionEmailBody(requestDto);
                requestTOSave.CreatedBy = loggedUser.Id;
                requestTOSave.ModifiedBy = loggedUser.Id;
                requestTOSave.AMSID = requestDto.FundRequisitionId.ToString();
                requestTOSave.Department = loggedUser.Department_Id;

                var response = await uow.EmailRepo.SaveForEmailServer(requestTOSave);
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> SaveFundRequisitionRejectEmailEmail(FundRequisitionVM requestDto, IUnitOfWork uow,UserEntity loggedUser)
        {
            try
            {
                requestDto.FundRejectorName = loggedUser.First_Name + ' ' + loggedUser.Last_Name;
                var fundRequisitionHistory = await uow.FundRequisitionRepo.GetFundRequisitionHistoryByFundRequisitionId(requestDto.FundRequisitionId);
                var requestTOSave = new CreateEmailContantRequest();
                requestTOSave.ToEmail = _userService.GetById(fundRequisitionHistory.FundRequisitionCreatedBy).Result.Email_Address;
                requestTOSave.ToCc = loggedUser.Email_Address;
                requestTOSave.Subject = requestDto.RequisitionType.ToLower() == "Fund".ToLower()
                    ? "New Fund Requisition Reject - : " + requestDto.EstimateIdentifier + " [" +
                      requestDto.FundRequisitionId + "]"
                    : "New Payment Requisition Reject - : " + requestDto.EstimateIdentifier + " [" +
                      requestDto.FundRequisitionId + "]";
                requestTOSave.Body = _htmlGeneratorService.getRejectFundRequisitionEmailBody(requestDto);
                requestTOSave.CreatedBy = loggedUser.Id;
                requestTOSave.ModifiedBy = loggedUser.Id;
                requestTOSave.AMSID = requestDto.FundRequisitionId.ToString();
                requestTOSave.Department = loggedUser.Department_Id;

                var response = await uow.EmailRepo.SaveForEmailServer(requestTOSave);
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> SaveNewFundDisburseEmail(FundDisburseVM requestDto, IUnitOfWork uow,UserEntity loggedUser)
        {
            try
            {
                var fundRequisitionHistory = await uow.FundRequisitionRepo.GetFundRequisitionHistoryByFundRequisitionId(requestDto.FundRequisitionId);

                var requestTOSave = new CreateEmailContantRequest();


                requestTOSave.ToEmail = _userService.GetById(fundRequisitionHistory.FundRequisitionCreatedBy).Result.Email_Address;

                requestTOSave.ToCc = loggedUser.Email_Address /*+ "," + allStackHolder[1]*/;
                requestTOSave.Subject = requestDto.RequisitionType.ToLower() == "Fund".ToLower()
                    ? "New Fund Disburse - : " + requestDto.EstimateIdentifier + " [" +
                      requestDto.FundDisburseId + "]"
                    : "New Payment Disburse - : " + requestDto.EstimateIdentifier + " [" +
                      requestDto.FundDisburseId + "]";
                requestDto.FundSenderName = loggedUser.First_Name + " " + loggedUser.Last_Name;
                requestTOSave.Body = _htmlGeneratorService.getNewFundDisburseEmailBody(requestDto);
                requestTOSave.CreatedBy = loggedUser.Id;
                requestTOSave.ModifiedBy = loggedUser.Id;
                requestTOSave.AMSID = requestDto.FundDisburseId.ToString();
                requestTOSave.Department = loggedUser.Department_Id;

                var response = await uow.EmailRepo.SaveForEmailServer(requestTOSave);
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> SaveNewFundReceiveRollbackEmail(FundDisburseVM requestDto, IUnitOfWork uow,UserEntity loggedUser)
        {
            try
            {
                var requestTOSave = new CreateEmailContantRequest();

                string toEmail = requestDto.DisburseStatus == 100

                    ? _userService.GetById(requestDto.FundSenderUserId).Result.Email_Address
                    : loggedUser.Email_Address;
                string ccEmail = requestDto.DisburseStatus == 100
                    ? loggedUser.Email_Address
                    : _userService.GetById(requestDto.FundSenderUserId).Result.Email_Address;


                requestTOSave.ToEmail = toEmail;

                requestTOSave.ToCc = ccEmail;
                requestTOSave.Subject = requestDto.DisburseStatus == 100
                    ? "AMS - New Fund Received By Requestor   :" + requestDto.EstimateIdentifier + "[" +
                      requestDto.FundDisburseId + "]"
                    : "AMS-New  Fund Rollback To Finance :" + requestDto.EstimateIdentifier + "[" +
                      requestDto.FundDisburseId + "]";
                requestTOSave.Body = _htmlGeneratorService.getNewFundReceivedOrRollbackEmailBody(requestDto);
                requestTOSave.CreatedBy = loggedUser.Id;
                requestTOSave.ModifiedBy = loggedUser.Id;
                requestTOSave.AMSID = requestDto.FundDisburseId.ToString();
                requestTOSave.Department = loggedUser.Department_Id;
                using var uuow = _uowFactory.GetUnitOfWork();
                var response = await uuow.EmailRepo.SaveForEmailServer(requestTOSave);
                uuow.Commit();

                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task<int> SaveSettlementEmail(int settlementId,UserEntity loggedUser)
        {
            try
            {
                var requestTOSave = new CreateEmailContantRequest();
                using (var uowObUnitOfWork = _uowFactory.GetUnitOfWork())
                {
                    var requestId = _protector.Protect(settlementId.ToString());
                    var settlement = uowObUnitOfWork.SettlementRepo.getSettlementById(settlementId).Result;
                    var SettlementApprovalEmail = uowObUnitOfWork.SettlementRepo.getSettlementRelatedEmailAddressBySettlement(settlementId).Result;
                    string
                        message = settlement.IsItFinalSetttlement == 1
                            ? "This is to inform you that following Final settlement <a href='https://ams.summitcommunications.net/Settlement/SettlementAprroverView?id=" + requestId + "'>" + settlementId + "</a> has been initiated by " +
                              loggedUser.First_Name + " " + loggedUser.Last_Name + " ( " + loggedUser.Mobile_Number +
                              " ) ." +
                              "Please check and acknowledge accordingly to the system "
                            : "This is to inform you that following  settlement <a href='https://ams.summitcommunications.net/Settlement/SettlementAprroverView?id=" + requestId + "'>" + settlementId + "</a> has been initiated by " +
                              loggedUser.First_Name + " " + loggedUser.Last_Name + " ( " + loggedUser.Mobile_Number +
                              " ) ." +
                              "Please check and acknowledge accordingly to the system ";

                    uowObUnitOfWork.Commit();
                    requestTOSave.ToEmail = SettlementApprovalEmail.NextApprover;

                    requestTOSave.ToCc = SettlementApprovalEmail.AllApproverExceptNextApprover;
                    requestTOSave.Subject = settlement.IsItFinalSetttlement == 1
                        ? "Final Settlement  - : " + settlement.EstimateIdentifier + " [" +
                          settlement.Id + "]"
                        : "New Settlement - : " + settlement.EstimateIdentifier + " [" +
                          settlement.Id + "]";
                    requestTOSave.Body = await _htmlGeneratorService
                        .getNewSettlementInitEmailBody(settlement.Id, message);
                    requestTOSave.CreatedBy = loggedUser.Id;
                    requestTOSave.ModifiedBy = loggedUser.Id;
                    requestTOSave.AMSID = settlement.Id.ToString();
                    requestTOSave.Department = loggedUser.Department_Id;
                }
                var uowObUnitOfWorkEmail = _uowFactory.GetUnitOfWork();
                var response = await uowObUnitOfWorkEmail.EmailRepo.SaveForEmailServer(requestTOSave);
                uowObUnitOfWorkEmail.Commit();

                return 1;
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
                throw;
            }
        }
        public async Task<int> SaveMemoEmail(int memoId,UserEntity loggedUser)
        {
            try
            {
                var requestTOSave = new CreateEmailContantRequest();
                using (var uowObUnitOfWork = _uowFactory.GetUnitOfWork())
                {
                    var requestId = _protector.Protect(memoId.ToString());
                    var memo = uowObUnitOfWork.EstimateMemoEntityRepo.GetEstimateMemoEntityById(memoId).Result;
                    var SettlementApprovalEmail = uowObUnitOfWork.EstimateMemoEntityRepo.getMemoRelatedEmailAddressByMemo(memoId).Result;

                    uowObUnitOfWork.Commit();
                    requestTOSave.ToEmail = SettlementApprovalEmail.NextApprover;

                    requestTOSave.ToCc = SettlementApprovalEmail.AllApproverExceptNextApprover;

                    requestTOSave.Subject = "Memo Initiated - : " + memo.EstimateIdentifier + " [" +
                                            memoId + "]";

                    string message =
                        "This is to inform you that following Memo <a href='https://ams.summitcommunications.net/BudgetMemo/ShowDetailsForMemoApproval?id=" + requestId + "'>" + memoId + "</a> has been initiate by " + loggedUser.First_Name +
                        " " + loggedUser.Last_Name + " ( " + loggedUser.Mobile_Number + ")." + " Please check and acknowledge accordingly to the system.";

                    requestTOSave.Body = await _htmlGeneratorService.getMemoInitEmailBody(memo.Id, message);
                    requestTOSave.CreatedBy = loggedUser.Id;
                    requestTOSave.ModifiedBy = loggedUser.Id;
                    requestTOSave.AMSID = memoId.ToString();
                    requestTOSave.Department = loggedUser.Department_Id;
                }
                var uowObUnitOfWorkEmail = _uowFactory.GetUnitOfWork();
                var response = await uowObUnitOfWorkEmail.EmailRepo.SaveForEmailServer(requestTOSave);
                uowObUnitOfWorkEmail.Commit();

                return 1;
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
                throw;
            }
        }
    }
}

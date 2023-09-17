using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Common.Constants;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.FundDisburse;
using AMS.Models.ServiceModels.FundRequisition;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using AMS.Services.Contracts;
using AMS.Services.FundRequisitionService;
using AMS.Services.Managers.Contracts;
using HTML.Generator.Helper;
using HtmlGenerate;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using MySqlX.XDevAPI;

namespace AMS.Services.FundDisburseService
{
    public  class FundDisburseService : IFundDisburseService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionManager _sessionManager;
        private readonly IDepartmentService _departmentService;
        private readonly IUserService _userService;
       private readonly IEmailHandlerService _emailHandlerService;
        private readonly IBudgetApproverService _budgetApproverService;
        private readonly IConfiguration _configuration;
        IDataProtector _protector;

        public FundDisburseService(IUnitOfWorkFactory uowFactory, ISessionManager sessionManager,
            IDepartmentService departmentService, IUserService userService,
            IEmailHandlerService emailHandlerService,
            IBudgetApproverService budgetApproverService, IDataProtectionProvider provider,
            IConfiguration configuration)
        {
            _uowFactory = uowFactory;
            _sessionManager = sessionManager;
            _departmentService = departmentService;
            _userService = userService;
            _emailHandlerService = emailHandlerService;
            _budgetApproverService = budgetApproverService;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
            //_fundRequisitionService = fundRequisitionService;
        }
        public async Task<List<FundDisburseHistory>> GetFundDisburseHistoryByEstimateId(int estimationId)
        {
            try
            {
                var sessionUser = _sessionManager.GetUser();
                if (sessionUser == null)
                    throw new Exception("user session expired");

                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.FundDisburseRepo.GetFundDisburseHistoryByEstimateId(estimationId);

                return response;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> addFundDisburse(FundDisburse requestDto)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

              
                requestDto.Created_By = sessionUser.Id;
                int fundDisburseId = 0;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var response = await this.CreateFundDisburse(requestDto, uow);
                    fundDisburseId = response.fundDisburseId;

                    uow.Commit();
                }

                if (fundDisburseId > 0)
                {
                    var fundDisburseResponse = await GetFundDisburseHistoryByFundDisburseId(fundDisburseId);
                    using (var uowObUnitOfWork = _uowFactory.GetUnitOfWork())
                    {
                        _emailHandlerService.SaveNewFundDisburseEmail(fundDisburseResponse, uowObUnitOfWork, sessionUser);
                    }
                }
                return fundDisburseId;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<int> FundDisburseReceiveOrRollback(FundDisburse requestDto)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");


                requestDto.Updated_By = sessionUser.Id;
                int fundDisburseId = 0;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var response = await this.CreateFundDisburseReceiveOrRollback(requestDto, uow);
                    fundDisburseId = response.fundDisburseId;

                    uow.Commit();
                }
                if (fundDisburseId > 0)
                {
                    var fundDisburseResponse = await GetFundDisburseHistoryByFundDisburseId(fundDisburseId);
                    using (var uowObUnitOfWork = _uowFactory.GetUnitOfWork())
                    {
                        _emailHandlerService.SaveNewFundReceiveRollbackEmail(fundDisburseResponse, uowObUnitOfWork, sessionUser);
                        
                    }
                }
                return fundDisburseId;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<CreateFundDisburseResponse> CreateFundDisburse(FundDisburse fundDisburseRequest, IUnitOfWork uow) //
        {
            try
            {
                var response = new CreateFundDisburseResponse();
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                {
                    throw new Exception("user session expired");
                }

                var userDept = await _departmentService.GetById(sessionUser.Department_Id);
                if (userDept == null)
                {
                    throw new Exception("Department Can not be empty");
                }

                //var uniqueValueRow = await uow.EstimationRepo.GetIncrementedValue();

                //DateTime dt = DateTime.Now;
                //var month = dt.Month < 10 ? "0" + dt.Month.ToString() : dt.Month.ToString();
                //var day = dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString();


                //var firstPartUniqueId = "";
                //if (estimateRequest.EstimateType == 2)
                //    firstPartUniqueId = "NB";
                //else if (estimateRequest.EstimateType == 3)
                //    firstPartUniqueId = "MM";
                //else if (estimateRequest.EstimateType == 7)
                //    firstPartUniqueId = "PA";
                //var secondPartUniqueId = !String.IsNullOrWhiteSpace(userDept.Name) && userDept.Name.Length >= 3 ? userDept.Name.Substring(0, 3) : userDept.Name;
                //var thirdPartUniqueId = month + day + dt.Year.ToString();
                //var lastPartUniqueId = (uniqueValueRow.Value + 1).ToString();

                //estimateRequest.UniqueIdentifier = firstPartUniqueId + "-" + secondPartUniqueId + "-" + thirdPartUniqueId + "-" + lastPartUniqueId;

                response.fundDisburseId = await uow.FundDisburseRepo.CreateFundDisburse(new FundDisburse()
                {

                    Status = FundDisburseStatus.FundDisbursePending,

                    FundRequisitionId = fundDisburseRequest.FundRequisitionId,
                    DisburseAmount = fundDisburseRequest.DisburseAmount,
                    RemarksByFinance = fundDisburseRequest.RemarksByFinance,
                    FundAvailableDate = fundDisburseRequest.FundAvailableDate,
                    Created_Date = DateTime.Now,
                    Updated_By = sessionUser.Id,
                    Updated_Date = DateTime.Now,

                    Created_By = sessionUser.Id
                });

                //await uow.EstimationRepo.IncreaseIncrementalValue(sessionUser.Id);
              

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<CreateFundDisburseResponse> CreateFundDisburseReceiveOrRollback(FundDisburse fundDisburseRequest, IUnitOfWork uow) //
        {
            try
            {
                var response = new CreateFundDisburseResponse();
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                {
                    throw new Exception("user session expired");
                }

                var userDept = await _departmentService.GetById(sessionUser.Department_Id);
                if (userDept == null)
                {
                    throw new Exception("Department Can not be empty");
                }

                //var uniqueValueRow = await uow.EstimationRepo.GetIncrementedValue();

                //DateTime dt = DateTime.Now;
                //var month = dt.Month < 10 ? "0" + dt.Month.ToString() : dt.Month.ToString();
                //var day = dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString();


                //var firstPartUniqueId = "";
                //if (estimateRequest.EstimateType == 2)
                //    firstPartUniqueId = "NB";
                //else if (estimateRequest.EstimateType == 3)
                //    firstPartUniqueId = "MM";
                //else if (estimateRequest.EstimateType == 7)
                //    firstPartUniqueId = "PA";
                //var secondPartUniqueId = !String.IsNullOrWhiteSpace(userDept.Name) && userDept.Name.Length >= 3 ? userDept.Name.Substring(0, 3) : userDept.Name;
                //var thirdPartUniqueId = month + day + dt.Year.ToString();
                //var lastPartUniqueId = (uniqueValueRow.Value + 1).ToString();

                //estimateRequest.UniqueIdentifier = firstPartUniqueId + "-" + secondPartUniqueId + "-" + thirdPartUniqueId + "-" + lastPartUniqueId;

                response.fundDisburseId = await uow.FundDisburseRepo.FundDisburseReceiveOrRollback(new FundDisburse()
                {

                    Status = fundDisburseRequest.isRollBack? FundDisburseStatus.FundDisburseRollBack : FundDisburseStatus.FundDisburseSuccessfullyReceived,

                    Id = fundDisburseRequest.Id,
                    ReceivedAmount = fundDisburseRequest.ReceivedAmount,
                    RemarksByFundReceiver = fundDisburseRequest.RemarksByFundReceiver,
                    Updated_By = sessionUser.Id,
                    Created_By = sessionUser.Id,
                    Created_Date = DateTime.Now,
                    Updated_Date = DateTime.Now

                });

                //await uow.EstimationRepo.IncreaseIncrementalValue(sessionUser.Id);


                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<FundDisburseVM>> PendingFundDisburseListForAcknowledgement(int userId, int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                List<FundDisburseVM> response = new List<FundDisburseVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {

                    response = await uow.FundDisburseRepo.PendingFundDisburseListForAcknowledgement(userId, currentPageIndex, pAGE_SIZE, sessionUser.Id, sessionUser.Department_Id);

                }
                return response;
            }
            catch (Exception e) { throw; }

        }
        public async Task<List<FundDisburseVM>> FundDisburseByFinanceWaitingForAcknowledge(int userId, int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                List<FundDisburseVM> response = new List<FundDisburseVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {

                    response = await uow.FundDisburseRepo.FundDisburseByFinanceWaitingForAcknowledge(userId, currentPageIndex, pAGE_SIZE, sessionUser.Id, sessionUser.Department_Id);

                }
                return response;
            }
            catch (Exception e) { throw; }

        }
        public async Task<List<FundDisburseVM>> PendingRollBackListForReSubmitByFinance(int userId, int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                List<FundDisburseVM> response = new List<FundDisburseVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {

                    response = await uow.FundDisburseRepo.PendingRollBackListForReSubmitByFinance(userId, currentPageIndex, pAGE_SIZE, sessionUser.Id, sessionUser.Department_Id);

                }
                return response;
            }
            catch (Exception e) { throw; }

        }
        

        public async Task<List<FundDisburseVM>> CompletedDisburseListDepartmentWise(int userId, int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                List<FundDisburseVM> response = new List<FundDisburseVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {

                    response = await uow.FundDisburseRepo.CompletedDisburseListDepartmentWise(userId, currentPageIndex, pAGE_SIZE, sessionUser.Id, sessionUser.Department_Id);

                }
                return response;
            }
            catch (Exception e) { throw; }

        }

        public async Task<FundDisburseVM> GetFundDisburseHistoryByFundDisburseId(int fundDisburseId)
        {
            try
            {
                var response = new FundDisburseVM();

                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.FundDisburseRepo.GetFundDisburseHistoryByFundDisburseId(fundDisburseId);

                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> FundReDisburse(FundDisburse requestDto)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");


                requestDto.Updated_By = sessionUser.Id;
                int fundDisburseId = 0;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var response = await this.CreateFundReDisburse(requestDto, uow);
                    fundDisburseId = response.fundDisburseId;

                    uow.Commit();
                }
                return fundDisburseId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateFundDisburseResponse> CreateFundReDisburse(FundDisburse fundDisburseRequest, IUnitOfWork uow) //
        {
            try
            {
                var response = new CreateFundDisburseResponse();
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                {
                    throw new Exception("user session expired");
                }

                var userDept = await _departmentService.GetById(sessionUser.Department_Id);
                if (userDept == null)
                {
                    throw new Exception("Department Can not be empty");
                }

                //var uniqueValueRow = await uow.EstimationRepo.GetIncrementedValue();

                //DateTime dt = DateTime.Now;
                //var month = dt.Month < 10 ? "0" + dt.Month.ToString() : dt.Month.ToString();
                //var day = dt.Day < 10 ? "0" + dt.Day.ToString() : dt.Day.ToString();


                //var firstPartUniqueId = "";
                //if (estimateRequest.EstimateType == 2)
                //    firstPartUniqueId = "NB";
                //else if (estimateRequest.EstimateType == 3)
                //    firstPartUniqueId = "MM";
                //else if (estimateRequest.EstimateType == 7)
                //    firstPartUniqueId = "PA";
                //var secondPartUniqueId = !String.IsNullOrWhiteSpace(userDept.Name) && userDept.Name.Length >= 3 ? userDept.Name.Substring(0, 3) : userDept.Name;
                //var thirdPartUniqueId = month + day + dt.Year.ToString();
                //var lastPartUniqueId = (uniqueValueRow.Value + 1).ToString();

                //estimateRequest.UniqueIdentifier = firstPartUniqueId + "-" + secondPartUniqueId + "-" + thirdPartUniqueId + "-" + lastPartUniqueId;

                response.fundDisburseId = await uow.FundDisburseRepo.FundReDisburse(new FundDisburse()
                {

                    Status =  FundDisburseStatus.FundDisbursePending,

                    Id = fundDisburseRequest.Id,
                    DisburseAmount = fundDisburseRequest.DisburseAmount,
                    RemarksByFinance = fundDisburseRequest.RemarksByFinance,
                    Updated_By = sessionUser.Id,
                    Created_By = sessionUser.Id,
                    Created_Date = DateTime.Now,
                    Updated_Date = DateTime.Now

                });

                //await uow.EstimationRepo.IncreaseIncrementalValue(sessionUser.Id);

                


                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }

       
        


       

       


    }
}

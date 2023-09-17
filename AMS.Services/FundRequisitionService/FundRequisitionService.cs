using System;
using AMS.Models.DomainModels;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Managers.Contracts;
using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using AMS.Common.Constants;
using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.FundRequisition;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using AMS.Services.Contracts;
using AMS.Services.FundDisburseService;
using HTML.Generator.Helper;
using HtmlGenerate;

namespace AMS.Services.FundRequisitionService
{
    public class FundRequisitionService : IFundRequisitionService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionManager _sessionManager;
        private readonly IDepartmentService _departmentService;
        private readonly IUserService _userService;
        private readonly IBudgetApproverService _budgetApproverService;
        private readonly IEmailHandlerService _emailHandlerService;

        private readonly IConfiguration _configuration;
        IDataProtector _protector;

        public FundRequisitionService(IUnitOfWorkFactory uowFactory, ISessionManager sessionManager,
            IDepartmentService departmentService, IUserService userService,
            IBudgetApproverService budgetApproverService,
            IEmailHandlerService emiEmailHandlerService,
            IDataProtectionProvider provider,
            IConfiguration configuration)
        {
            _uowFactory = uowFactory;
            _sessionManager = sessionManager;
            _departmentService = departmentService;
            _userService = userService;
            _budgetApproverService = budgetApproverService;
            _emailHandlerService = emiEmailHandlerService;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
            
        }

        public async Task<int> addFundRequisition(FundRequisition requestDto)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                requestDto.RequisitionStatus = FundRequisitionStatus.Pending;
                requestDto.Created_By = sessionUser.Id;
                int fundRequisitionId = 0;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var response = await this.CreateFundRequisition(requestDto, uow);
                    fundRequisitionId = response.fundRequisitionId;


                    uow.Commit();
                }

                if (fundRequisitionId > 0)
                {
                    var fundRequisitionResponse = await GetFundRequisitionHistoryByFundRequisitionId(fundRequisitionId);
                    using (var uow = _uowFactory.GetUnitOfWork())
                    {
                        _emailHandlerService.SaveNewFundRequisitionEmail(fundRequisitionResponse, uow, sessionUser);
                    }
                }

                return fundRequisitionId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateFundRequisitionResponse> CreateFundRequisition(FundRequisition fundRequisitionRequest,
            IUnitOfWork uow) //
        {
            try
            {
                var response = new CreateFundRequisitionResponse();
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

                response.fundRequisitionId = await uow.FundRequisitionRepo.CreateFundRequisition(new FundRequisition()
                {
                    RequisitionStatus = fundRequisitionRequest.RequisitionStatus,

                    EstimationId = fundRequisitionRequest.EstimationId,
                    Amount = fundRequisitionRequest.Amount,
                    Remarks = fundRequisitionRequest.Remarks,
                    ProposedDisburseDate = fundRequisitionRequest.ProposedDisburseDate,
                    Type = fundRequisitionRequest.Type,
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

        public async Task<List<FundRequisitionVM>> SubmitedFundRequistionList(int userId, int currentPageIndex,
            int pAGE_SIZE)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                List<FundRequisitionVM> response = new List<FundRequisitionVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.FundRequisitionRepo.SubmitedFundRequistionList(userId, currentPageIndex,
                        pAGE_SIZE, sessionUser.Id, sessionUser.Department_Id);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        //

        public async Task<List<FundRequisitionVM>> FundRequistionListByStatus(int userId, int currentPageIndex,
            int pAGE_SIZE, int status)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                List<FundRequisitionVM> response = new List<FundRequisitionVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.FundRequisitionRepo.FundRequistionListByStatus(userId, currentPageIndex,
                        pAGE_SIZE, sessionUser.Id, sessionUser.Department_Id, status);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<FundRequisitionVM>> FundRequistionListForFinance(int userId, int currentPageIndex,
            int pAGE_SIZE)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                List<FundRequisitionVM> response = new List<FundRequisitionVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.FundRequisitionRepo.FundRequistionListForFinance(userId, currentPageIndex,
                        pAGE_SIZE, sessionUser.Id, sessionUser.Department_Id);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<FundRequisitionVM>> RejectedFundRequistionListForFinance(int userId, int currentPageIndex,
            int pAGE_SIZE)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                List<FundRequisitionVM> response = new List<FundRequisitionVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.FundRequisitionRepo.RejectedFundRequistionListForFinance(userId, currentPageIndex,
                        pAGE_SIZE, sessionUser.Id, sessionUser.Department_Id);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<FundRequisitionVM> GetFundRequisitionHistoryByFundRequisitionId(int fundRequisitioId)
        {
            try
            {
                var response = new FundRequisitionVM();

                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.FundRequisitionRepo.GetFundRequisitionHistoryByFundRequisitionId(fundRequisitioId);

                uow.Commit();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FundRequisitionVM> GetFundRequisitionHistoryByEstimationId(int estimationId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var response = new FundRequisitionVM();

                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.FundRequisitionRepo.GetFundRequisitionHistoryByEstimationId(estimationId , sessionUser.Id , sessionUser.Department_Id);

                uow.Commit();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<FundRequisitionDisburseHistory>> getFundRequisitionDisburseHistory(int estimationId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var response = new List<FundRequisitionDisburseHistory>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.FundRequisitionRepo.getFundRequisitionDisburseHistory(sessionUser.Id,
                    sessionUser.Department_Id, estimationId);
                uow.Commit();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<FundRequisitionDisburseHistory>> getTotalFundRequisitionDisburseHistory(int estimationId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var response = new List<FundRequisitionDisburseHistory>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.FundRequisitionRepo.getTotalFundRequisitionDisburseHistory(sessionUser.Id,
                    sessionUser.Department_Id, estimationId);
                uow.Commit();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<FundRequisitionDisburseHistory>> getFundRequisitionDisburseHistoryOfOtherDepartment(int estimationId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var response = new List<FundRequisitionDisburseHistory>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.FundRequisitionRepo.getFundRequisitionDisburseHistory_of_other_department(sessionUser.Id,
                    sessionUser.Department_Id, estimationId);
                uow.Commit();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<int> UpdateFundRequistionStatus(int fundRequistionId, string Remarks)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.FundRequisitionRepo.UpdateFundRequistionStatus(fundRequistionId, Remarks,
                    BaseEntity.EntityStatus.Reject, sessionUser.Id);
                uow.Commit();
                if (response > 0)
                {

                    var fundRequisitionResponse = await GetFundRequisitionHistoryByFundRequisitionId(fundRequistionId);
                    using (var uowObUnitOfWork = _uowFactory.GetUnitOfWork())
                    {
                        _emailHandlerService.SaveFundRequisitionRejectEmailEmail(fundRequisitionResponse, uowObUnitOfWork, sessionUser);
                        uowObUnitOfWork.Commit();
                    }

                }

                return 1;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        

        public async Task<List<FundRequisitionVM>> getInCompletedFundRequistionForCheckFinalSettlement(int estimationId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var response = new List<FundRequisitionVM>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.FundRequisitionRepo.getInCompletedFundRequisitionForCheckFinalSettlement(sessionUser.Id,
                    sessionUser.Department_Id, estimationId);
                uow.Commit();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }



        


       

       
    }
}
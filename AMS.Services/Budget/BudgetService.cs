using AMS.Common.Notifications;
using AMS.Infrastructure.Email;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ServiceModels.BudgetEstimate.DraftedBudget;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models;
using AMS.Repositories.DatabaseRepos.EstimateApproverRepo.Models;
using AMS.Repositories.DatabaseRepos.EstimateDetailsRepo.Models;
using AMS.Repositories.DatabaseRepos.EstimationRepo.Models;
using AMS.Repositories.DatabaseRepos.ParticularWiseSummaryRepo.Models;
using AMS.Repositories.DatabaseRepos.ProcurementApproval.Models;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using AMS.Services.Budget.Contracts;
using AMS.Services.Managers.Contracts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AMS.Services.Contracts;
using AMS.Common.Constants;
using AMS.Models.ServiceModels.Dashboard;

namespace AMS.Services.Budget
{
    public class BudgetService : IBudgetService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionManager _sessionManager;
        private readonly IDepartmentService _departmentService;
        private readonly IUserService _userService;
        private readonly IBudgetApproverService _budgetApproverService;
        private readonly IHtmlGeneratorService _htmlGeneratorService;
        private readonly IConfiguration _configuration;
        IDataProtector _protector;

        public BudgetService(IUnitOfWorkFactory uowFactory, ISessionManager sessionManager,
            IDepartmentService departmentService, IUserService userService, 
            IBudgetApproverService budgetApproverService, 
            IHtmlGeneratorService htmlGeneratorService,
            IDataProtectionProvider provider,
            IConfiguration configuration)
        {
            _uowFactory = uowFactory;
            _sessionManager = sessionManager;
            _departmentService = departmentService;
            _userService = userService;
            _budgetApproverService = budgetApproverService;
            _htmlGeneratorService = htmlGeneratorService;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
        }

        public async Task<CreateEstimateResponse> CreateBudgeEstimation(CreateEstimateRequest estimateRequest, IUnitOfWork uow) //
        {
            try
            {
                var response = new CreateEstimateResponse();
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
                int id;
                id = await uow.EstimationRepo.CreateEstimation(new CreateEstimationRequest()
                {
                    EstimateType = estimateRequest.EstimateType,
                    CurrencyType = estimateRequest.CurrencyType,
                    DepartmentName = userDept.Name,
                    Status = estimateRequest.Status,
                    SystemID = estimateRequest.SystemID,
                    Project_Id = estimateRequest.Project_Id,
                    Subject = estimateRequest.Subject,
                    Objective = estimateRequest.Objective,
                    Details = estimateRequest.Details,
                    PlanStartDate = estimateRequest.PlanStartDate,
                    PlanEndDate = estimateRequest.PlanEndDate,
                    Remarks = estimateRequest.Remarks,
                    TotalPrice = estimateRequest.TotalPrice,
                    Created_By = sessionUser.Id,
                    TotalPriceRemarks = estimateRequest.TotalPriceRemarks
                });

                await uow.EstimationRepo.IncreaseIncrementalValue(sessionUser.Id);

                response.budgetEstimateID = id;
                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<CreateDepartmentWiseSummaryServiceResponse> CreateBudgetDepartmentWiseSummary(CreateDepartmentWiseSummaryServiceRequest deptSummaryReq)
        {
            try
            {
                var response = new CreateDepartmentWiseSummaryServiceResponse();
                var sessionUser = await _sessionManager.GetUser();

                int id;

                using var uow = _uowFactory.GetUnitOfWork();
                id = await uow.DepartmentWiseSummaryRepo.CreateDepartmentWiseSummary(
                    new Repositories.DatabaseRepos.DepartmentWiseSummaryRepo.Models.CreateDepartmentWiseSummaryRequest()
                    {
                        Department_Id = deptSummaryReq.Department_Id,
                        TotalPrice = deptSummaryReq.TotalPrice,
                        Estimate_Id = deptSummaryReq.Estimate_Id,
                        Created_By = sessionUser.Id
                    });

                uow.Commit();
                response.DepartmentWiseSummary_Id = id;

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateEstimateDetailsServiceResponse> CreateBudgetEstimaeDetails(CreateEstimateDetailsServiceRequest detailsReq)
        {
            try
            {
                var response = new CreateEstimateDetailsServiceResponse();
                var sessionUser = await _sessionManager.GetUser();

                int id;

                using var uow = _uowFactory.GetUnitOfWork();
                id = await uow.EstimateDetailsRepo.CreateEstimateDetails(new CreateEstimateDetailsRequest()
                {
                    Estimation_Id = detailsReq.Estimation_Id,
                    Item_Id = detailsReq.Item_Id,
                    NoOfMachineAndUsagesAndTeamAndCar = detailsReq.NoOfMachineAndUsagesAndTeamAndCar,
                    NoOfDayAndTotalUnit = detailsReq.NoOfDayAndTotalUnit,
                    QuantityRequired = detailsReq.QuantityRequired,
                    UnitPrice = detailsReq.UnitPrice,
                    TotalPrice = detailsReq.TotalPrice,
                    Remarks = detailsReq.Remarks,
                    AreaType = detailsReq.AreaType,
                    ResponsibleDepartment_Id = detailsReq.ResponsibleDepartment_Id,
                    Thana_Id = detailsReq.Thana_Id,
                    Created_By = sessionUser.Id
                });

                uow.Commit();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateEstimateApproverResponse> CreateBudgetEstimateApprover(CreateEstimateApproverServiceRequest approverReq)
        {
            try
            {
                var response = new CreateEstimateApproverResponse();
                var sessionUser = await _sessionManager.GetUser();

                int id;

                using var uow = _uowFactory.GetUnitOfWork();
                id = await uow.EstimateApproverRepo.CreateEstimateApprover(new CreateEstimateApproverRequest()
                {
                    Estimate_Id = approverReq.Estimate_Id,
                    User_Id = approverReq.User_Id,
                    Priority = approverReq.Priority,
                    Status = approverReq.Status,
                    Remarks = approverReq.Remarks,
                    RolePriority_Id = approverReq.RolePriority_Id,
                    Created_By = sessionUser.Id
                });

                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CreateDepartmentWiseSummaryHistoryResponse> CreateDepartmentWiseSummaryHistory(CreateDepartmentWiseSummaryHistoryServiceRequest deptSummaryHistoryReq)
        {
            try
            {
                var response = new CreateDepartmentWiseSummaryHistoryResponse();
                var sessionUser = await _sessionManager.GetUser();

                int id;

                using var uow = _uowFactory.GetUnitOfWork();
                id = await uow.DepartmentWiseSummaryHistoryRepo.CreateDepartmentWiseSummaryHistory(
                    new Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo.Models.CreateDepartmentWiseSummaryHistoryRequest()
                    {
                        Department_Id = deptSummaryHistoryReq.Department_Id,
                        TotalPrice = deptSummaryHistoryReq.TotalPrice,
                        Estimate_Id = deptSummaryHistoryReq.Estimate_Id,
                        //DepartmentWiseSummary_Id = deptSummaryHistoryReq.DepartmentWiseSummary_Id,
                        Created_By = sessionUser.Id
                    });

                uow.Commit();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateEstimateApproverChangeHistoryServiceResponse> CreateEstimateApproverHistory(CreateEstimateApproverChangeHistoryServiceRequest approReq)
        {
            try
            {
                var response = new CreateEstimateApproverChangeHistoryServiceResponse();
                var sessionUser = await _sessionManager.GetUser();

                int id;

                using var uow = _uowFactory.GetUnitOfWork();
                id = await uow.EstimateApproverChangeHistoryRepo.CreateEstimateApproverChangeHistory(
                    new Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Models.CreateEstimateApproverChangeHistoryRequest()
                    {
                        Estimate_Id = approReq.Estimate_Id,
                        User_Id = approReq.User_Id,
                        Priority = approReq.Priority,
                        Status = approReq.Status,
                        Remarks = approReq.Remarks,
                        RolePriority_Id = approReq.RolePriority_Id,
                        Created_By = sessionUser.Id
                    });

                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CreateAttachmentServiceResponse> CreateEstimateAttachment(CreateAttachmentServiceRequest attachReq)
        {
            try
            {
                var response = new CreateAttachmentServiceResponse();
                var sessionUser = await _sessionManager.GetUser();

                int id;

                using var uow = _uowFactory.GetUnitOfWork();
                id = await uow.EstimationAttachment.CreateAttachment(new Repositories.DatabaseRepos.EstimationAttachmentRepo.Models.CreateAttachmentRequest()
                {
                    URL = attachReq.URL,
                    FileName = attachReq.FileName,
                    Estimation_Id = attachReq.Estimation_Id,
                    Created_By = sessionUser.Id
                });

                uow.Commit();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateEstimationHistoryServiceResponse> CreateEstimateHistory(CreateEstimationHistoryServiceRequest estiReq)
        {
            try
            {
                var response = new CreateEstimationHistoryServiceResponse();
                var sessionUser = await _sessionManager.GetUser();

                int id;

                using var uow = _uowFactory.GetUnitOfWork();
                id = await uow.EstimateChangeHistoryRepo.CreateEstimationHistory(new
                    Repositories.DatabaseRepos.EstimateChangeHistoryRepo.Models.CreateEstimationHistoryRequest()
                {
                    EstimateType_Id = estiReq.EstimateType,
                    Status = estiReq.Status,
                    SystemID = estiReq.SystemID,
                    ProjectId = estiReq.ProjectId,
                    Subject = estiReq.Subject,
                    Objective = estiReq.Objective,
                    Details = estiReq.Details,
                    PlanStartDate = estiReq.PlanStartDate,
                    PlanEndDate = estiReq.PlanEndDate,
                    Remarks = estiReq.Remarks,
                    TotalPrice = estiReq.TotalPrice,
                    Created_By = sessionUser.Id
                });

                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CreateEstimateDetailsChangeHistoryResponse> CreateEstimationDetailsHistry(CreateEstimateDetailsChangeHistoryServiceRequest detailHisReq)
        {
            try
            {
                var response = new CreateEstimateDetailsChangeHistoryResponse();
                var sessionUser = await _sessionManager.GetUser();

                int id;

                using var uow = _uowFactory.GetUnitOfWork();
                id = await uow.EstimateDetailsChangeHistoryRepo.CreateEstimateDetailsHistory(new
                    Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo.Models.CreateEstimateDetailsChangeHistoryRequest()
                {
                    Estimation_Id = detailHisReq.Estimation_Id,
                    Item_Id = detailHisReq.Item_Id,
                    NoOfMachineAndUsagesAndTeamAndCar = detailHisReq.NoOfMachineAndUsagesAndTeamAndCar,
                    NoOfDayAndTotalUnit = detailHisReq.NoOfDayAndTotalUnit,
                    QuantityRequired = detailHisReq.QuantityRequired,
                    UnitPrice = detailHisReq.UnitPrice,
                    TotalPrice = detailHisReq.TotalPrice,
                    Remarks = detailHisReq.Remarks,
                    AreaType = detailHisReq.AreaType,
                    ResponsibleDepartment_Id = detailHisReq.ResponsibleDepartment_Id,
                    Thana_Id = detailHisReq.Thana_Id,
                    Created_By = sessionUser.Id
                });

                uow.Commit();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateParticularWiseSummaryResponse> CreateParticularWiseSummary(CreateParticularWiseSummaryServiceRequest partiSummaryReq)
        {
            try
            {
                var response = new CreateParticularWiseSummaryResponse();
                var sessionUser = await _sessionManager.GetUser();

                int id;

                using var uow = _uowFactory.GetUnitOfWork();
                id = await uow.ParticularWiseSummaryRepo.CreateParticularWiseSummary(
                    new Repositories.DatabaseRepos.ParticularWiseSummaryRepo.Models.CreateParticularWiseSummaryRequest()
                    {
                        Particular_Id = partiSummaryReq.Particular_Id,
                        Estimate_Id = partiSummaryReq.Estimate_Id,
                        TotalPrice = partiSummaryReq.TotalPrice,
                        Created_By = sessionUser.Id
                    });

                uow.Commit();
                response.ParticularWiseSummary_Id = id;

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateParticularWiseSummaryHistoryResponse> CreateParticularWiseSummaryHistory(CreateParticularWiseSummaryHistoryServiceRequest partiSummaryReq)
        {
            try
            {
                var response = new CreateParticularWiseSummaryHistoryResponse();
                var sessionUser = await _sessionManager.GetUser();

                int id;

                using var uow = _uowFactory.GetUnitOfWork();
                id = await uow.ParticularWiseSummaryHistoryRepo.CreateParticularWiseSummaryHistory(
                    new Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo.Models.CreateParticularWiseSummaryHistoryRequest()
                    {
                        Particular_Id = partiSummaryReq.Particular_Id,
                        Estimate_Id = partiSummaryReq.Estimate_Id,
                        TotalPrice = partiSummaryReq.TotalPrice,
                        //ParticularWishSummary_Id = partiSummaryReq.ParticularWishSummary_Id,
                        Created_By = sessionUser.Id
                    });

                uow.Commit();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<DepartmentWiseSummaryEntity>> LoadDepartmentWiseSummaryByEstimationId(int estimationId)
        {
            try
            {
                List<DepartmentWiseSummaryEntity> result = new List<DepartmentWiseSummaryEntity>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.DepartmentWiseSummaryRepo.LoadDepartmentWiseSummaryByEstimationId(estimationId);
                }
                return result;
            }
            catch (Exception e) { throw; }
        }

        public async Task<EstimationEntity> GetEstimateById(int id)
        {
            try
            {
                EstimationEntity? estimationEntity = null;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    estimationEntity = await uow.EstimationRepo.GetById(id);
                }
                return estimationEntity;
            }
            catch (Exception e) { throw; }
        }

        public async Task<List<ParticularWiseSummaryEntity>> LoadParticularWiseSummaryByEstimationId(int estimationId)
        {
            try
            {
                List<ParticularWiseSummaryEntity> result = new List<ParticularWiseSummaryEntity>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.ParticularWiseSummaryRepo.LoadParticularWiseSummaryByEstimationId(estimationId);
                }
                return result;
            }
            catch (Exception e) { throw; }
        }

        public async Task<bool> IsValidToShowInParking(int estimationId, int priority)
        {
            try
            {
                int response;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimationRepo.CheckIsValidToShowInParking(estimationId, priority);
                }
                if (response == 0) return true;
                return false;
            }
            catch (Exception e) { throw; }
        }

        public async Task<List<EstimateVM>> LoadAllPendingEstimateByUser(int userId)
        {
            try
            {
                List<EstimateVM> response = new List<EstimateVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimationRepo.LoadAllPendingEstimateByUser(userId);
                }
                return response;
            }
            catch (Exception e) { throw; }
        }

        public async Task<List<EstimateApproverEntity>> LoadEstimateApproverByEstimationId(int estimationId)
        {
            try
            {
                List<EstimateApproverEntity> response = new List<EstimateApproverEntity>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimateApproverRepo.LoadEstimateApproverByEstimationId(estimationId);
                }
                return response;
            }
            catch (Exception e) { throw; }
        }

        public async Task<List<EstimateDetailsEntity>> LoadEstimateDetailByEstimationId(int estimationId)
        {
            try
            {
                List<EstimateDetailsEntity> response = new List<EstimateDetailsEntity>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimateDetailsRepo.LoadEstimateDetailByEstimationId(estimationId);
                }
                return response;
            }
            catch (Exception e) { throw; }
        }

        public Task<ParticularWiseSummaryEntity> GetParticularWiseSummaryByEstimationId(int estimationId)
        {
            throw new NotImplementedException();
        }

        public Task<DepartmentWiseSummaryEntity> GetDepartmentWiseSummaryByEstimationId(int estimationId)
        {
            throw new NotImplementedException();
        }

        public async void CompleteBudget(int id, int userId, int StatusCode)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                using var uow = _uowFactory.GetUnitOfWork();
                await uow.EstimationRepo.CompleteBudget(id, userId, StatusCode);
                uow.Commit();           
            }
            catch (Exception) { throw; }
        }

        public async void CompleteApproverFeedback(int estimateId, int userId, string feedback, string remarks)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                await uow.EstimateApproverRepo.CompleteApproverFeedback(estimateId, userId, feedback, remarks);
                uow.Commit();
            }
            catch (Exception e) { throw; }
        }

        public async Task<int> SaveBudgetData(AddBudgetEstimation requestDto)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                requestDto.Estimation.Status = BaseEntity.EntityStatus.Pending.ToString();
                requestDto.Estimation.CreatedBy = sessionUser.Id;
                int estimateId = 0;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var response = await this.CreateBudgeEstimation(requestDto.Estimation, uow);
                    estimateId = response.budgetEstimateID;
                    requestDto.Estimation.Id = estimateId;

                    await this.CreateEstimateHistory(requestDto.Estimation, uow);
                    await this.CreateBudgetEstimateApprover(requestDto, uow);
                    await this.CreateBudgetEstimateDetails(requestDto, uow);
                    await this.CreateBudgetDepartmentWiseSummary(requestDto, uow);
                    await this.CreateParticularWiseSummary(requestDto, uow);
                    var estimateInfo = await uow.EstimationRepo.SingleEstimationWithType(estimateId);
                    string uniqueIdentifier = estimateInfo.EstimationIdentifier;
                    requestDto.Estimation.UniqueIdentifier = uniqueIdentifier;
                     
                    if(requestDto.Estimation.EstimateType == 7 || requestDto.Estimation.EstimateType == 8)
                    {
                        await this.CreateProcurementApprovalService(requestDto, uow);
                    }
                    
                    uow.Commit();
                }

                using (var uowT = _uowFactory.GetUnitOfWork())
                {
                    if (requestDto.Estimation.Status != BaseEntity.EntityStatus.Draft.ToString())
                    {
                        var user = _userService.GetById(requestDto.EstimateApproverList[0].User_Id).Result;
                        await this.SaveToEmailTable(requestDto, uowT, sessionUser,
                            user.First_Name + " " + user.Last_Name);
                    }

                    uowT.Commit();
                }

                return estimateId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DraftBudgetData(AddBudgetEstimation requestDto)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                requestDto.Estimation.Status = EstimationEntity.EntityStatus.Draft.ToString();
                requestDto.Estimation.CreatedBy = sessionUser.Id;
                int estimateId = 0;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var response = await this.CreateBudgeEstimation(requestDto.Estimation, uow);
                    estimateId = response.budgetEstimateID;
                    requestDto.Estimation.Id = estimateId;

                    await this.CreateEstimateHistory(requestDto.Estimation, uow);
                    if (requestDto.Estimation.EstimateType == 7 || requestDto.Estimation.EstimateType == 8)
                    {
                        await this.CreateProcurementApprovalService(requestDto, uow);
                    }
                    await this.DraftBudgetEstimateApprover(requestDto, uow);
                    await this.CreateBudgetEstimateDetails(requestDto, uow);
                    await this.CreateBudgetDepartmentWiseSummary(requestDto, uow);
                    await this.CreateParticularWiseSummary(requestDto, uow);
                    uow.Commit();
                }
                return estimateId;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<int> CreateParticularWiseSummary(AddBudgetEstimation requestDto, IUnitOfWork uow)
        {
            try
            {
                if (requestDto.ParticularWiseSummary == null) return 1;
                foreach (var partiSummaryReq in requestDto.ParticularWiseSummary)
                {
                    partiSummaryReq.Estimate_Id = requestDto.Estimation.Id;
                    //var responseAfterAddingPartiSummary = await this.CreateParticularWiseSummary(item);

                    int id = await uow.ParticularWiseSummaryRepo.CreateParticularWiseSummary(
                    new CreateParticularWiseSummaryRequest()
                    {
                        Particular_Id = partiSummaryReq.Particular_Id,
                        Estimate_Id = partiSummaryReq.Estimate_Id,
                        TotalPrice = partiSummaryReq.TotalPrice,
                        Created_By = requestDto.Estimation.CreatedBy
                    });

                    await uow.ParticularWiseSummaryHistoryRepo.CreateParticularWiseSummaryHistory(
                    new Repositories.DatabaseRepos.ParticularWiseSummaryHistoryRepo.Models.CreateParticularWiseSummaryHistoryRequest()
                    {
                        Particular_Id = partiSummaryReq.Particular_Id,
                        Estimate_Id = partiSummaryReq.Estimate_Id,
                        TotalPrice = partiSummaryReq.TotalPrice,
                        Created_By = requestDto.Estimation.CreatedBy
                    });
                }
                return 1;
            }
            catch (Exception e) { throw; }
        }

        private async Task<int> CreateBudgetDepartmentWiseSummary(AddBudgetEstimation requestDto, IUnitOfWork uow)
        {
            try
            {
                if (requestDto.DepartmentWiseSummary == null) return 1;
                foreach (var deptSummaryReq in requestDto.DepartmentWiseSummary)
                {
                    deptSummaryReq.Estimate_Id = requestDto.Estimation.Id;

                    await uow.DepartmentWiseSummaryRepo.CreateDepartmentWiseSummary(
                   new Repositories.DatabaseRepos.DepartmentWiseSummaryRepo.Models.CreateDepartmentWiseSummaryRequest()
                   {
                       Department_Id = deptSummaryReq.Department_Id,
                       TotalPrice = deptSummaryReq.TotalPrice,
                       Estimate_Id = deptSummaryReq.Estimate_Id,
                       Created_By = requestDto.Estimation.CreatedBy
                   });

                    await uow.DepartmentWiseSummaryHistoryRepo.CreateDepartmentWiseSummaryHistory(
                    new Repositories.DatabaseRepos.DepartmentWiseSummaryHistoryRepo.Models.CreateDepartmentWiseSummaryHistoryRequest()
                    {
                        Department_Id = deptSummaryReq.Department_Id,
                        TotalPrice = deptSummaryReq.TotalPrice,
                        Estimate_Id = deptSummaryReq.Estimate_Id,
                        Created_By = requestDto.Estimation.CreatedBy
                    });
                }
                return 1;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<int> CreateBudgetEstimateDetails(AddBudgetEstimation requestDto, IUnitOfWork uow)
        {
            try
            {
                if (requestDto.EstimateDetails == null) return 1;
                foreach (var detailsReq in requestDto.EstimateDetails)
                {
                    detailsReq.Estimation_Id = requestDto.Estimation.Id;

                    int detailId = 0;
                    detailId = await uow.EstimateDetailsRepo.CreateEstimateDetails(new CreateEstimateDetailsRequest()
                    {
                        Estimation_Id = detailsReq.Estimation_Id,
                        Item_Id = detailsReq.Item_Id,
                        NoOfMachineAndUsagesAndTeamAndCar = detailsReq.NoOfMachineAndUsagesAndTeamAndCar,
                        NoOfDayAndTotalUnit = detailsReq.NoOfDayAndTotalUnit,
                        QuantityRequired = detailsReq.QuantityRequired,
                        UnitPrice = detailsReq.UnitPrice,
                        TotalPrice = detailsReq.TotalPrice,
                        Remarks = detailsReq.Remarks,
                        AreaType = detailsReq.AreaType,
                        ResponsibleDepartment_Id = detailsReq.ResponsibleDepartment_Id,
                        Thana_Id = detailsReq.Thana_Id,
                        Created_By = requestDto.Estimation.CreatedBy
                    });

                    await uow.EstimateDetailsChangeHistoryRepo.CreateEstimateDetailsHistory(new
                    Repositories.DatabaseRepos.EstimateDetailsChangeHistoryRepo.Models.CreateEstimateDetailsChangeHistoryRequest()
                    {
                        Estimation_Id = detailsReq.Estimation_Id,
                        Item_Id = detailsReq.Item_Id,
                        NoOfMachineAndUsagesAndTeamAndCar = detailsReq.NoOfMachineAndUsagesAndTeamAndCar,
                        NoOfDayAndTotalUnit = detailsReq.NoOfDayAndTotalUnit,
                        QuantityRequired = detailsReq.QuantityRequired,
                        UnitPrice = detailsReq.UnitPrice,
                        TotalPrice = detailsReq.TotalPrice,
                        Remarks = detailsReq.Remarks,
                        AreaType = detailsReq.AreaType,
                        ResponsibleDepartment_Id = detailsReq.ResponsibleDepartment_Id,
                        Thana_Id = detailsReq.Thana_Id,
                        Created_By = requestDto.Estimation.CreatedBy
                    });
                }
                return 1;
            }
            catch (Exception e) { throw; }
        }

        private async Task<int> CreateBudgetEstimateApprover(AddBudgetEstimation requestDto, IUnitOfWork uow)
        {
            try
            {
                foreach (var approverReq in requestDto.EstimateApproverList)
                {
                    approverReq.Estimate_Id = requestDto.Estimation.Id;
                    
                    int approverId = 0;
                    if (approverReq.RolePriority_Id == 3)
                        approverReq.Status = BaseEntity.EntityStatus.Completed.ToString();
                    else
                        approverReq.Status = EstimateApproverEntity.EntityStatus.Pending.ToString();

                    approverId = await uow.EstimateApproverRepo.CreateEstimateApprover(new CreateEstimateApproverRequest()
                    {
                        Estimate_Id = approverReq.Estimate_Id,
                        User_Id = approverReq.User_Id,
                        Priority = approverReq.Priority,
                        Status = approverReq.Status,
                        Remarks = approverReq.Remarks,
                        RolePriority_Id = approverReq.RolePriority_Id,
                        Created_By = requestDto.Estimation.CreatedBy,
                        PlanDate = approverReq.ExpectedTime
                    });

                    await uow.EstimateApproverChangeHistoryRepo.CreateEstimateApproverChangeHistory(
                    new Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Models.CreateEstimateApproverChangeHistoryRequest()
                    {
                        Estimate_Id = approverReq.Estimate_Id,
                        //EstimateApprover_Id = approverId,
                        User_Id = approverReq.User_Id,
                        Priority = approverReq.Priority,
                        Status = approverReq.Status,
                        Remarks = approverReq.Remarks,
                        RolePriority_Id = approverReq.RolePriority_Id,
                        Created_By = requestDto.Estimation.CreatedBy,
                        PlanDate = approverReq.ExpectedTime
                    });

                }
                return 1;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<int> CreateProcurementApprovalService(AddBudgetEstimation dto, IUnitOfWork uow)
        {
            try
            {
                var response = await uow.ProcurementApprovalRepo.CreateProcurementApproval(
                    new Repositories.DatabaseRepos.ProcurementApproval.Models.CreateProcurementApprovalRequest()
                    {
                        EstimationId = dto.Estimation.Id,
                        PAReferenceNo = dto.ProcurementApprovalRequest.PAReferenceNo,
                        TitleOfPRorRFQ = dto.ProcurementApprovalRequest.TitleOfPRorRFQ,
                        RFQReferenceNo = dto.ProcurementApprovalRequest.RFQReferenceNo,
                        PRReferenceNo = dto.ProcurementApprovalRequest.PRReferenceNo,
                        NameOfRequester = dto.ProcurementApprovalRequest.NameOfRequester,
                        DepartmentId = dto.ProcurementApprovalRequest.DepartmentId,
                        RFQProcess = dto.ProcurementApprovalRequest.RFQProcess,
                        SourcingMethod = dto.ProcurementApprovalRequest.SourcingMethod,
                        NameOfRecommendedSupplier = dto.ProcurementApprovalRequest.NameOfRecommendedSupplier,
                        PurchaseValue = dto.ProcurementApprovalRequest.PurchaseValue,
                        SavingAmount = dto.ProcurementApprovalRequest.SavingAmount,
                        SavingType = dto.ProcurementApprovalRequest.SavingType,
                        Created_By = dto.Estimation.CreatedBy

                    });

                return response;
            }
            catch(Exception e)
            {
                throw;
            }
        }

        private async Task<int> CreateEstimateHistory(CreateEstimateRequest estimation, IUnitOfWork uow)
        {
            try
            {
                return await uow.EstimateChangeHistoryRepo.CreateEstimationHistory(
                      new Repositories.DatabaseRepos.EstimateChangeHistoryRepo.Models.CreateEstimationHistoryRequest()
                      {
                          EstimateType_Id = estimation.EstimateType,
                          Status = estimation.Status,
                          SystemID = estimation.SystemID,
                          ProjectId = estimation.Project_Id,
                          Subject = estimation.Subject,
                          Objective = estimation.Objective,
                          Details = estimation.Details,
                          PlanStartDate = estimation.PlanStartDate,
                          PlanEndDate = estimation.PlanEndDate,
                          Remarks = estimation.Remarks,
                          TotalPrice = estimation.TotalPrice,
                          Estimate_Id = estimation.Id,
                          TotalPriceRemarks = estimation.TotalPriceRemarks
                      });
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Task<CreateEstimateResponse> CreateBudgeEstimation(CreateEstimateRequest estimateBudget)
        {
            throw new NotImplementedException();
        }

        public async Task<SingleEstimationWithTypeResponse> GetOneEstimationWithType(int estiId)
        {
            try
            {
                var response = new SingleEstimationWithTypeResponse();

                using var uow = _uowFactory.GetUnitOfWork();
                response.Estimation = await uow.EstimationRepo.SingleEstimationWithType(estiId);

                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<ParticularWiseSummaryDetailsByEstimationId>> LoadParticularSummaryDetails(int estimationId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.ParticularWiseSummaryRepo.LoadParticularWiseSummaryDetailsByEstimationId(estimationId);
                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<DepartWiseSummaryDetailsByEstimationId>> LoadDepartmentSummaryDetails(int estimationId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.DepartmentWiseSummaryRepo.LoadDepartmentWiseSummaryDetailsByEstimationId(estimationId);
                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DisabledEstimationResponse> DisabledResponse(int estimationId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var response = new DisabledEstimationResponse();

                EstimationEntity? estimation;
                using var uow = _uowFactory.GetUnitOfWork();
                estimation = await uow.EstimationRepo.GetById(estimationId);

                await uow.EstimationRepo.DisabledEstimation(new DisableEstimation()
                {
                    EsimtationId = estimation.Id,
                    Updated_By = sessionUser.Id
                });

                uow.Commit();

                response.Notifications.Add($"Estimation '{estimation.Id}' has been disabled", NotificationTypeEnum.Success);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<EstimateEditVM> CastEstimateToVM(EstimationEntity estimate, List<EstimateApproverEntity> estimateApprovers,
            List<EstimateDetailsEntity> estimateDetails = null, List<ParticularWiseSummaryEntity> particularWiseSummary = null,
            List<DepartmentWiseSummaryEntity> departmentWiseSummary = null)
        {
            try
            {
                var latestApproversLevel = await _budgetApproverService.GetUpcomingPendingApproverLevel(estimate.Id);
                EstimateEditVM editVM = new EstimateEditVM();
                //editVM.ProjectName = _budgetService.GetByProjectId(estimate.Project_Id);
                editVM.PlanEndDate = estimate.PlanEndDate;
                editVM.EstimationIdentity = estimate.UniqueIdentifier;
                editVM.Subject = estimate.Subject;
                editVM.Objective = estimate.Objective;
                editVM.Details = estimate.Details;
                editVM.PlanStartDate = estimate.PlanStartDate;
                editVM.PlanEndDate = estimate.PlanEndDate;
                editVM.Remarks = estimate.Remarks;
                editVM.TotalPrice = estimate.TotalPrice;
                editVM.Id = estimate.Id;
                editVM.EncryptedId = _protector.Protect(estimate.Id.ToString());
                editVM.Status = estimate.Status;
                editVM.NextPriority = latestApproversLevel;
                editVM.CurrencyType = estimate.CurrencyType;

                editVM.CreatedBy = await _userService.GetById(estimate.Created_By);
                editVM.CreatedTime = estimate.Created_Date;

                //TODO: Load estimate type from table
                editVM.EstimateType = estimate.EstimateType_Id.ToString();
                foreach (var item in estimateApprovers)
                {
                    var user = _userService.GetById(item.User_Id).Result;
                    EstimateApproverVM approverVM = new EstimateApproverVM();
                    approverVM.Estimate_Id = item.Estimate_Id;
                    approverVM.Priority = item.Priority;
                    approverVM.Status = item.Status;
                    approverVM.UserName = user.Username;
                    approverVM.UserFullName = user.First_Name + " " + user.Last_Name;
                    approverVM.UserDept = _departmentService.GetById(user.Department_Id).Result.Name;
                    if (approverVM.Status == BaseEntity.EntityStatus.Completed.ToString() || approverVM.Status == BaseEntity.EntityStatus.Reject.ToString() || approverVM.Status == BaseEntity.EntityStatus.CR.ToString())
                    {
                        EstimateApproverFeedbackEntity estimateApproverFeedbackEntity = await _budgetApproverService.GetFeedbackByEstimationandUserId(approverVM.Estimate_Id, user.Id, Int16.Parse(approverVM.Status));
                        if (estimateApproverFeedbackEntity != null)
                            approverVM.ResponseTime = estimateApproverFeedbackEntity.Created_Date.ToString("MM/dd/yyyy hh:mm tt");
                        //approverVM.ResponseTime = await _budgetApproverService.GetFeedbackByEstimationandUserId(); //GetApproverByEstimateAndUser
                    }
                    editVM.EstimateApproverList.Add(approverVM);
                }

                return editVM;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<BudgetEstimationResponse> UpdateEstimation(BudgetEstimationUpdateRequest request, int requestStatus)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var response = new BudgetEstimationResponse();

                if (sessionUser == null)
                {
                    response.Notifications.Add($"User is not loged in", NotificationTypeEnum.Information);
                    return response;
                }


                EstimationEntity? estimation;
                using var uow = _uowFactory.GetUnitOfWork();
                estimation = await uow.EstimationRepo.GetById(request.EstimateId);

                if (estimation == null)
                {
                    response.Notifications.Add($"Estimation '{estimation.Id}' has not been found", NotificationTypeEnum.Warning);
                    return response;
                }

                await uow.EstimationRepo.UpdateEstimation(new UpdateEstimationRequest()
                {
                    estimationId = request.EstimateId,
                    EstimateType = request.EstimateType,
                    CurrencyType = request.CurrencyType,
                    Status = requestStatus.ToString(),
                    Subject = request.Subject,
                    Objective = request.Objective,
                    Details = request.Details,
                    PlanStart = request.PlanStart,
                    PlanEnd = request.PlanEnd,
                    TotalPrice = request.TotalPrice,
                    Updated_By = sessionUser.Id,
                    TotalPriceRemarks = request.TotalPriceRemarks
                });
                uow.Commit();

                response.Notifications.Add($"Estimation '{estimation.Id}' has been updated", NotificationTypeEnum.Success);
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<EstimateEditVM>> LoadAllPendingEstimate(int userId, int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                //var sessionUser = await _sessionManager.GetUser();

                //if (sessionUser == null)
                //{
                //    if (sessionUser == null) throw new Exception("user session expired");
                //}

                List<EstimateEditVM> response = new List<EstimateEditVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimationRepo.LoadAllPendingEstimate(userId, currentPageIndex, pAGE_SIZE, userId);
                }
                return response;
            }
            catch (Exception e) { throw; }
        }
        
        public async Task<List<EstimateEditVM>> LoadAllCompleteEstimate(int userId, int currentPageIndex,
            int pAGE_SIZE, string whereClause, bool IsNotForCount = true)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                
                List<EstimateEditVM> response = new List<EstimateEditVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    //response = await uow.EstimationRepo.LoadAllCompleteEstimateExceptFinance(userId, 
                    //    currentPageIndex, pAGE_SIZE, sessionUser.Id, whereClause, IsNotForCount);
                    if (sessionUser.Department_Id == 19) // check the login user is from Finance
                    {
                        response = await uow.EstimationRepo.LoadAllCompleteEstimate(userId, currentPageIndex, pAGE_SIZE, sessionUser.Id, whereClause, IsNotForCount);
                    }
                    else
                    {
                        response = await uow.EstimationRepo.LoadAllCompleteEstimateExceptFinance(userId, currentPageIndex, pAGE_SIZE, sessionUser.Id, whereClause, IsNotForCount);
                    }
                       

                }
                return response;
            }
            catch (Exception e) { throw; }
        }

        public async Task<int> SaveEstimationDetailsApproversSummaries(AddBudgetEstimation requestDto)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                requestDto.Estimation.Status = EstimationEntity.EntityStatus.Pending.ToString();
                requestDto.Estimation.CreatedBy = sessionUser.Id;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    await this.CreateEstimateHistory(requestDto.Estimation, uow);
                    await this.CreateBudgetEstimateApprover(requestDto, uow);
                    await this.CreateBudgetEstimateDetails(requestDto, uow);
                    await this.CreateBudgetDepartmentWiseSummary(requestDto, uow);
                    await this.CreateParticularWiseSummary(requestDto, uow);
                    uow.Commit();
                }

                return requestDto.Estimation.Id;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<int> ReDraftEstimationDetailsApproversSummaries(AddBudgetEstimation requestDto)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                requestDto.Estimation.Status = EstimationEntity.EntityStatus.Draft.ToString();
                requestDto.Estimation.CreatedBy = sessionUser.Id;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    await this.CreateEstimateHistory(requestDto.Estimation, uow);
                    await this.DraftBudgetEstimateApprover(requestDto, uow);
                    await this.CreateBudgetEstimateDetails(requestDto, uow);
                    await this.CreateBudgetDepartmentWiseSummary(requestDto, uow);
                    await this.CreateParticularWiseSummary(requestDto, uow);
                    uow.Commit();
                }
                return requestDto.Estimation.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> DraftBudgetEstimateApprover(AddBudgetEstimation requestDto, IUnitOfWork uow)
        {
            try
            {
                if (requestDto.EstimateApproverList == null) return 1;
                foreach (var approverReq in requestDto.EstimateApproverList)
                {
                    approverReq.Estimate_Id = requestDto.Estimation.Id;
                    approverReq.Status = EstimateApproverEntity.EntityStatus.Draft.ToString();
                    int approverId = 0;
                    approverId = await uow.EstimateApproverRepo.CreateEstimateApprover(new CreateEstimateApproverRequest()
                    {
                        Estimate_Id = approverReq.Estimate_Id,
                        User_Id = approverReq.User_Id,
                        Priority = approverReq.Priority,
                        Status = approverReq.Status,
                        Remarks = approverReq.Remarks,
                        RolePriority_Id = approverReq.RolePriority_Id,
                        Created_By = requestDto.Estimation.CreatedBy,
                        PlanDate = approverReq.ExpectedTime
                    });

                    await uow.EstimateApproverChangeHistoryRepo.CreateEstimateApproverChangeHistory(
                    new Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Models.CreateEstimateApproverChangeHistoryRequest()
                    {
                        Estimate_Id = approverReq.Estimate_Id,
                        //EstimateApprover_Id = approverId,
                        User_Id = approverReq.User_Id,
                        Priority = approverReq.Priority,
                        Status = approverReq.Status,
                        Remarks = approverReq.Remarks,
                        RolePriority_Id = approverReq.RolePriority_Id,
                        Created_By = requestDto.Estimation.CreatedBy,
                        PlanDate = approverReq.ExpectedTime
                    });

                }
                return 1;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<EstimationAttachmentEntity>> LoadAttachmentsByEstimateId(int estimateId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.EstimationAttachment.LoadAttachmentsByEstimate(estimateId);
                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GetDraftBudgetEstimationbyUserResponse> LoadDraftEstimation(int userId, int start, int pAGE_SIZE)
        {
            try
            {
                var response = new GetDraftBudgetEstimationbyUserResponse();
                var sessionUser = await _sessionManager.GetUser();

                using var uow = _uowFactory.GetUnitOfWork();

                response.DraftedBudgest = await uow.EstimationRepo.LoadDraftEstimation(userId, start, pAGE_SIZE);

                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GetDraftBudgetEstimationbyUserResponse> LoadCREstimation(int userId, int start, int pAGE_SIZE)
        {
            try
            {
                var response = new GetDraftBudgetEstimationbyUserResponse();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response.DraftedBudgest = await uow.EstimationRepo.LoadCREstimation(userId, start, pAGE_SIZE);
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task DeleteEstimationDetails(EstimationEntity request, IUnitOfWork uow)
        {
            try
            {
                await uow.EstimateDetailsRepo.DeleteEstimateDetailsByEstimate(request.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task UpdateEstimationTotalPrice(EstimationEntity request, IUnitOfWork uow)
        {
            try
            {
                await uow.EstimationRepo.UpdateTotalPrice(new UpdateEstimateTotalPriceRequest()
                {
                    id = request.Id,
                    totalPrice = request.TotalPrice
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ReDefineEstimationDetailsAndSummaries(AddBudgetEstimation requestDto)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                {
                    throw new Exception("user session expired");
                }
                requestDto.Estimation.CreatedBy = sessionUser.Id;
                EstimationEntity? estimationdb;
                using var uow = _uowFactory.GetUnitOfWork();
                estimationdb = await uow.EstimationRepo.GetById(requestDto.Estimation.Id);
                if (estimationdb == null)
                {
                    throw new Exception("Estimation Not found");
                }

                await this.DeleteEstimationDetails(estimationdb, uow);

                await uow.DepartmentWiseSummaryRepo.DeleteDepartmentSummaryByEstimate(estimationdb.Id);
                await uow.ParticularWiseSummaryRepo.DeleteParticularSummaryByEstimateId(estimationdb.Id);

                await this.CreateBudgetEstimateDetails(requestDto, uow);
                await this.CreateBudgetDepartmentWiseSummary(requestDto, uow);
                await this.CreateParticularWiseSummary(requestDto, uow);
                estimationdb.TotalPrice = requestDto.Estimation.TotalPrice;
                await this.UpdateEstimationTotalPrice(estimationdb, uow);

                uow.Commit();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ReDefineEsitmationDetailsApproversAndSummariesService(ModifyEstimationAndReAddOther request)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                {
                    throw new Exception("user session expired");
                }
                request.ReAddBudgetDetailsAndOthers.Estimation.CreatedBy = sessionUser.Id;
                request.ReAddBudgetDetailsAndOthers.Estimation.Status = request.EstimationStatus.ToString();
                EstimationEntity? estimationdb;
                using var uow = _uowFactory.GetUnitOfWork();
                estimationdb = await uow.EstimationRepo.GetById(request.EstimationId);
                if (estimationdb == null)
                {
                    throw new Exception("Estimation Not found");
                }

                request.ReAddBudgetDetailsAndOthers.Estimation.UniqueIdentifier = estimationdb.UniqueIdentifier;
                

                await this.UpdateEstimationSingleTransaction(request.BudgetEstimationUpdateRequest, request.EstimationStatus, sessionUser.Id, uow);
                await this.DeleteProcurementApprovalByEstimateService(estimationdb.Id, uow);
                await this.DeleteAttachments(request.AttachmentsToRemove, uow);
                await this.DeleteDetailApproverHistory(estimationdb.Id, uow);
                if (request.ReAddBudgetDetailsAndOthers.Estimation.EstimateType == EstimationType.PROCUREMENT || request.ReAddBudgetDetailsAndOthers.Estimation.EstimateType == EstimationType.Factsheet)
                {
                    request.ReAddBudgetDetailsAndOthers.ProcurementApprovalRequest.EstimationId = estimationdb.Id;
                    await this.CreateProcurementApprovalService(request.ReAddBudgetDetailsAndOthers, uow);
                }
                await this.SaveEstimationDetailsApproversSummariesSingleTransaction(request.ReAddBudgetDetailsAndOthers, uow, sessionUser);
                //uow.Commit();


            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task DeleteProcurementApprovalByEstimateService(int estimateId, IUnitOfWork uow)
        {
            try
            {
                await uow.ProcurementApprovalRepo.DeleteProcurementApprovalByEstimateID(estimateId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task DeleteAttachments(List<int> attachementIDs, IUnitOfWork uow)
        {
            try
            {
                foreach(var id in attachementIDs)
                {
                    await uow.EstimationAttachment.DeleteAttachment(id);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task DeleteDetailApproverHistory(int estimateId, IUnitOfWork uow)
        {
            try
            {
                await uow.DepartmentWiseSummaryRepo.DeleteDepartmentSummaryByEstimate(estimateId);
                await uow.ParticularWiseSummaryRepo.DeleteParticularSummaryByEstimateId(estimateId);
                await uow.EstimateDetailsRepo.DeleteEstimateDetailsByEstimate(estimateId);
                await uow.EstimateApproverRepo.DeleteApproverByEstimate(estimateId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task UpdateEstimationSingleTransaction(BudgetEstimationUpdateRequest request, int requestStatus, int sessionUserId ,IUnitOfWork uow)
        {
            try
            {
                //var sessionUser = await _sessionManager.GetUser();
                //if (sessionUser == null)
                //{
                //    throw new Exception("user session expired");
                //}
                await uow.EstimationRepo.UpdateEstimation(new UpdateEstimationRequest()
                {
                    estimationId = request.EstimateId,
                    EstimateType = request.EstimateType,
                    CurrencyType = request.CurrencyType,
                    Status = requestStatus.ToString(),
                    Subject = request.Subject,
                    Objective = request.Objective,
                    Details = request.Details,
                    PlanStart = request.PlanStart,
                    PlanEnd = request.PlanEnd,
                    TotalPrice = request.TotalPrice,
                    Updated_By = sessionUserId,
                    TotalPriceRemarks = request.TotalPriceRemarks
                    
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task SaveEstimationDetailsApproversSummariesSingleTransaction(AddBudgetEstimation requestDto, IUnitOfWork uow, UserEntity sessionUser)
        {
            try
            {
                await this.CreateEstimateHistory(requestDto.Estimation, uow);
                await this.AddBudgetEstimateApprover(requestDto, uow);
                await this.CreateBudgetEstimateDetails(requestDto, uow);
                await this.CreateBudgetDepartmentWiseSummary(requestDto, uow);
                await this.CreateParticularWiseSummary(requestDto, uow);
                uow.Commit();
                using (var uowT = _uowFactory.GetUnitOfWork())
                {
                    if (requestDto.Estimation.Status != BaseEntity.EntityStatus.Draft.ToString())
                    {
                        //var user = _userService.GetById(requestDto.EstimateApproverList[0].User_Id).Result;
                        await this.SaveToEmailTable(requestDto, uow, sessionUser,
                            sessionUser.First_Name + " " + sessionUser.Last_Name);
                        //var t = "";
                        
                    }

                    
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> AddBudgetEstimateApprover(AddBudgetEstimation requestDto, IUnitOfWork uow)
        {
            try
            {
                foreach (var approverReq in requestDto.EstimateApproverList)
                {
                    approverReq.Estimate_Id = requestDto.Estimation.Id;
                    if (approverReq.RolePriority_Id == 3)
                        approverReq.Status = BaseEntity.EntityStatus.Completed.ToString();
                    else
                        approverReq.Status = EstimateApproverEntity.EntityStatus.Pending.ToString();
                    int approverId = 0;
                    approverId = await uow.EstimateApproverRepo.CreateEstimateApprover(new CreateEstimateApproverRequest()
                    {
                        Estimate_Id = approverReq.Estimate_Id,
                        User_Id = approverReq.User_Id,
                        Priority = approverReq.Priority,
                        Status = approverReq.Status,
                        Remarks = approverReq.Remarks,
                        RolePriority_Id = approverReq.RolePriority_Id,
                        Created_By = requestDto.Estimation.CreatedBy,
                        PlanDate = approverReq.ExpectedTime
                    });

                    await uow.EstimateApproverChangeHistoryRepo.CreateEstimateApproverChangeHistory(
                    new Repositories.DatabaseRepos.EstimateApproverChangeHistoryRepo.Models.CreateEstimateApproverChangeHistoryRequest()
                    {
                        Estimate_Id = approverReq.Estimate_Id,
                        //EstimateApprover_Id = approverId,
                        User_Id = approverReq.User_Id,
                        Priority = approverReq.Priority,
                        Status = approverReq.Status,
                        Remarks = approverReq.Remarks,
                        RolePriority_Id = approverReq.RolePriority_Id,
                        Created_By = requestDto.Estimation.CreatedBy,
                        PlanDate = approverReq.ExpectedTime
                    });

                }
                return 1;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<EstimationSearch>> GetSearchService(string keyWord)
        {
            var sessionUser = await _sessionManager.GetUser();
            if (sessionUser == null) throw new Exception("user session expired");

            using var uow = _uowFactory.GetUnitOfWork();
            var response = await uow.EstimationRepo.GetEstimateSearch(keyWord, sessionUser.Id);
            uow.Commit();

            return response;
        }

        public async Task<List<EstimateEditVM>> LoadRejectedEstimate(int userId, int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                if (sessionUser == null) throw new Exception("user session expired");

                List<EstimateEditVM> response = new List<EstimateEditVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimationRepo.LoadRejectedEstimate(userId, currentPageIndex, pAGE_SIZE, sessionUser.Id);
                }
                return response;
            }
            catch (Exception) { throw; }
        }

        private async Task<int> SaveToEmailTable(AddBudgetEstimation requestDto, IUnitOfWork uow, UserEntity loggedUser, string approverUser)
        {
            try
            {
                var requestTOSave = new CreateEmailContantRequest();
                var allStackHolder = await this.GetLatestApproversForNewBudget(requestDto.EstimateApproverList);

                requestTOSave.ToEmail = allStackHolder[0];
                requestTOSave.ToCc = loggedUser.Email_Address /*+ "," + allStackHolder[1]*/;
                string message =
                    "This is inform you that "+ EstimationType.GetEstimationTypeByTypeId(requestDto.Estimation.EstimateType) + " has been initiated.Please check and respose accordingly.";
                requestTOSave.Subject = "AMS- Approval Initiating ["+ requestDto.Estimation.UniqueIdentifier + "]: " + requestDto.Estimation.Subject;
               // requestTOSave.Body = await this.GetEmailBody(requestDto, approverUser);
               requestTOSave.Body = await _htmlGeneratorService.getNewEstimateInitEmailBody(requestDto.Estimation.Id, message);
                requestTOSave.CreatedBy = loggedUser.Id;
                requestTOSave.ModifiedBy = loggedUser.Id;
                requestTOSave.AMSID = requestDto.Estimation.UniqueIdentifier;
                requestTOSave.Department = loggedUser.Department_Id;

                var response = await uow.EmailRepo.SaveForEmailServer(requestTOSave);
                uow.Commit();
                return 1;
            }
            catch (Exception) { throw; }
        }

        private async Task<List<string>> GetLatestApproversForNewBudget(List<CreateEstimateApproverServiceRequest> requestList)
        {
            try
            {
                var responseList = new List<string?>();
                var response = "";
                var responseToCC = "";
                var lowestPriority = 0;
                foreach (var item in requestList)
                {
                    if (item.Priority > lowestPriority && item.Priority != 400)
                        lowestPriority = item.Priority;
                }
                foreach (var item in requestList)
                {
                    if (item.Priority == lowestPriority)
                    {
                        using var uowUnder = _uowFactory.GetUnitOfWork();
                        var user = await uowUnder.UserRepo.GetUserById(new Repositories.DatabaseRepos.UserRepo.Models.GetUserByIdRequest() { Id = item.User_Id });

                        if (item.Priority == 1)
                        {
                            response = "asif.ahmed@summitcommunications.net";//"sayed.nazmul@summitcommunications.net";
                        }

                        else
                        {
                            if (response == "")
                                response += user.Email_Address;

                            else
                                response = response + "," + user.Email_Address;
                        }
                    }
                    else
                    {
                        using var uowUnder = _uowFactory.GetUnitOfWork();
                        var user = await uowUnder.UserRepo.GetUserById(new Repositories.DatabaseRepos.UserRepo.Models.GetUserByIdRequest() { Id = item.User_Id });

                        if (item.Priority == 1)
                        {
                            if(responseToCC == "")
                            {
                                responseToCC = "asif.ahmed@summitcommunications.net";//"sayed.nazmul@summitcommunications.net";
                            }
                            else
                                responseToCC = responseToCC + "," + "asif.ahmed@summitcommunications.net";//"sayed.nazmul@summitcommunications.net";
                        }
                        else
                        {
                            if (responseToCC == "")
                                responseToCC += user.Email_Address;

                            else
                                responseToCC = responseToCC + "," + user.Email_Address;
                        }
                    }
                }
                responseList.Add(response);
                responseList.Add(responseToCC);
                return responseList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<string> GetEmailBody(AddBudgetEstimation requestBody, string approverUser)
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
                string itemApproversInSingleRow = "";
                string deptSummaryInSingleRow = "";
                string deptSummary = "";
                string partiSummaryInSingleRow = "";
                string partiSummary = "";
                #endregion

                if (requestBody.Estimation.EstimateType == 7)
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
                if (IsEmpty(requestBody.EstimateDetails))
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
                if (IsEmpty(requestBody.DepartmentWiseSummary))
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
                if (IsEmpty(requestBody.ParticularWiseSummary))
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

                itemApprovers = @"<tr align='center' style='background-color:khaki;'><td>" + userWithDeptInfo.First_Name + " " + userWithDeptInfo.Last_Name  + "</td><td>Creator</td><td>"+ userWithDeptInfo.DepartmentName + "</td><td></td></tr>";
                foreach (var item in requestBody.EstimateApproverList)
                {
                    itemApproversInSingleRow = @"<tr align='center'><td>" + item.ApproverFullName + "</td><td>" + item.ApproverRole + "</td><td>" + item.ApproverDepartment + "</td><td>" + item.ExpectedTime + "</td></tr>";
                    itemApprovers = itemApprovers + itemApproversInSingleRow;
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
                string upperBodyPart = @"<p>A <b>" + estimateType + @"</b> has been initiated by " + userWithDeptInfo.First_Name + " " + userWithDeptInfo.Last_Name + "/"+ userWithDeptInfo.DepartmentName + ".Please see the below information.</p>";

                body = GetEmailBodyForCreatingApproval.GenericEmailBody(upperBodyPart, estimateType, requestBody.Estimation.UniqueIdentifier, requestBody.Estimation.Subject, requestBody.Estimation.Objective,
                     requestBody.Estimation.Details, requestBody.Estimation.PlanStartDate, requestBody.Estimation.PlanEndDate,
                     itemDetailsTable, requestBody.Estimation.TotalPrice.ToString("C", CultureInfo.CreateSpecificCulture("bn-BD")), deptSummary, partiSummary, itemApprovers, "",procurementApprovalPart);
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

        public async Task<GetProcurementApprovalResponse> GetProcurementApprovalByEstimateService(int estimateId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.ProcurementApprovalRepo.GetProcurementApprovalByEstimateId(estimateId);

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AddBudgetEstimation> GetEstimationFullInfo(int estimationId)
        {
            var sessionUser = await _sessionManager.GetUser();

            if (sessionUser == null)
            {
                if (sessionUser == null) throw new Exception("user session expired");
            }

            using var uow = _uowFactory.GetUnitOfWork();
            var estimateInfo = await uow.EstimationRepo.SingleEstimationWithType(estimationId);
            if (estimateInfo == null)
                throw new Exception("Invalid Estimate ID.");
            var procurementApprovalOrFactsheet = await uow.ProcurementApprovalRepo.GetProcurementApprovalByEstimateId(estimationId);
            var estimateItemDetails = await uow.EstimateDetailsRepo.LoadEstimationDetailsWithOtherInformationsByEstimationId(estimationId);
            var estimateDeptSummaryList = await uow.DepartmentWiseSummaryRepo.LoadDepartmentWiseSummaryDetailsByEstimationId(estimationId);
            var estimatePartiSummaryList = await uow.ParticularWiseSummaryRepo.LoadParticularWiseSummaryDetailsByEstimationId(estimationId);
            var estimateApproverList = await uow.EstimateApproverRepo.LoadEstimateApproverDetailsByEstimationId(estimationId);
            var estimateApproverFeedBackList = await uow.EstimateApproverFeedbackRepo.LoadApproverFeedBackDetails(estimationId);

            var result = new AddBudgetEstimation();

            #region Estimate Basic Information
            var resultEsimate = new CreateEstimateRequest();

            resultEsimate.Id = estimationId;
            resultEsimate.EstimateTypeName = estimateInfo.EstimationTypeName;
            resultEsimate.UniqueIdentifier = estimateInfo.EstimationIdentifier;
            resultEsimate.Subject = estimateInfo.EstimationSubject;
            resultEsimate.Objective = estimateInfo.EstimationObjective;
            resultEsimate.Details = estimateInfo.EstimationDetails;
            resultEsimate.PlanStartDate = estimateInfo.EstimationPlanStartDate.ToString();
            resultEsimate.PlanEndDate = estimateInfo.EstimationPlanEndDate.ToString();
            resultEsimate.TotalPrice = estimateInfo.EstimaionTotalPrice;
            resultEsimate.Status = estimateInfo.EstimationStatus;
            resultEsimate.CreatedByName = estimateInfo.CreateorFullName;
            resultEsimate.CreationDate = estimateInfo.CreatedDate;
            resultEsimate.CreatedBy = estimateInfo.CreatorID;
            resultEsimate.TotalPriceRemarks = estimateInfo.TotalPriceRemarks;
            resultEsimate.EstimateType = estimateInfo.EstimationTypeId;
            resultEsimate.CurrencyType = estimateInfo.CurrencyType;

            result.Estimation = resultEsimate;
            #endregion

            #region Factsheet or Procurment

            if (estimateInfo.EstimationTypeId == EstimationType.Factsheet ||
                estimateInfo.EstimationTypeId == EstimationType.PROCUREMENT)
            {
                var factsheetOrProcument = new CreateProcurementApprovalServiceRequest();
                factsheetOrProcument.PAReferenceNo = procurementApprovalOrFactsheet.PAReferenceNo;
                factsheetOrProcument.TitleOfPRorRFQ = procurementApprovalOrFactsheet.TitleOfPRorRFQ;
                factsheetOrProcument.RFQReferenceNo = procurementApprovalOrFactsheet.RFQReferenceNo;
                factsheetOrProcument.PRReferenceNo = procurementApprovalOrFactsheet.PRReferenceNo;
                factsheetOrProcument.NameOfRequester = procurementApprovalOrFactsheet.NameOfRequester;
                factsheetOrProcument.DepartmentName = procurementApprovalOrFactsheet.DepartmentName;
                factsheetOrProcument.RFQProcess = procurementApprovalOrFactsheet.RFQProcess;
                factsheetOrProcument.SourcingMethod = procurementApprovalOrFactsheet.SourcingMethod;
                factsheetOrProcument.NameOfRecommendedSupplier =
                    procurementApprovalOrFactsheet.NameOfRecommendedSupplier;
                factsheetOrProcument.PurchaseValue = procurementApprovalOrFactsheet.PurchaseValue;
                factsheetOrProcument.SavingAmount = procurementApprovalOrFactsheet.SavingAmount;
                factsheetOrProcument.SavingType = procurementApprovalOrFactsheet.SavingType;
                result.ProcurementApprovalRequest = factsheetOrProcument;
            }

            #endregion


            #region Estimate Approver
            var approverList = new List<CreateEstimateApproverServiceRequest>();            
            foreach(var item in estimateApproverList)
            {
                var approver = new CreateEstimateApproverServiceRequest() {
                    ApproverFullName = item.ApproverFullName,
                    ApproverDepartment = item.ApproverDepartment,
                    ApproverRole = item.ApproverRoleName,
                    ExpectedTime = item.ApproverPlanDate.ToString(),
                    Status = item.ApproverStatus
                };
                approverList.Add(approver);
            }

            result.EstimateApproverList = approverList;

            #endregion

            #region Estimate Item Details
            if(estimateItemDetails.Count > 0)
            {
                var itemList = new List<CreateEstimateDetailsServiceRequest>();
                foreach (var item in estimateItemDetails)
                {
                    var lineItem = new CreateEstimateDetailsServiceRequest()
                    {
                        Estimation_Id = item.EstimationId,
                        Particular = item.Particular,
                        ItemCategory = item.ItemCategory,
                        ItemName = item.ItemName,
                        Item_Id = item.ItemId,
                        ItemCode = item.ItemCode,
                        ItemUnit = item.ItemUnit,
                        NoOfMachineAndUsagesAndTeamAndCar = item.NoOfMachineAndUsagesAndTeamAndCar,
                        NoOfDayAndTotalUnit = item.NoOfDayAndTotalUnit,
                        QuantityRequired = item.QuantityRequired,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice,
                        Remarks = item.Remarks,
                        AreaType = item.AreaType,
                        ResponsibleDepartmentName = item.DepartmentName,
                        ResponsibleDepartment_Id = item.DepartmentId,
                        DistrictName = item.DistrictName,
                        ThanaName = item.ThanaName,
                        Thana_Id = item.ThanaId,
                        EstimateSettleItemId = item.EstimateSettleItemId

                    };
                    itemList.Add(lineItem);
                }

                result.EstimateDetails = itemList;
            }
            
            #endregion

            #region Estimate Line Item Summary By Department
            if(estimateDeptSummaryList.Count > 0)
            {
                var deptSummaryList = new List<CreateDepartmentWiseSummaryServiceRequest>();
                foreach (var item in estimateDeptSummaryList)
                {
                    var deptSum = new CreateDepartmentWiseSummaryServiceRequest()
                    {
                        DepartmentName = item.DepartmentName,
                        Department_Id = item.DepartmentId,
                        TotalPrice = item.Price,
                        Estimate_Id = item.EstimationId
                    };

                    deptSummaryList.Add(deptSum);
                }
                result.DepartmentWiseSummary = deptSummaryList;
            }
            #endregion

            #region Estimate Line Item Summary By Particular
            if (estimatePartiSummaryList.Count > 0)
            {
                var particularSummaryList = new List<CreateParticularWiseSummaryServiceRequest>();
                foreach (var item in estimatePartiSummaryList)
                {
                    var partiSum = new CreateParticularWiseSummaryServiceRequest()
                    {
                        ParticlarName = item.ParticularName,
                        Particular_Id = item.ParticularId,
                        TotalPrice = item.Price,
                        Estimate_Id = item.EstimationId
                    };

                    particularSummaryList.Add(partiSum);
                }
                result.ParticularWiseSummary = particularSummaryList;
            }
            #endregion

            #region Estimate Approvers Feedbacks
            if(estimateApproverFeedBackList.Count > 0)
            {
                var feedbackList = new List<LoadApproverFeedBackDetails>();
                foreach(var item in estimateApproverFeedBackList)
                {
                    var sta = "";
                    if (item.EstimateStatus == 100)
                        sta = "Approved";
                    else if (item.EstimateStatus == -404)
                        sta = "Change Requested";
                    else if (item.EstimateStatus == -500)
                        sta = "Rejected";

                    var feedback = new LoadApproverFeedBackDetails() {
                        EstimateId = item.EstimateId,
                        ApproverFullName = item.ApproverFullName,
                        ApproverUserId = item.ApproverUserId,
                        ApproverUserName = item.ApproverUserName,
                        EstimateStatus = item.EstimateStatus,
                        EstimateApproverStatusString = sta,
                        FeedBack = item.FeedBack,
                        FeedBackDate = item.FeedBackDate
                    };

                    feedbackList.Add(feedback);
                }
                result.EstimateApproverFeedBacks = feedbackList;
            }            

            #endregion

            return result;
        }

        public async Task<GetDraftBudgetEstimationbyUserResponse> LoadOngoinEstimationByUserService(int userId)
        {
            try
            {
                var response = new GetDraftBudgetEstimationbyUserResponse();

                using var uow = _uowFactory.GetUnitOfWork();

                response.DraftedBudgest = await uow.EstimationRepo.LoadOngoinEstimationByUser(userId);

                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region FundRequisition 

        public async Task<List<EstimateEditVM>> LoadAllApprovedEstimateByUserDepartment(int userId, int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                List<EstimateEditVM> response = new List<EstimateEditVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    
                        response = await uow.EstimationRepo.LoadAllApprovedEstimateByUserDepartment(userId, currentPageIndex, pAGE_SIZE, sessionUser.Id, sessionUser.Department_Id);

                }
                return response;
            }
            catch (Exception e) { throw; }
        }

        public async Task<EstimationInfoForMemo> EstimationInfoService(int estiId)
        {
            try
            {
                var response = new EstimationInfoForMemo();

                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.EstimationRepo.EstimationInfo(estiId);

                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IList<DepartWiseSummaryDetailsForSettledEstimation>> LoadDeptSummaryForSettledEstimateByaEstimationService(int estimationId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.SettlementDepartmentWiseSummaryRepo.LoadDeptSummaryForSettledEstimateByaEstimation(estimationId);
                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IList<DepartWiseSummaryDetailsForSettledEstimation>> LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(int estimationId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.SettlementDepartmentWiseSummaryRepo.LoadDeptSummaryForSettledEstimateByaEstimationWithBudgetData(estimationId);
                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IList<DepartWiseSummaryDetailsForSettledEstimation>> LoadDeptSummeryForRunningSettlementBySettlementId(int settlementId , int estimationId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.SettlementDepartmentWiseSummaryRepo.LoadDeptSummaryForRunningSettlementBySettlementId(settlementId,estimationId);
                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>> LoadParticularWiseSummaryForAEstimationSettleService(int estimationId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.SettlementParticularWiseSummaryRepo.LoadParticularWiseSummaryForAEstimationSettle(estimationId);
                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>> LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(int estimationId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.SettlementParticularWiseSummaryRepo.LoadParticularWiseSummaryForAEstimationSettleWithBudgetData(estimationId);
                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        public async Task<List<EstimateVM>> LoadAllEstimateByUserExceptPending(int userId)
        {
            try
            {
                List<EstimateVM> response = new List<EstimateVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimationRepo.LoadAllEstimateByUserExceptPending(userId);
                }
                return response;
            }
            catch (Exception e) { throw; }
        }


        public async Task<List<EstimateVM>> LoadAllRejectedEstimateBySpecificUser(int userId)
        {
            try
            {
                List<EstimateVM> response = new List<EstimateVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimationRepo.LoadAllRejectedEstimateBySpecificUser(userId);
                }
                return response;
            }
            catch (Exception e) { throw; }
        }
        public async Task<List<EstimateVM>> LoadAllRunningEstimateBySpecificUser(int userId)
        {
            try
            {
                List<EstimateVM> response = new List<EstimateVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimationRepo.LoadAllRunningEstimateBySpecificUser(userId);
                }
                return response;
            }
            catch (Exception e) { throw; }
        }

        public async Task<List<EstitmationApprovalInfo>> GetAllEstimationApprovalInfoByUserId()
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                if (sessionUser == null)
                {
                    if (sessionUser == null) throw new Exception("user session expired");
                }
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.EstimationRepo.load_all_approval_data_forUserByUserID(sessionUser.Id);

                foreach (var estimation in response)
                {
                    estimation.ViewURL = "/BudgetEstimation/ViewEstimation?id=" +  _protector.Protect(estimation.EstimationId.ToString()) + "";
                }

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<DashboardTotalAmountVM>> GetAllBudgetAmountSumByUserId()
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                {
                    throw new Exception("user session expired");
                }
                List<DashboardTotalAmountVM> response = new List<DashboardTotalAmountVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimationRepo.GetAllBudgetAmountSumByUserId(sessionUser.Id);
                }
                return response;
            }
            catch (Exception e) { throw; }
        }

        public async Task<IList<ParticularWiseSummaryDetailsForSettledEstimation>> LoadParticularWiseSummaryForRunningSettlementBySettlementId(int settlementId , int estimationId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.SettlementParticularWiseSummaryRepo.LoadParticularWiseSummaryForRunningSettlementBySettlementId(settlementId,estimationId);
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

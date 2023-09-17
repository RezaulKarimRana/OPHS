using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMS.Common.Constants;
using AMS.Infrastructure.Configuration;
using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Settlement;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.SettlementRepo.Models;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Contracts;
using AMS.Services.Managers.Contracts;
using static AMS.Infrastructure.Configuration.ApplicationConstants;

namespace AMS.Services.SettlementService
{
    public class SettlementService : ISettlementService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionManager _sessionManager;
        private readonly IEmailHandlerService _emailHandlerService;


        public SettlementService(IUnitOfWorkFactory uowFactory,
            ISessionManager sessionManager,
            IEmailHandlerService emailHandlerService
            )
        {
            _uowFactory = uowFactory;
            _sessionManager = sessionManager;
            _emailHandlerService = emailHandlerService;
        }
        public async Task<SettlementInitModel> GetSettlementInitData()
        {
            try
            {
                var response = new SettlementInitModel();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var districts = await uow.DistRepo.GetAllDist();
                    var department = await uow.DepartmentRepo.GetAllDepartmentsJoinUserByConfiguration();
                    response.DistrictList = districts.Select(x => new NameIdPairModel { Id = x.Id, Name = x.Name }).ToList();
                    response.ParticularList = await uow.ParticularRepo.GetAllAsNameIdPair();
                    response.AreaTypeList = Enum.GetValues(typeof(AreaType)).Cast<AreaType>().Select(v => new NameIdPairModel
                    {
                        Id = (int)v,
                        Name = EnumUtility.GetDescriptionFromEnumValue(v)
                    }).ToList();
                    response.DepartmentList = department.Select(x => new NameIdPairModel { Id = x.Id, Name = x.Name }).ToList();
                    uow.Commit();
                }
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task<int> SaveSettlementData(SettlmentDTO requestDto, int status)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                requestDto.Settlement.CreatedBy = sessionUser.Id;
                requestDto.Settlement.Status = status;
                int settlementId = 0;
                double totalSettleAmount = 0;
                foreach (var detailsReq in requestDto.SettlementItems)
                {
                    totalSettleAmount += detailsReq.ActualTotalPrice;
                }

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    settlementId = await uow.SettlementRepo.CreateOrModifySettlement(new CreateSettlementRequest()
                    {
                        Id = requestDto.Settlement.Id,
                        EstimationId = requestDto.Settlement.EstimationId,
                        CreationDate = DateTime.Now,
                        CreatedBy = sessionUser.Id,
                        SettlementNote = requestDto.Settlement.SettlementNote,
                        Status = requestDto.Settlement.Status,
                        TotalAmount = totalSettleAmount,
                        IsItFinalSetttlement = requestDto.Settlement.IsItFinalSetttlement
                    });


                    requestDto.Settlement.Id = settlementId;

                    try
                    {
                        foreach (var detailsReq in requestDto.SettlementItems)
                        {
                            detailsReq.EstimationId = requestDto.Settlement.EstimationId;
                            detailsReq.SettlementId = requestDto.Settlement.Id;

                            int estimateSettleItemId = 0;
                            int SettleItemId = 0;

                            if (detailsReq.EstimateSettleItemId > 0)
                            {
                                estimateSettleItemId = detailsReq.EstimateSettleItemId;
                            }
                            else
                            {
                                totalSettleAmount +=
                                    estimateSettleItemId =
                                        await uow.SettlementItemRepo.CreateOrModifyEstimateSettleItem(
                                            new CreateSettlementItemRequest()
                                            {
                                                EstimationId = detailsReq.EstimationId,
                                                ItemId = detailsReq.ItemId,

                                                NoOfMachineAndUsagesAndTeamAndCar =
                                                    detailsReq.NoOfMachineAndUsagesAndTeamAndCar,
                                                NoOfDayAndTotalUnit = detailsReq.NoOfDayAndTotalUnit,
                                                QuantityRequired = detailsReq.QuantityRequired,
                                                UnitPrice = detailsReq.UnitPrice,
                                                TotalPrice = detailsReq.TotalPrice,
                                                Remarks = detailsReq.Remarks,
                                                AreaType = detailsReq.AreaType,
                                                ResponsibleDepartment_Id = detailsReq.ResponsibleDepartment_Id,
                                                ThanaId = detailsReq.ThanaId,
                                                CreatedBy = requestDto.Settlement.CreatedBy
                                            }
                                        );
                            }

                            try
                            {
                                SettleItemId = await uow.SettlementItemRepo.CreateOrModifySettleItem(
                                    new CreateSettlementItemRequest()
                                    {
                                        SettleItemId = detailsReq.SettleItemId,
                                        SettlementId = detailsReq.SettlementId,
                                        ActualTotalPrice = detailsReq.ActualTotalPrice,
                                        ActualUnitPrice = detailsReq.ActualUnitPrice,
                                        ActualQuantity = detailsReq.ActualQuantity,
                                        SettleItemRemarks = detailsReq.SettleItemRemarks,
                                        EstimationId = detailsReq.EstimationId,
                                        EstimateSettleItemId = estimateSettleItemId,
                                        CreatedBy = sessionUser.Id
                                    });
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                throw;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    try
                    {
                        foreach (var approverReq in requestDto.SettlementApproverList)
                        {
                            approverReq.Settlement_Id = requestDto.Settlement.Id;

                            int approverId = 0;
                            if (approverReq.RolePriority_Id == 3)
                                approverReq.Status = BaseEntity.EntityStatus.Completed.ToString();
                            else
                                approverReq.Status = EstimateApproverEntity.EntityStatus.Pending.ToString();

                            approverId = await uow.SettlementRepo.CreateSettlementApprover(
                                new CreateSettlementApproverRequest()
                                {
                                    Settlement_Id = approverReq.Settlement_Id,
                                    User_Id = approverReq.User_Id,
                                    Priority = approverReq.Priority,
                                    Status = approverReq.Status,
                                    Remarks = approverReq.Remarks,
                                    RolePriority_Id = approverReq.RolePriority_Id,
                                    Created_By = requestDto.Settlement.CreatedBy,
                                    PlanDate = approverReq.ExpectedTime
                                });
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    uow.Commit();
                }

                if (settlementId > 0 && status != 5)
                {

                    await _emailHandlerService.SaveSettlementEmail(settlementId, sessionUser);

                }
                return settlementId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task<int> UpdateSettlementStatusAndRelatedData(SettlementFeedback settlementFeedback)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.SettlementRepo.UpdateSettlementFeedback(settlementFeedback);
                uow.Commit();

                if (response > 0)
                {
                    if (settlementFeedback.CurrentUserRolePiority != 1 && settlementFeedback.Feedback == 100)
                    {
                        //await _emailHandlerService.SaveNewSettlementOnGogingCompleteEmail(settlementFeedback.SettlementId, sessionUser, 1);
                        await _emailHandlerService.SaveSettlementEmail(settlementFeedback.SettlementId, sessionUser);
                    }
                    else if (settlementFeedback.CurrentUserRolePiority == 1 && settlementFeedback.Feedback == 100)
                    {
                        //await _emailHandlerService.SaveNewSettlementOnGogingCompleteEmail(settlementFeedback.SettlementId, sessionUser, 2);
                        await _emailHandlerService.SaveSettlementEmail(settlementFeedback.SettlementId, sessionUser);
                    }

                    if (settlementFeedback.Feedback == -404)
                    {
                        //await _emailHandlerService.SaveNewSettlementOnRejectRollbackEmail(settlementFeedback.SettlementId, sessionUser, 1);
                        await _emailHandlerService.SaveSettlementEmail(settlementFeedback.SettlementId, sessionUser);
                    }

                    if (settlementFeedback.Feedback == -500)
                    {
                        //await _emailHandlerService.SaveNewSettlementOnRejectRollbackEmail(settlementFeedback.SettlementId, sessionUser, 0);
                        await _emailHandlerService.SaveSettlementEmail(settlementFeedback.SettlementId, sessionUser);
                    }


                }
                return 1;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<SettlementApproverDetails>> GetAllApproverBySettlementIdForRollbackDraft(
            int settlementId)
        {
            try
            {
                var response = new List<SettlementApproverDetails>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.SettlementRepo.GetAllApproverBySettlementIdForRollbackDraft(settlementId);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CreateSettlementRequest> getDraftSettlement(int estimateId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");


                CreateSettlementRequest settlement = null;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    settlement = await uow.SettlementRepo.getDraftSettlmentId(estimateId, sessionUser.Id);

                    uow.Commit();
                }

                return settlement;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<CreateSettlementRequest> getSettlementBySettlementId(int settlementId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");



                CreateSettlementRequest settlement = null;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    settlement = await uow.SettlementRepo.getSettlementBySettlementId(settlementId, sessionUser.Id);

                    uow.Commit();
                }

                return settlement;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<SettlementVM>> LoadAllSettlementForFollower(int userId)
        {
            try
            {
                List<SettlementVM> response = new List<SettlementVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.SettlementRepo.LoadAllSettlementForFollower(userId);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<SettlementApproverDetails>> LoadSettlementApproverDetailsBySettlementId(int settlementId)
        {
            try
            {
                var response = new List<SettlementApproverDetails>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.SettlementRepo.LoadSettlementApproverDetailsBySettlementId(settlementId);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<SettlementApproverFeedbackDetails>> LoadSettlementApproverRemarks(int settlementId)
        {
            try
            {
                var sessionUser = _sessionManager.GetUser();
                if (sessionUser == null)
                    throw new Exception("user session expired");

                using var uow = _uowFactory.GetUnitOfWork();
                {
                    var response =
                        await uow.SettlementRepo.LoadSettlementApproverFeedBackDetailsBySettlementId(settlementId);


                    return response;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<SettlementVM>> LoadSettlementByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                List<SettlementVM> response = new List<SettlementVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.SettlementRepo.LoadSettlementByStatus(userId, status, currentPageIndex, pAGE_SIZE);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<SettlementSummaryVM> LoadSettlementSummary()
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");
                var response = new SettlementSummaryVM();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.SettlementRepo.LoadSettlementSummary(sessionUser.Id);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<SettlementVM>> getInCompletedSettlementForCheckFinalSettlement(int estimationId)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                var response = new List<SettlementVM>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.SettlementRepo.getInCompletedSettlementForCheckFinalSettlement(sessionUser.Id,
                    sessionUser.Department_Id, estimationId);
                uow.Commit();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> SaveEstimateSettlementAttachment(CreateAttachmentForSettlementRequest attachReq)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                {
                    throw new Exception("Invalid User");
                }
                int id;

                using var uow = _uowFactory.GetUnitOfWork();

                id = await uow.SettlementRepo.CreateAttachmentForSettlement(new CreateAttachmentForSettlementRequest()
                {
                    URL = attachReq.URL,
                    FileName = attachReq.FileName,
                    EstimationSettlement_Id = attachReq.EstimationSettlement_Id,
                    Created_By = sessionUser.Id
                });

                uow.Commit();

                if (id < 1)
                    throw new Exception("Invalid ID");
                return id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<EstimateSettlementAttachmentsEntity>> LoadSettlementAttachmentsBySettlementId(int settlementId)
        {
            try
            {
                var result = new List<EstimateSettlementAttachmentsEntity>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.SettlementRepo.LoadSettlementAttachmentsBySettlementId(settlementId);
                }
                foreach (var _item in result)
                {
                    var index = _item.URL.IndexOf("\\uploadedFiles\\SettlementFiles");
                    _item.URL = _item.URL[index..];
                }
                return result.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<ReadyForSettlementVM>> ReadyToSettlementList(int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();

                List<ReadyForSettlementVM> response = new List<ReadyForSettlementVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {

                    response = await uow.SettlementRepo.ReadyToSettlementList(sessionUser.Id, currentPageIndex, pAGE_SIZE, sessionUser.Department_Id);

                }
                return response;
            }
            catch (Exception e) { throw; }
        }
        public async Task<bool> DeleteAttachmentsById(int id)
        {
            try
            {
                bool response = false;

                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.SettlementRepo.DeleteAttachmentsById(id);
                uow.Commit();

                return response;
            }
            catch (Exception e) { throw; }
        }
    }
}

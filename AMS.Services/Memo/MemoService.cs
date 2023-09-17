using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.BudgetEstimate;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.EstimateMemo.Models;
using AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo.Models;
using AMS.Repositories.DatabaseRepos.MemoApproverRepo.Models;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Managers.Contracts;
using AMS.Services.Memo.Contracts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMS.Models.ServiceModels.BudgetEstimate.BudgetApprover;
using AMS.Services.Contracts;
using AMS.Models.ServiceModels.Memo;

namespace AMS.Services.Memo
{
    public class MemoService : IMemoService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionManager _sessionManager;
        private readonly IEmailHandlerService _emailHandlerService;
        private readonly IConfiguration _configuration;
        IDataProtector _protector;

        public MemoService(IUnitOfWorkFactory uowFactory, ISessionManager sessionManager,
            IEmailHandlerService emailHandlerService,
            IDataProtectionProvider provider,
            IConfiguration configuration)
        {
            _uowFactory = uowFactory;
            _sessionManager = sessionManager;
            _emailHandlerService = emailHandlerService;
            _configuration = configuration;
            _protector = provider.CreateProtector(_configuration.GetSection("URLParameterEncryptionKey").Value);
        }

        public async Task<int> SaveEstimationMemoService(AddBudgetEstimationMemoDTO request)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");

                int estimateMemoId = 0;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    estimateMemoId = await uow.EstimateMemoEntityRepo.CreateEstimationMemo(request, sessionUser.Id);
                    uow.Commit();
                }

                if (estimateMemoId > 0)
                {
                   await _emailHandlerService.SaveMemoEmail(estimateMemoId, sessionUser);
                }
                return estimateMemoId;
               
            }
            catch (Exception e)
            {

                throw;
            }
  
        }

        public async Task<EstimationReferenceEntity> GetEstimationReferenceByEstimateService(int estimateId)
        {
            try
            {
                var result = new EstimationReferenceEntity();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.EstimationReferenceRepo.GetEstimationReferenceByEstimate(estimateId);
                }
                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<EstimationReferenceEntity> GetEstimationReferenceByIdService(int id)
        {
            try
            {
                var result = new EstimationReferenceEntity();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.EstimationReferenceRepo.GetEstimationReferenceById(id);
                }
                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<List<PendingMemo>> LoadAllPendingMemoService()
        {
            try
            {
                IList<PendingMemo> result = new List<PendingMemo>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.EstimateMemoEntityRepo.LoadAllPendingMemo();
                }
                return result.ToList();
            }
            catch (Exception e) 
            { 
                throw; 
            }
        }

        public async Task<List<EstimationSettleItemDetailDTO>> LoadEstimationSettleItemDetailsByEstimationIdService(int estimationId)
        {
            try
            {
                var result = new List<EstimationSettleItemDetailDTO>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.EstimateMemoEntityRepo.LoadEstimationSettleItemDetailsByEstimationId(estimationId);
                }
                return result.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<EstimateSettleItemDetails>> LoadSettleItemDetailsBySettleItemAndEstimationIdService(int estimationId, int estimateSettleItem)
        {
            try
            {
                var result = new List<EstimateSettleItemDetails>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.EstimateMemoEntityRepo.LoadSettleItemDetailsBySettleItemAndEstimationId(estimationId,estimateSettleItem);
                }
                return result.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<int> SaveEstimateMemoAttachment(CreateAttachmentForMemoRequest attachReq)
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

                id = await uow.EstimateMemoAttachmentsRepo.CreateAttachmentForMemo(new CreateAttachmentForMemoRequest()
                {
                    URL = attachReq.URL,
                    FileName = attachReq.FileName,
                    EstimationMemo_Id = attachReq.EstimationMemo_Id,
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

        public async Task<IList<PendingMemo>> LoadAllPendingMemoApprovalForAUserService(int user_Id)
        {
            try
            {
                IList<PendingMemo> result = new List<PendingMemo>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.MemoApproverRepo.LoadAllPendingMemoApprovalForAUser(user_Id);
                }
                return result.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> CheckIsValidToShowInMemoParkingService(int estimateMemoId, int priority)
        {
            try
            {
                int response;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.MemoApproverRepo.CheckIsValidToShowInMemoParking(estimateMemoId, priority);
                }
                if (response == 0) 
                    return true;

                return false;
            }
            catch (Exception e) { throw; }
        }

        public async Task<EstimateMemoEntity> GetEstimateMemoEntityByIdService(int id)
        {
            try
            {
                var result = new EstimateMemoEntity();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.EstimateMemoEntityRepo.GetEstimateMemoEntityById(id);
                }
                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<EstimationInfoForMemo> EstimationMemoInfoDetailsByIdService(int id)
        {
            try
            {
                var response = new EstimationInfoForMemo();

                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.EstimateMemoEntityRepo.EstimationMemoInfoDetailsById(id);

                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<EstimateMemoAttachmentsEntity>> LoadMemoAttachmentsByMemoService(int estimateMemoId)
        {
            try
            {
                var result = new List<EstimateMemoAttachmentsEntity>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.EstimateMemoAttachmentsRepo.LoadMemoAttachmentsByEstimateMemo(estimateMemoId);
                }
                foreach(var _item in result)
                {
                    var index = _item.URL.IndexOf("\\uploadedFiles\\MemoFiles");
                    _item.URL = _item.URL[index..];
                }
                return result.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<MemoApproverDetailsDTO>> LoadMemoApproverDetailsByMemoIdService(int memoId)
        {
            try
            {
                var result = new List<MemoApproverDetailsDTO>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.MemoApproverRepo.LoadMemoApproverDetailsByMemoId(memoId);
                }
                return result.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<MemoApproverEntity>> LoadLatestPendingMemoApproverByMemoIdService(int memoId)
        {
            try
            {
                var result = new List<MemoApproverEntity>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.MemoApproverRepo.LoadLatestPendingMemoApproverByMemoId(memoId);
                }
                return result.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<MemoApproverEntity> GetLatestPendingApproverByMemoIdAndUserIdService(int memoId, 
            int userId)
        {
            try
            {
                var approver = new MemoApproverEntity();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    approver = await uow.MemoApproverRepo.GetLatestPendingApproverByMemoIdAndUserId(memoId, userId);
                }
                return approver;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<IList<MemoApproverEntity>> GetLatestPendingApproveresOfAMemoService(int memoId)
        {
            try
            {
                var approver = new List<MemoApproverEntity>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    approver = await uow.MemoApproverRepo.GetLatestPendingApproveresOfAMemo(memoId);
                }
                return approver;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public async Task<bool> SaveApproverApprovalFeedBack(int memoId, string approverFeedBackStatus, string approverFeedBackRemarks, bool isFinalApproved)
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null)
                {
                    throw new Exception("Invalid User");
                }

                var result = false;
                using var uow = _uowFactory.GetUnitOfWork();

                await uow.MemoApproverRepo.UpdateMemoApproverOnApproval(new UpdateMemoApprover()
                {
                    Id = memoId, Status = approverFeedBackStatus, Remarks = approverFeedBackRemarks, Updated_By = sessionUser.Id, IsFinalApproved = isFinalApproved 
                });
                if(uow.Commit())
                {
                    result = true;
                }

                await _emailHandlerService.SaveMemoEmail(memoId, sessionUser);

                return result;
            }
            catch (Exception e )
            {

                throw e;
            }
        }

        public async Task<IList<LoadMemoApproverFeedBackDetails>> LoadMemoApproverFeedBackDetailsService(int memoId)
        {
            try
            {
                var result = new List<LoadMemoApproverFeedBackDetails>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.MemoApproverRepo.LoadMemoApproverFeedBackDetails(memoId);
                }
                return result.ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<ApproverDetailsDTO>> LoadApproverDetailsByMemoService(int memoId)
        {
            try
            {
                var result = new List<ApproverDetailsDTO>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    result = await uow.MemoApproverRepo.LoadApproverDetailsByMemo(memoId);
                }
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IList<EstimateMemoDetails>> LoadAllRunningMemoApprovalForAUserService(int user_Id, int pageNumber, 
            int pageSize, string whereClause)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var result = await uow.MemoApproverRepo.LoadAllRunningMemoApprovalForAUser(user_Id, 
                    pageNumber, pageSize, whereClause);
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<MemoSummaryVM> LoadMemoSummary()
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");
                var response = new MemoSummaryVM();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimateMemoEntityRepo.LoadMemoSummary(sessionUser.Id);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<MemoSummaryVM> LoadApproverMemoSummary()
        {
            try
            {
                var sessionUser = await _sessionManager.GetUser();
                if (sessionUser == null) throw new Exception("user session expired");
                var response = new MemoSummaryVM();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimateMemoEntityRepo.LoadApproverMemoSummary(sessionUser.Id);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<MemoVM>> LoadMemoByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                List<MemoVM> response = new List<MemoVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimateMemoEntityRepo.LoadMemoByStatus(userId, status, currentPageIndex, pAGE_SIZE);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<MemoVM>> LoadApproverMemoByStatus(int userId, int status, int currentPageIndex, int pAGE_SIZE)
        {
            try
            {
                List<MemoVM> response = new List<MemoVM>();
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.EstimateMemoEntityRepo.LoadApproverMemoByStatus(userId, status, currentPageIndex, pAGE_SIZE);
                }

                return response;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<bool> DeleteAttachmentsById(int id)
        {
            try
            {
                bool response = false;

                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.EstimateMemoAttachmentsRepo.DeleteAttachmentsById(id);
                uow.Commit();

                return response;
            }
            catch (Exception e) { throw; }
        }
    }
}

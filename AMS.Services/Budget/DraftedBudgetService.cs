using AMS.Models.CustomModels;
using AMS.Models.ServiceModels.BudgetEstimate.DraftedBudget;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Budget.Contracts;
using AMS.Services.Managers.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Services.Budget
{
    public class DraftedBudgetService : IDraftedBudgetService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ISessionManager _sessionManager;

        public DraftedBudgetService(IUnitOfWorkFactory uowFactory, ISessionManager sessionManager)
        {
            _uowFactory = uowFactory;
            _sessionManager = sessionManager;
        }

        //public async Task<GetDraftBudgetEstimationbyUserResponse> GetDraftBudgetEstimation()
        //{
        //    try
        //    {
        //        var response = new GetDraftBudgetEstimationbyUserResponse();
        //        var sessionUser = await _sessionManager.GetUser();

        //        using var uow = _uowFactory.GetUnitOfWork();

        //        response.DraftedBudgest = await uow.EstimationRepo.GetAllDraftedBudgetEstimationByUser(sessionUser.Id);

        //        uow.Commit();

        //        return response;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public async Task<GetDraftBudgetEstimationResponse> GetSingleDraft(int estimateId)
        {
            try
            {
                var response = new GetDraftBudgetEstimationResponse();

                using var uow = _uowFactory.GetUnitOfWork();

                response.Estimation = await uow.EstimationRepo.GetSingleEstimation(estimateId);

                uow.Commit();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EstimateApproverByEstimateId>> LoadEstimateApproverDetailsByEstimation(int estiId)
        {
            try
            {
                var response = new List<EstimateApproverByEstimateId>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.EstimateApproverRepo.LoadEstimateApproverDetailsByEstimationId(estiId);
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<EstimationDetailsWithJoiningOtherTables>> LoadWholeEstimationDetailsByEstimation(int estiId)
        {
            try
            {
                var response = new List<EstimationDetailsWithJoiningOtherTables>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.EstimateDetailsRepo.LoadEstimationDetailsWithOtherInformationsByEstimationId(estiId);
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

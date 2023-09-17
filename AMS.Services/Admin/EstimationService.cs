using AMS.Models.ServiceModels.Admin.Estimation;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using System;
using System.Threading.Tasks;

namespace AMS.Services.Admin
{
    public class EstimationService : IEstimationService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public EstimationService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public async Task<EstimationNameListGetResponse> GetAllEstimationNameListService()
        {
            var response = new EstimationNameListGetResponse();
            try
            {               
                using var uow = _uowFactory.GetUnitOfWork();
                response.EstimationNames = await uow.EstimationRepo.GetAllEstimationSubjectNameList();

                uow.Commit();

                return response;
            }

            catch(Exception e)
            {
                throw;
            }
        }
    }
}

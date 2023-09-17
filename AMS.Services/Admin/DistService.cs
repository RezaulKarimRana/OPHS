using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Dist;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using System;
using System.Threading.Tasks;

namespace AMS.Services.Admin
{
    public class DistService : IDistService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public DistService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }
        public async Task<GetAllDistrictResponse> GetAllDistrict()
        {
            var response = new GetAllDistrictResponse();

            using var uow = _uowFactory.GetUnitOfWork();
            response.Districts = await uow.DistRepo.GetAllDist();
            uow.Commit();

            return response;
        }

        public async Task<DistEntity> GetById(int id)
        {
            try
            {
                DistEntity response = null;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.DistRepo.GetSingleDist(id);
                }
                return response;
            }
            catch (Exception) { throw; }
        }
    }
}

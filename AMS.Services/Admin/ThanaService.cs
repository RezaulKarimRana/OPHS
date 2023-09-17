using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Thana;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using System;
using System.Threading.Tasks;

namespace AMS.Services.Admin
{
    public class ThanaService : IThanaService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ThanaService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public async Task<GetThanaByDistIDResponse> getThanaByDistIDResponse(int distId)
        {
            var response = new GetThanaByDistIDResponse();

            using var uow = _uowFactory.GetUnitOfWork();
            response.ThanaS = await uow.ThanaRepo.GetThanaByDistId(distId);
            uow.Commit();

            return response;
        }

        public async Task<ThanaEntity> GetById(int id)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.ThanaRepo.GetSingleThana(id);
                return response;
            }
            catch (Exception e) { throw; }
        }
    }
}

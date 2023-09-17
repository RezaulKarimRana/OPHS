using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Particular;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using System.Threading.Tasks;

namespace AMS.Services.Admin
{
    public class ParticularService : IParticularService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ParticularService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public async Task<ParticularEntity> GetById(int id)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var response = await uow.ParticularRepo.GetSingleParticular(id);
            return response;
        }

        public async Task<GetParticularsResponse> GetParticulars()
        {
            var response = new GetParticularsResponse();

            using var uow = _uowFactory.GetUnitOfWork();
            response.Particulars = await uow.ParticularRepo.GetAllParticular();
            uow.Commit();

            return response;
        }
    }
}

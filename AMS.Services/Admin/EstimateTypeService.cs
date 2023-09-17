using AMS.Models.ServiceModels.Admin.EstimateType;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using System;
using System.Threading.Tasks;
using AMS.Models.ServiceModels.Admin.Dropdown;
using System.Collections.Generic;

namespace AMS.Services.Admin
{
    public class EstimateTypeService : IEstimateTypeService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public EstimateTypeService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public async Task<LoadAllEstimateType> LoadAllTheEstimateTypes()
        {
            try
            {
                var response = new LoadAllEstimateType();
                using var uow = _uowFactory.GetUnitOfWork();

                response.EstimateTypes = await uow.EstimateTyeRepo.GetAllEstimateType();

                uow.Commit();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<EstimationDropdown>> LoadAllDropdownItemByDropdownNameAndEstimationType(string dropDownName, int estimationType)
        {
            try
            {
                
                using var uow = _uowFactory.GetUnitOfWork();

                  var   response = await uow.EstimateTyeRepo.LoadAllDropdownItemByDropdownNameAndEstimationType(dropDownName, estimationType);

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

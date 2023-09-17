using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.ItemCategory;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using System;
using System.Threading.Tasks;

namespace AMS.Services.Admin
{
    public class ItemCategoryService : IItemCategoryService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ItemCategoryService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public async Task<ItemCategoryEntity> GetById(int id)
        {
            try
            {
                ItemCategoryEntity response = null;
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response = await uow.ItemCategoryRepo.GetSingleItemCategory(id);
                }
                return response;
            }
            catch (Exception) { throw; }
        }

        public async Task<GetItemCategoriesByParticularResponse> GetItemes()
        {
            var response = new GetItemCategoriesByParticularResponse();
            using var uow = _uowFactory.GetUnitOfWork();
            response.ItemCategories = await uow.ItemCategoryRepo.GetItemCategories();
            uow.Commit();
            return response;
        }

        public async Task<GetItemCategoriesByParticularResponse> GetItemesByParticularName(int particularId)
        {
            var response = new GetItemCategoriesByParticularResponse();

            using var uow = _uowFactory.GetUnitOfWork();
            response.ItemCategories = await uow.ItemCategoryRepo.GetItemCategoriesByParticular(particularId);
            uow.Commit();

            return response;
        }
    }
}

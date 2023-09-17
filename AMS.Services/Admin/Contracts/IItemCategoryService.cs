using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.ItemCategory;
using System.Threading.Tasks;

namespace AMS.Services.Admin.Contracts
{
    public interface IItemCategoryService
    {
        Task<GetItemCategoriesByParticularResponse> GetItemesByParticularName(int particularId);
        Task<GetItemCategoriesByParticularResponse> GetItemes();
        Task<ItemCategoryEntity> GetById(int id);
    }
}

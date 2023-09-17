using AMS.Models.BudgetDTO;
using AMS.Models.DomainModels;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.ItemCategoryRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ItemCategoryRepo.Contracts
{
    public interface IItemCategoryRepo
    {
        Task<int> CreateItemCategory(CreateItemCategoryRequest request);
        Task<List<ItemCategoryEntity>> GetAllItemCategory();
        Task<ItemCategoryEntity> GetSingleItemCategory(int id);
        Task UpdateItemCategory(UpdateItemCategoryRequest request);
        Task DeleteItemCategory(DeleteItemCategoryRequest request);
        Task<List<ItemCategoryEntity>> GetItemCategoriesByParticular(int particularId);
        Task<List<ItemCategoryEntity>> GetItemCategories();
        Task<List<NameIdPairModel>> GetAllAsNameIdPair();
    }
}

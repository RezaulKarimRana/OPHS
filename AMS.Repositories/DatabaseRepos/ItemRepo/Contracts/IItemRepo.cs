using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.ItemRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ItemRepo.Contracts
{
    public interface IItemRepo
    {
        Task<int> CreateItem(CreateItemRequest request);
        Task<int> CreateAMSItem(CreateAMSItemRequest request);
        Task<int> SaveParticular(NameModel model);
        Task<int> SaveItemCategory(ItemCategoryModel model);
        Task<List<ItemEntity>> GetAllItems();
        Task<ItemEntity> GetSingleItem(int id);
        Task UpdateItem(UpdateItemRequest request);
        Task DeleteItem(DeleteItemRequest request);
        Task<List<GetItemUnitDetails>> GetItemUnitDetails(int ItemCategoryId);
        Task<GetItemDetails> GetItemDetailsByItemCode(string itemCode);
        Task<List<GetItemDetails>> GetItemDetailsByItemCodes(string itemCodes); 
    }
}

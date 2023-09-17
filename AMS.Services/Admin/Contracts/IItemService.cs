using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Item;
using AMS.Repositories.DatabaseRepos.ItemRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Services.Admin.Contracts
{
    public interface IItemService
    {
        Task<GetAllItems> GetItems();
        Task<GetItemUnitDetailsResponse> GetItemUnitDetails(int itemCategoryId);
        Task<ItemEntity> GetById(int id);
        Task<GetItemDetails> GetItemDetailsByItemCodeService(string itemCode);
        Task<List<GetItemDetails>> GetItemDetailsByItemCodesService(string itemCode); 
    }
}

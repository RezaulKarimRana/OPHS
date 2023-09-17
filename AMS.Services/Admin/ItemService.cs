using AMS.Models.CustomModels;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Item;
using AMS.Repositories.DatabaseRepos.ItemRepo.Models;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMS.Services.Admin
{
    public class ItemService : IItemService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public ItemService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public async Task<ItemEntity> GetById(int id)
        {
            try
            {

                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.ItemRepo.GetSingleItem(id);
                return response;
            }
            catch (Exception) { throw; }
        }

        public async Task<GetItemDetails> GetItemDetailsByItemCodeService(string itemCode)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.ItemRepo.GetItemDetailsByItemCode(itemCode);

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<GetItemDetails>> GetItemDetailsByItemCodesService(string itemCode)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();
                var response = await uow.ItemRepo.GetItemDetailsByItemCodes(itemCode); 
                var items = itemCode.Split(",").AsList().Select(itemId => response.FirstOrDefault(x=> x.ItemCode == itemId)).ToList();

                return items;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GetAllItems> GetItems()
        {
            try
            {
                var response = new GetAllItems();
                using var uow = _uowFactory.GetUnitOfWork();
                response.Items = await uow.ItemRepo.GetAllItems();
                return response;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<GetItemUnitDetailsResponse> GetItemUnitDetails(int itemCategoryId)
        {
            try
            {
                var response = new GetItemUnitDetailsResponse();

                using var uow = _uowFactory.GetUnitOfWork();
                response.ItemUnitModel = await uow.ItemRepo.GetItemUnitDetails(itemCategoryId);

                return response;
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}

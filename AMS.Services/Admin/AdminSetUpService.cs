using AMS.Infrastructure.Configuration;
using AMS.Models.ViewModel;
using AMS.Repositories.DatabaseRepos.ItemRepo.Models;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Admin.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AMS.Infrastructure.Configuration.ApplicationConstants;

namespace AMS.Services.Admin
{
    public class AdminSetUpService : IAdminSetUpService
    {
        private readonly IUnitOfWorkFactory _uowFactory;

        public AdminSetUpService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }
        public async Task<ItemInitModel> GetItemInitData()
        {
            var response = new ItemInitModel();

            using var uow = _uowFactory.GetUnitOfWork();
            response.ParitcularList = await uow.ParticularRepo.GetAllAsNameIdPair();
            response.ItemCategoryList = await uow.ItemCategoryRepo.GetAllAsNameIdPair();
            response.UnitList = await uow.UnitRepo.GetAllAsNameIdPair();
            response.ModuleList = Enum.GetValues(typeof(ApplicationModule)).Cast<ApplicationModule>().Select(v => new NameIdPairModel
            {
                Id = (int)v,
                Name = EnumUtility.GetDescriptionFromEnumValue(v)

            }).ToList();
            response.StatusList = Enum.GetValues(typeof(ApplicationStatus)).Cast<ApplicationStatus>().Select(v => new NameIdPairModel
            {
                Id = (int)v,
                Name = EnumUtility.GetDescriptionFromEnumValue(v)

            }).ToList();
            response.RoleList = Enum.GetValues(typeof(ApproverRole)).Cast<ApproverRole>().Select(v => new NameIdPairModel
            {
                Id = (int)v,
                Name = EnumUtility.GetDescriptionFromEnumValue(v)

            }).ToList();
            response.Users = (List<Models.CustomModels.GetUsersWithDepartmentName>)await uow.UserRepo.GetUsersWithDepartmentName();
            response.DepartmentList = await uow.DepartmentRepo.GetAllDepartments();
            uow.Commit();

            return response;
        }

        public async Task<ResponseViewModel<int>> SaveItem(ItemSaveModel model)
        {
            var response = new CreateItemRequest
            {
                Name = model.ItemName,
                Unit_Id = model.UnitId,
                ItemCode = model.ItemCode,
                ItemCategory_Id = model.ItemCategoryId,
                IndicatingUnitPrice = model.IndicatingUnitPrice,
                Particular_Id = model.ParticularId,
                SystemName = model.ItemName,
                Created_By = model.CreatedById
            };
            using var uow = _uowFactory.GetUnitOfWork();
            int P_Key = await uow.ItemRepo.CreateItem(response);
            uow.Commit();
            return new ResponseViewModel<int> { Data = P_Key, Success = true };
        }
        public async Task<ResponseViewModel<int>> AMSSaveItem(ItemSaveModel model)
        {
            var response = new CreateAMSItemRequest
            {
                Name = model.ItemName,
                Unit_Id = model.UnitId,
                ItemCategory_Id = model.ItemCategoryId,
                IndicatingUnitPrice = model.IndicatingUnitPrice,
                Particular_Id = model.ParticularId,
                SystemName = model.ItemName,
                Created_By = model.CreatedById
            };
            using var uow = _uowFactory.GetUnitOfWork();
            int P_Key = await uow.ItemRepo.CreateAMSItem(response);
            uow.Commit();
            return new ResponseViewModel<int> { Data = P_Key, Success = true };
        }
        public async Task<ResponseViewModel<int>> SaveParticular(NameModel model)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            int P_Key = await uow.ItemRepo.SaveParticular(model);
            uow.Commit();
            return new ResponseViewModel<int> { Data = P_Key, Success = true };
        }
        public async Task<ResponseViewModel<int>> SaveItemCategory(ItemCategoryModel model)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            int P_Key = await uow.ItemRepo.SaveItemCategory(model);
            uow.Commit();
            return new ResponseViewModel<int> { Data = P_Key, Success = true };
        }
        public async Task<List<NameIdPairModel>> GetItemCategoryByParticularId(int particularId)
        {
            try
            {
                using var uow = _uowFactory.GetUnitOfWork();

                var response = await uow.ItemCategoryRepo.GetItemCategoriesByParticular(particularId);

                uow.Commit();

                return response.Select(x=> new NameIdPairModel { Id = x.Id, Name = x.Name}).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<ApproverModificationVM>> GetAllApprover(int moduleId, string requestNo)
        {
            try
            {
                var response = new List<ApproverModificationVM>();
                using var uow = _uowFactory.GetUnitOfWork();
                response = await uow.AdminSupportRepo.GetAllApprover(moduleId,requestNo);
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<ResponseViewModel<int>> UpdateApproverModification(ApproverModificationUpdateModel model)
        {
            var topPriotiry = model.Approvers.Count();
            foreach(var approver in model.Approvers)
            {
                approver.PriorityId = topPriotiry--;
                approver.StatusId = approver.IsApproved ? 100 : approver.StatusId;
            }
            using var uow = _uowFactory.GetUnitOfWork();
            int P_Key = await uow.AdminSupportRepo.UpdateApproverModification(model);
            uow.Commit();
            return new ResponseViewModel<int> { Data = P_Key, Success = true };
        }
        public async Task<ResponseViewModel<int>> UpdateRequestStatus(StatusUpdateModel model)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            int P_Key = await uow.AdminSupportRepo.UpdateRequestStatus(model);
            uow.Commit();
            return new ResponseViewModel<int> { Data = P_Key, Success = true };
        }
        public async Task<ResponseViewModel<int>> UpdateApproverRole(ApproverRoleUpdateModel model)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            int P_Key = await uow.AdminSupportRepo.UpdateApproverRole(model);
            uow.Commit();
            return new ResponseViewModel<int> { Data = P_Key, Success = true };
        }
        public async Task<ResponseViewModel<int>> UpdateUserDepartment(UserDepartmentUpdateModel model)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            int P_Key = await uow.AdminSupportRepo.UpdateUserDepartment(model);
            uow.Commit();
            return new ResponseViewModel<int> { Data = P_Key, Success = true };
        }
    }
}

using AMS.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Services.Admin.Contracts
{
    public interface IAdminSetUpService
    {
        Task<ItemInitModel> GetItemInitData();
        Task<ResponseViewModel<int>> SaveItem(ItemSaveModel model);
        Task<ResponseViewModel<int>> AMSSaveItem(ItemSaveModel model);
        Task<ResponseViewModel<int>> SaveParticular(NameModel model);
        Task<ResponseViewModel<int>> SaveItemCategory(ItemCategoryModel model);
        Task<List<NameIdPairModel>> GetItemCategoryByParticularId(int particularId);
        Task<List<ApproverModificationVM>> GetAllApprover(int moduleId, string requestNo);
        Task<ResponseViewModel<int>> UpdateApproverModification(ApproverModificationUpdateModel model);
        Task<ResponseViewModel<int>> UpdateRequestStatus(StatusUpdateModel model);
        Task<ResponseViewModel<int>> UpdateApproverRole(ApproverRoleUpdateModel model);
        Task<ResponseViewModel<int>> UpdateUserDepartment(UserDepartmentUpdateModel model);
    }
}

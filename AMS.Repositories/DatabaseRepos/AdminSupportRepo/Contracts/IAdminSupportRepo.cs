using AMS.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.AdminSupportRepo.Contracts
{
    public interface IAdminSupportRepo
    {
        Task<List<ApproverModificationVM>> GetAllApprover(int moduleId, string requestNo);
        Task<int> UpdateApproverModification(ApproverModificationUpdateModel model);
        Task<int> UpdateRequestStatus(StatusUpdateModel model);
        Task<int> UpdateApproverRole(ApproverRoleUpdateModel model);
        Task<int> UpdateUserDepartment(UserDepartmentUpdateModel model);
    }
}

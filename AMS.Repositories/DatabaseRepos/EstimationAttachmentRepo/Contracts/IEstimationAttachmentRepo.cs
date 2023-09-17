using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimationAttachmentRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimationAttachmentRepo.Contracts
{
    public interface IEstimationAttachmentRepo
    {
        Task<int> CreateAttachment(CreateAttachmentRequest request);
        Task<List<EstimationAttachmentEntity>> GetAllAttachments();
        Task<EstimationAttachmentEntity> GetSingleAttachment(int id);
        Task UpdateAttachment(UpdateAttachmentRequest request);
        Task DeleteAttachment(int attachmentId);
        Task DeleteAttachmentByEstimate(int estimateId);
        Task<List<EstimationAttachmentEntity>> LoadAttachmentsByEstimate(int estimateId);
    }
}

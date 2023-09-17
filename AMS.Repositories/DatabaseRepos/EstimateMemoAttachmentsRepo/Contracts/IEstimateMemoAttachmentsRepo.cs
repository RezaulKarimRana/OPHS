using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo.Contracts
{
    public interface IEstimateMemoAttachmentsRepo
    {
        Task<int> CreateAttachmentForMemo(CreateAttachmentForMemoRequest request);
        Task<List<EstimateMemoAttachmentsEntity>> LoadMemoAttachmentsByEstimateMemo(int estimateMemoId);
        Task<bool> DeleteAttachmentsById(int id);
    }
}

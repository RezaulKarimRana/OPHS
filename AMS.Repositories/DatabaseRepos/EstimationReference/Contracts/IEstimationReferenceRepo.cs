using AMS.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.EstimationReference.Contracts
{
    public interface IEstimationReferenceRepo 
    {
        Task<EstimationReferenceEntity> GetEstimationReferenceById(int id);
        Task<EstimationReferenceEntity> GetEstimationReferenceByEstimate(int estimateId);
    }
}

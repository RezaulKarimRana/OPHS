using AMS.Models.DomainModels;
using AMS.Repositories.DatabaseRepos.ProjectRepo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMS.Repositories.DatabaseRepos.ProjectRepo.Contracts
{
    public interface IProjectRepo
    {
        Task<int> CreateProject(CreateProjectRequest request);
        Task<List<ProjectEntity>> GetAllProject();
        Task<ProjectEntity> GetSingleProject(int id);
        Task UpdateProject(UpdateProjectRequest request);
        Task DeleteProject(DeleteProjectRequest request);
    }
}

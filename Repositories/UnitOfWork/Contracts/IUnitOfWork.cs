using System;
using Repositories.DatabaseRepos.ConfigurationRepo.Contracts;
using Repositories.DatabaseRepos.DashboardRepo.Contracts;
using Repositories.DatabaseRepos.SessionRepo.Contracts;
using Repositories.DatabaseRepos.UserRepo.Contracts;

namespace Repositories.UnitOfWork.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IConfigurationRepo ConfigurationRepo { get; }
        IUserRepo UserRepo { get; }
        ISessionRepo SessionRepo { get; }
        IDashboardRepo DashboardRepo { get; }
        bool Commit();
    }
}

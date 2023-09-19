using System;
using AMS.Repositories.DatabaseRepos.ConfigurationRepo.Contracts;
using AMS.Repositories.DatabaseRepos.DashboardRepo.Contracts;
using AMS.Repositories.DatabaseRepos.SessionRepo.Contracts;
using AMS.Repositories.DatabaseRepos.UserRepo.Contracts;

namespace AMS.Repositories.UnitOfWork.Contracts
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

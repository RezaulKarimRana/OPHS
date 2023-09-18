using System.Threading.Tasks;
using AMS.Infrastructure.Cache;
using AMS.Infrastructure.Cache.Contracts;
using Models.ManagerModels.Admin;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Services.Managers.Contracts;

namespace AMS.Services.Managers
{
    public class AdminManager : IAdminManager
    {
        #region Instance Fields

        private readonly ICacheProvider _cacheProvider;
        private readonly IUnitOfWorkFactory _uowFactory;

        #endregion

        #region Constructor

        public AdminManager(
            IUnitOfWorkFactory uowFactory,
            ICacheProvider cacheProvider)
        {
            _uowFactory = uowFactory;
            _cacheProvider = cacheProvider;
        }

        #endregion

        #region Public Methods

        public async Task<CheckForAdminUserResponse> CheckForAdminUser()
        {
            var response = new CheckForAdminUserResponse();

            //if (_cacheProvider.TryGet(CacheConstants.AdminUserExists, out bool? requiresAdminUser))
            //{
            //    response.AdminUserExists = requiresAdminUser.Value;
            //    return response;
            //}

            var username = "superadmin"; // todo: move this to the configuration table
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var user = await uow.UserRepo.GetUserByUsername(new Repositories.DatabaseRepos.UserRepo.Models.GetUserByUsernameRequest()
                {
                    Username = username
                });

                if (user != null)
                {
                    response.AdminUserExists = true;
                    _cacheProvider.Set(CacheConstants.AdminUserExists, true);
                }
                else
                {
                    _cacheProvider.Set(CacheConstants.AdminUserExists, false);
                }
            }
            return response;
        }

        #endregion
    }
}

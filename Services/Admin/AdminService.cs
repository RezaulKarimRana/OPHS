using System.Threading.Tasks;
using Infrastructure.Authentication;
using Infrastructure.Cache;
using Infrastructure.Cache.Contracts;
using Infrastructure.Configuration;
using Infrastructure.Session;
using Infrastructure.Session.Contracts;
using Models.ServiceModels;
using Models.ServiceModels.Admin;
using Repositories.UnitOfWork.Contracts;
using Services.Admin.Contracts;
using Services.Contracts;
using Services.Managers.Contracts;

namespace Services.Admin
{
    public class AdminService : IAdminService
    {
        #region Instance Fields

        private readonly IAccountService _accountService;
        private readonly ISessionManager _sessionManager;
        private readonly ISessionProvider _sessionProvider;

        private readonly IAuthenticationManager _authenticationManager;

        private readonly IUnitOfWorkFactory _uowFactory;

        private readonly ICacheProvider _cacheProvider;

        private readonly IAdminManager _adminManager;

        #endregion

        #region Constructor

        public AdminService(
            IAccountService accountService,
            ISessionManager sessionManager,
            ISessionProvider sessionProvider,
            IUnitOfWorkFactory uowFactory,
            ICacheProvider cacheProvider,
            IAdminManager adminManager,
             IAuthenticationManager authenticationManager)
        {
            _uowFactory = uowFactory;
            _cacheProvider = cacheProvider;
            _accountService = accountService;
            _sessionManager = sessionManager;
            _sessionProvider = sessionProvider;
            _adminManager = adminManager;
            _authenticationManager = authenticationManager;
        }

        #endregion

        #region Public Methods

        public async Task<CheckIfCanCreateAdminUserResponse> CheckIfCanCreateAdminUser()
        {
            var response = new CheckIfCanCreateAdminUserResponse();

            var adminCheckResponse = await _adminManager.CheckForAdminUser();
            if (adminCheckResponse.AdminUserExists)
            {
                response.Notifications.AddError("Admin already exists");
            }
            return response;
        }

        public async Task<CreateAdminUserResponse> CreateAdminUser(CreateAdminUserRequest request)
        {
            var response = new CreateAdminUserResponse();
            var username = request.Username;
            var session = await _sessionManager.GetSession();

            var duplicateResponse = await _accountService.DuplicateUserCheck(new DuplicateUserCheckRequest()
            {
                Username = username
            });

            if (duplicateResponse.Notifications.HasErrors)
            {
                response.Notifications.Add(duplicateResponse.Notifications);
                return response;
            }

            int userId;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                userId = await uow.UserRepo.CreateUser(new Repositories.DatabaseRepos.UserRepo.Models.CreateUserRequest()
                {
                    Username = username,
                    First_Name = username,
                    Password_Hash = PasswordHelper.HashPassword(request.Password),
                    Created_By = ApplicationConstants.SystemUserId,
                    Registration_Confirmed = true,
                    Is_Enabled = true
                });

                await uow.UserRepo.CreateUserRole(new Repositories.DatabaseRepos.UserRepo.Models.CreateUserRoleRequest()
                {
                    User_Id = userId,
                    Role_Id = 1, // the first role should always be admin
                    Created_By = ApplicationConstants.SystemUserId
                });

                var sessionEntity = await uow.SessionRepo.AddUserToSession(new Repositories.DatabaseRepos.SessionRepo.Models.AddUserToSessionRequest()
                {
                    Id = session.SessionEntity.Id,
                    User_Id = userId,
                    Updated_By = ApplicationConstants.SystemUserId
                });
                await _sessionProvider.Set(SessionConstants.SessionEntity, sessionEntity);
                uow.Commit();
            }

            _cacheProvider.Set(CacheConstants.AdminUserExists, true);
            await _authenticationManager.SignIn(session.SessionEntity.Id);

            return response;
        }

        #endregion
    }
}

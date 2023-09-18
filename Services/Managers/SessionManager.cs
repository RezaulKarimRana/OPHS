using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AMS.Infrastructure.Configuration;
using AMS.Repositories.UnitOfWork.Contracts;
using AMS.Infrastructure.Session;
using AMS.Infrastructure.Session.Contracts;
using Models.DomainModels;
using Models.ManagerModels.Session;
using AMS.Services.Managers.Contracts;
using Microsoft.Net.Http.Headers;

namespace AMS.Services.Managers
{
    public class SessionManager : ISessionManager
    {
        #region Instance Fields

        private readonly ISessionProvider _sessionProvider;
        private readonly ICacheManager _cache;
        private readonly IUnitOfWorkFactory _uowFactory;

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        public SessionManager(
            ISessionProvider sessionProvider,
            IUnitOfWorkFactory uowFactory,
            ICacheManager cache,
            IHttpContextAccessor httpContextAccessor)
        {
            _uowFactory = uowFactory;
            _sessionProvider = sessionProvider;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        #endregion

        #region Public Methods

        public async Task<GetSessionResponse> GetSession()
        {
            var response = new GetSessionResponse();

            // get or create a new session
            var session = await _sessionProvider.Get<SessionEntity>(SessionConstants.SessionEntity);
            if (session == null)
            {
                // flush any authenticated cookies in the event the application restarts
                await _httpContextAccessor.HttpContext.SignOutAsync();
                await _sessionProvider.Clear();

                var userAgent = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent].FirstOrDefault();

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    response.SessionEntity = await uow.SessionRepo.CreateSession(new Repositories.DatabaseRepos.SessionRepo.Models.CreateSessionRequest()
                    {
                        User_Agent = userAgent,
                        Created_By = ApplicationConstants.SystemUserId
                    });
                    uow.Commit();
                }

                await _sessionProvider.Set(SessionConstants.SessionEntity, response.SessionEntity);
                return response;
            }
            response.SessionEntity = session;

            var sessionLogId = await _sessionProvider.Get<int?>(SessionConstants.SessionLogId);
            if (sessionLogId != null)
            {
                response.SessionLogId = sessionLogId.Value;
            }

            var isDebug = await _sessionProvider.Get<bool?>(SessionConstants.IsDebug);
            if (isDebug != null)
            {
                response.IsDebug = isDebug ?? false;
            }

            return response;
        }

        public async Task<List<PermissionEntity>> GetPermissions()
        {
            var permissions = await _sessionProvider.Get<List<PermissionEntity>>(SessionConstants.UserPermissions);
            if (permissions != null)
            {
                return permissions;
            }

            var session = await GetSession();
            if (session.SessionEntity.User_Id.HasValue)
            {
                var userId = session.SessionEntity.User_Id.Value;

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var userRoles = await uow.UserRepo.GetUserRolesByUserId(new Repositories.DatabaseRepos.UserRepo.Models.GetUserRolesByUserIdRequest()
                    {
                        User_Id = userId
                    });
                    var rolePermissionsLookup = await _cache.RolePermissions();
                    var rolePermissions = rolePermissionsLookup.Where(rp => userRoles.Select(ur => ur.Role_Id).Contains(rp.Role_Id));

                    var userPermissions = await uow.UserRepo.GetUserPermissionsByUserId(new Repositories.DatabaseRepos.UserRepo.Models.GetUserPermissionsByIdRequest()
                    {
                        User_Id = userId
                    });
                    
                    var permissionIds = userPermissions.Select(up => up.Permission_Id).Concat(rolePermissions.Select(up => up.Permission_Id)).Distinct();

                    permissions = await uow.UserRepo.GetPermissions();
                    uow.Commit();

                    permissions = permissions.Where(p => permissionIds.Contains(p.Id)).ToList();
                }

                await _sessionProvider.Set(SessionConstants.UserPermissions, permissions);
            }
            return permissions;
        }

        public async Task DehydrateSession()
        {
            // exclude our esssential session variable else we may lose our session
            var essentialSessionVariables = new List<string>()
            {
                SessionConstants.SessionEntity,
                SessionConstants.SessionLogId,
                SessionConstants.IsDebug
            };

            var classToSearch = typeof(SessionConstants);
            var sessionKeys = classToSearch.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly
                                    && !essentialSessionVariables.Contains(fi.Name)).ToList();

            foreach (var key in sessionKeys)
            {
                await _sessionProvider.Remove((string)key.GetRawConstantValue());
            }
        }

        public async Task WriteSessionLogEvent(CreateSessionLogEventRequest request)
        {
            var session = await GetSession();

            var events = await _cache.SessionEvents();
            var eventItem = events.FirstOrDefault(e => e.Key == request.EventKey);

            if (eventItem == null)
            {
                // todo: log this
                return;
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.SessionRepo.CreateSessionLogEvent(new Repositories.DatabaseRepos.SessionRepo.Models.CreateSessionLogEventRequest()
                {
                    Session_Log_Id = session.SessionLogId,
                    Event_Id = eventItem.Id,
                    InfoDictionary_JSON = JsonConvert.SerializeObject(request.Info),
                    Created_By = ApplicationConstants.SystemUserId
                }); ;
                uow.Commit();
            }
        }

        public async Task<UserEntity?> GetUser()
        {
            var user = await _sessionProvider.Get<UserEntity>(SessionConstants.UserEntity);
            if (user != null)
            {
                return user;
            }

            var session = await GetSession();
            var userId = session.SessionEntity.User_Id;

            if (!userId.HasValue)
            {
                return null;
            }

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                user = await uow.UserRepo.GetUserById(new Repositories.DatabaseRepos.UserRepo.Models.GetUserByIdRequest()
                {
                    Id = userId.Value
                });
                uow.Commit();
            }

            await _sessionProvider.Set(SessionConstants.UserEntity, user);
            return user;
        }

        public async Task AddUserToSession(int sessionId, int userId)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var sessionEntity = await uow.SessionRepo.AddUserToSession(new Repositories.DatabaseRepos.SessionRepo.Models.AddUserToSessionRequest()
                {
                    Id = sessionId,
                    User_Id = userId,
                    Updated_By = ApplicationConstants.SystemUserId
                });
                uow.Commit();

                await _sessionProvider.Set(SessionConstants.SessionEntity, sessionEntity);
            }
        }

        #endregion
    }
}

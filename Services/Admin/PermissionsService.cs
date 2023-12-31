﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Notifications;
using Infrastructure.Cache;
using Infrastructure.Session;
using Models.ServiceModels.Admin.Permissions;
using Repositories.UnitOfWork.Contracts;
using Services.Admin.Contracts;
using Services.Managers.Contracts;

namespace Services.Admin
{
    public class PermissionsService : IPermissionsService
    {
        #region Instance Fields

        private readonly ISessionManager _sessionManager;
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly ICacheManager _cache;

        #endregion

        #region Constructor

        public PermissionsService(
            ISessionManager sessionManager,
            IUnitOfWorkFactory uowFactory,
            ICacheManager cache)
        {
            _uowFactory = uowFactory;
            _cache = cache;
            _sessionManager = sessionManager;
        }

        #endregion

        #region Public Methods

        public async Task<GetPermissionsResponse> GetPermissions()
        {
            var response = new GetPermissionsResponse
            {
                Permissions = await _cache.Permissions()
            };

            return response;
        }

        public async Task<GetPermissionResponse> GetPermission(GetPermissionRequest request)
        {
            var response = new GetPermissionResponse();

            var permissions = await _cache.Permissions();
            var permission = permissions.FirstOrDefault(c => c.Id == request.Id);

            response.Permission = permission;

            return response;
        }

        public async Task<UpdatePermissionResponse> UpdatePermission(UpdatePermissionRequest request)
        {
            var sessionUser = await _sessionManager.GetUser();
            var response = new UpdatePermissionResponse();

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                await uow.UserRepo.UpdatePermission(new Repositories.DatabaseRepos.UserRepo.Models.UpdatePermissionRequest()
                {
                    Id = request.Id,
                    Name = request.Name,
                    Group_Name = request.GroupName,
                    Description = request.Description,
                    Updated_By = sessionUser.Id
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.Permissions);

            var permissions = await _cache.Permissions();
            var permission = permissions.FirstOrDefault(c => c.Id == request.Id);

            await _sessionManager.WriteSessionLogEvent(new Models.ManagerModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.PermissionUpdated,
                Info = new Dictionary<string, string>()
                {
                    { "Key", permission.Key }
                }
            });

            response.Notifications.Add($"Permission '{permission.Key}' has been updated", NotificationTypeEnum.Success);
            return response;
        }

        public async Task<CreatePermissionResponse> CreatePermission(CreatePermissionRequest request)
        {
            var sessionUser = await _sessionManager.GetUser();
            var response = new CreatePermissionResponse();

            var permissions = await _cache.Permissions();
            var permission = permissions.FirstOrDefault(c => c.Key == request.Key);

            if (permission != null)
            {
                response.Notifications.AddError($"A permission already exists with the key {request.Key}");
                return response;
            }

            int id;
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                id = await uow.UserRepo.CreatePermission(new Repositories.DatabaseRepos.UserRepo.Models.CreatePermissionRequest()
                {
                    Key = request.Key,
                    Description = request.Description,
                    Group_Name = request.GroupName,
                    Name = request.Name,
                    Created_By = sessionUser.Id
                });
                uow.Commit();
            }

            _cache.Remove(CacheConstants.Permissions);

            await _sessionManager.WriteSessionLogEvent(new Models.ManagerModels.Session.CreateSessionLogEventRequest()
            {
                EventKey = SessionEventKeys.PermissionCreated,
                Info = new Dictionary<string, string>()
                {
                    { "Key", request.Key }
                }
            });

            response.Notifications.Add($"Permission '{request.Key}' has been created", NotificationTypeEnum.Success);
            return response;
        }

        #endregion
    }
}

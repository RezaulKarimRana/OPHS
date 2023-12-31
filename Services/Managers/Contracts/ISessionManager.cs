﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DomainModels;
using Models.ManagerModels.Session;

namespace Services.Managers.Contracts
{
    public interface ISessionManager
    {
        /// <summary>
        /// Gets the current session creating one if none exists
        /// </summary>
        /// <returns></returns>
        Task<GetSessionResponse> GetSession();

        /// <summary>
        /// Gets permissions granted for the current session
        /// </summary>
        /// <returns></returns>
        Task<List<PermissionEntity>> GetPermissions();

        /// <summary>
        /// Writes a session log even
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task WriteSessionLogEvent(Models.ManagerModels.Session.CreateSessionLogEventRequest request);

        /// <summary>
        /// Clears session of all non-essential variables using reflection on SessionConstants.cs
        /// </summary>
        /// <returns></returns>
        Task DehydrateSession();

        /// <summary>
        /// Binds a user to the session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task AddUserToSession(int sessionId, int userId);

        Task<UserEntity?> GetUser();
    }
}

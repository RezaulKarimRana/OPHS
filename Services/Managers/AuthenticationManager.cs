using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AMS.Infrastructure.Session;
using AMS.Infrastructure.Session.Contracts;
using AMS.Services.Managers.Contracts;

namespace AMS.Services.Managers
{
    public class AuthenticationManager : IAuthenticationManager
    {
        #region Instance Fields

        private readonly ISessionProvider _sessionProvider;
        private readonly ISessionManager _sessionManager;

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        public AuthenticationManager(
            ISessionProvider sessionProvider,
            ISessionManager sessionManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _sessionProvider = sessionProvider;
            _httpContextAccessor = httpContextAccessor;
            _sessionManager = sessionManager;
        }

        #endregion

        #region Public Methods

        public async Task SignIn(int sessionId)
        {
            await _sessionManager.DehydrateSession();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, sessionId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        public async Task SignOut()
        {
            var session = await _sessionManager.GetSession();

            await _sessionProvider.Clear();
            await _httpContextAccessor.HttpContext.SignOutAsync();

            // keep essential variables if the user continues to browse around
            await _sessionProvider.Set(SessionConstants.IsDebug, session.IsDebug);
            await _sessionProvider.Set(SessionConstants.SessionLogId, session.SessionLogId); // keeps state for the duration of the current request
        }

        #endregion
    }
}

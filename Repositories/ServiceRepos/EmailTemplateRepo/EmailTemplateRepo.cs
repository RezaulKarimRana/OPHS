using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Extensions;
using Infrastructure.HttpClients;
using Repositories.ServiceRepos.EmailTemplateRepo.Contracts;

namespace Repositories.ServiceRepos.EmailTemplateRepo
{
    public class EmailTemplateRepo : IEmailTemplateRepo
    {
        #region Instance Fields

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        public EmailTemplateRepo(IHttpClientFactory httpClientFactory, 
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Public Methods

        public async Task<string> GetResetPasswordHTML()
        {
            var baseUrl = _httpContextAccessor.HttpContext.Request.GetBaseUrl();
            var httpResponse = await HttpHelper.Get(_httpClientFactory, baseUrl, "Email/ResetPassword");

            var html = httpResponse.Content.ReadAsStringAsync().Result;
            return html;
        }

        public async Task<string> GetAccountActivationHTML()
        {
            var baseUrl = _httpContextAccessor.HttpContext.Request.GetBaseUrl();
            var httpResponse = await HttpHelper.Get(_httpClientFactory, baseUrl, "Email/AccountActivation");

            var html = httpResponse.Content.ReadAsStringAsync().Result;
            return html;
        }

        public async Task<string> GetSendFeedbackHTML()
        {
            var baseUrl = _httpContextAccessor.HttpContext.Request.GetBaseUrl();
            var httpResponse = await HttpHelper.Get(_httpClientFactory, baseUrl, "Email/SendFeedback");

            var html = httpResponse.Content.ReadAsStringAsync().Result;
            return html;
        }

        #endregion
    }
}

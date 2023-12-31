using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using Models.ServiceModels.Admin.Sessions;
using Services.Admin.Contracts;

namespace Web.Pages
{
    public class ManageSessionsModel : PageModel
    {
        #region Private Fields

        private readonly ISessionService _sessionService;

        #endregion

        #region Properties

        public int SessionExpirationMinutes { get; set; }

        #endregion

        #region Constructors

        public ManageSessionsModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
            SessionExpirationMinutes = 1;
        }

        #endregion

        public async Task OnGet()
        {
       
        }

        public async Task<JsonResult> OnGetLastXDays(int days)
        {
            var response = await _sessionService.GetSessions(new GetSessionsRequest()
            {
                LastXDays = days
            });

            return new JsonResult(response);
        }

        public async Task<JsonResult> OnGetFilterByDate(DateTime date)
        {
            var response = await _sessionService.GetSessions(new GetSessionsRequest()
            {
                Day = date
            });

            return new JsonResult(response);
        }

        public async Task<JsonResult> OnGetFilterByUserId(int userId)
        {
            var response = await _sessionService.GetSessions(new GetSessionsRequest()
            {
                UserId = userId
            });

            return new JsonResult(response);
        }

        public async Task<JsonResult> OnGetFilterByMobileNumber(string mobileNumber)
        {
            var response = await _sessionService.GetSessions(new GetSessionsRequest()
            {
                MobileNumber = mobileNumber
            });

            return new JsonResult(response);
        }

        public async Task<JsonResult> OnGetFilterByUsername(string username)
        {
            var response = await _sessionService.GetSessions(new GetSessionsRequest()
            {
                Username = username
            });

            return new JsonResult(response);
        }

        public async Task<JsonResult> OnGetRecent()
        {
            var response = await _sessionService.GetSessions(new GetSessionsRequest()
            {
                Last24Hours = true
            });

            return new JsonResult(response);
        }

        public async Task<JsonResult> OnGetFilterByEmailAddress(string emailAddress)
        {
            var response = await _sessionService.GetSessions(new GetSessionsRequest()
            {
                EmailAddress = emailAddress
            });

            return new JsonResult(response);
        }
    }
}

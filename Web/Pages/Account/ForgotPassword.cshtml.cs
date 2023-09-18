using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Models.ServiceModels;
using AMS.Services.Contracts;
using System;
using System.Text;

namespace AMS.Web.Pages
{
    public class ForgotPasswordModel : BasePageModel
    {
        #region Private Fields

        private readonly IAccountService _service;
        private readonly IHtmlGeneratorService _htmlGeneratorService;

        #endregion

        #region Properties

        [BindProperty]
        public ForgotPasswordRequest FormData { get; set; }

        #endregion

        #region Constructors

        public ForgotPasswordModel(IAccountService service, IHtmlGeneratorService htmlGeneratorService)
        {
            _service = service;
            _htmlGeneratorService = htmlGeneratorService;
            FormData = new ForgotPasswordRequest();
        }

        #endregion

        public void OnGet()
        {
            FormData = new ForgotPasswordRequest();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var password = CreateRandomPassword();
                var emailBody = await _htmlGeneratorService.getPasswordResetEmailBody(password);
                var response = await _service.ForgotPassword(FormData, emailBody, password);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToLogin();
                }
                else
                {
                    AddFormErrors(response);
                }
            }
            return Page();
        }

        public string CreateRandomPassword()
        {
            int passwordLength = 6;

            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < passwordLength--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}

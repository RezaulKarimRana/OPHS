using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Models.ServiceModels;
using Services.Contracts;
using System;
using System.Text;

namespace Web.Pages
{
    public class ForgotPasswordModel : BasePageModel
    {
        #region Private Fields

        private readonly IAccountService _service;

        #endregion

        #region Properties

        [BindProperty]
        public ForgotPasswordRequest FormData { get; set; }

        #endregion

        #region Constructors

        public ForgotPasswordModel(IAccountService service)
        {
            _service = service;
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
                var emailBody = string.Empty;
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

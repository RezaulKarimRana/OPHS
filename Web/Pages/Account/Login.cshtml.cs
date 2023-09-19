using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.ServiceModels;
using Services.Contracts;

namespace Web.Pages
{
    public class LoginModel : BasePageModel
    {

        private readonly IAccountService _accountService;

        [BindProperty]
        public LoginRequest FormData { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public bool ShowPassword { get; set; }

        public string ErrMessage { get; set; }

        public LoginModel(IAccountService accountService)
        {
            _accountService = accountService;
            FormData = new LoginRequest();
        }

        public void OnGet(string returnUrl = "/")
        {
            FormData = new LoginRequest();
            ViewData["ReturnUrl"] = returnUrl;
        }

        public async Task<IActionResult> OnPost(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "/";
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                var response = await _accountService.Login(FormData);
                ModelState.AddModelError("success", response.IsSuccessful.ToString());
                if (response.IsSuccessful)
                {
                    return RedirectToHome(ReturnUrl);
                }
                AddFormErrors(response);
            }
            FormData = new LoginRequest();
            ViewData["ReturnUrl"] = "/";
            return Page();
        }

    }
}

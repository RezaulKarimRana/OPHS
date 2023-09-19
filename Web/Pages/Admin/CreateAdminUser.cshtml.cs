using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Models.ServiceModels.Admin;
using Services.Admin.Contracts;

namespace Web.Pages
{
    public class CreateAdminUserModel : BasePageModel
    {
        #region Private Fields

        private readonly IAdminService _adminService;

        #endregion

        #region Properties

        [BindProperty]
        public CreateAdminUserRequest FormData { get; set; }

        #endregion

        #region Constructors

        public CreateAdminUserModel(IAdminService adminService)
        {
            _adminService = adminService;
            FormData = new CreateAdminUserRequest();
        }

        #endregion

        public async Task<IActionResult> OnGet()
        {
            var response = await _adminService.CheckIfCanCreateAdminUser();
            if (!response.IsSuccessful)
            {
                AddNotifications(response);
                return RedirectToHome();
            }
            FormData = new CreateAdminUserRequest();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var response = await _adminService.CreateAdminUser(FormData);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToHome();
                }
                AddFormErrors(response);
            }
            return Page();
        }
    }
}

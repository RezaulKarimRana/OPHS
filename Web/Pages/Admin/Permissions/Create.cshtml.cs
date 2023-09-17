using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AMS.Models.ServiceModels.Admin.Permissions;
using AMS.Services.Admin.Contracts;

namespace AMS.Web.Pages
{
    public class CreatePermissionModel : BasePageModel
    {
        #region Private Fields

        private readonly IPermissionsService _permissionsService;

        #endregion

        #region Properties

        [BindProperty]
        public CreatePermissionRequest FormData { get; set; }

        #endregion

        #region Constructors

        public CreatePermissionModel(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;
            FormData = new CreatePermissionRequest();
        }

        #endregion

        public async Task OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var response = await _permissionsService.CreatePermission(FormData);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToPage("/Admin/Permissions/Index");
                }
                AddFormErrors(response);
            }
            return Page();
        }
    }
}

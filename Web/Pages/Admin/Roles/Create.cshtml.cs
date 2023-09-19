using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DomainModels;
using Models.ServiceModels.Admin.Roles;
using Services.Admin.Contracts;
using Services.Managers.Contracts;

namespace Web.Pages
{
    public class CreateRoleModel : BasePageModel
    {
        #region Private Fields

        private readonly IRoleService _roleService;
        private readonly ICacheManager _cache;

        #endregion

        #region Properties

        [BindProperty]
        public CreateRoleRequest FormData { get; set; }

        public List<PermissionEntity> PermissionsLookup { get; set; }

        #endregion

        #region Constructors

        public CreateRoleModel(IRoleService roleService, ICacheManager cache)
        {
            _roleService = roleService;
            _cache = cache;
            PermissionsLookup = new List<PermissionEntity>();
            FormData = new CreateRoleRequest();
        }

        #endregion

        public async Task OnGet()
        {
            PermissionsLookup = await _cache.Permissions();
            FormData = new CreateRoleRequest();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var response = await _roleService.CreateRole(FormData);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToPage("/Admin/Roles/Index");
                }
                AddFormErrors(response);
            }
            return Page();
        }
    }
}

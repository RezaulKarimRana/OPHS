using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Models.ServiceModels.Admin.Roles;
using Models.DomainModels;
using System.Collections.Generic;
using System.Linq;
using Services.Managers.Contracts;
using Services.Admin.Contracts;
using Models;

namespace Web.Pages
{
    public class EditRoleModel : BasePageModel
    {
        #region Private Fields

        private readonly IRoleService _roleService;
        private readonly ICacheManager _cache;

        #endregion

        #region Properties

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public UpdateRoleRequest FormData { get; set; }

        public RoleEntity? RoleEntity { get; set; }

        public List<PermissionEntity> PermissionsLookup { get; set; }

        #endregion

        #region Constructors

        public EditRoleModel(IRoleService roleService, ICacheManager cache)
        {
            _roleService = roleService;
            _cache = cache;
            PermissionsLookup = new List<PermissionEntity>();
            FormData = new UpdateRoleRequest();
        }

        #endregion

        public async Task<IActionResult> OnGet()
        {
            var response = await _roleService.GetRole(new GetRoleRequest()
            {
                Id = Id
            });

            if (!response.IsSuccessful)
            {
                return NotFound();
            }

            PermissionsLookup = await _cache.Permissions();
            RoleEntity = response.Role;
            FormData = new UpdateRoleRequest()
            {
                Id = response.Role.Id,
                Description = response.Role.Description,
                Name = response.Role.Name,
                PermissionIds = response.Permissions.Select(c => new CheckboxItemSelection() {
                    Id = c.Id,
                    Selected = true
                }).ToList()
            };

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                if (FormData != null)
                {
                    FormData.Id = Id;
                }
                var response = await _roleService.UpdateRole(FormData);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToPage("/Admin/Roles/Index");
                }
                AddFormErrors(response);
            }
            var roleResponse = await _roleService.GetRole(new GetRoleRequest()
            {
                Id = Id
            });
            PermissionsLookup = await _cache.Permissions();
            RoleEntity = roleResponse.Role;
            return Page();
        }

        public async Task<IActionResult> OnPostDisableRole(int id)
        {
            var response = await _roleService.DisableRole(new DisableRoleRequest()
            {
                Id = id
            });
            AddNotifications(response);
            return RedirectToPage("/Admin/Roles/Index");
        }

        public async Task<IActionResult> OnPostEnableRole(int id)
        {
            var response = await _roleService.EnableRole(new EnableRoleRequest()
            {
                Id = id
            });
            AddNotifications(response);
            return RedirectToPage("/Admin/Roles/Index");
        }
    }
}

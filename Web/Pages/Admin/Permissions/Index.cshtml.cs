using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models.DomainModels;
using AMS.Services.Admin.Contracts;

namespace AMS.Web.Pages
{
    public class ManagePermissionsModel : PageModel
    {
        #region Private Fields

        private readonly IPermissionsService _permissionsService;

        #endregion

        #region Properties

        public List<PermissionEntity> Permissions { get; set; }

        #endregion

        #region Constructors

        public ManagePermissionsModel(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;

            Permissions = new List<PermissionEntity>();
        }

        #endregion

        public async Task OnGet()
        {
            var response = await _permissionsService.GetPermissions();
            Permissions = response.Permissions;
        }
    }
}

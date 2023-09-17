using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Users;
using AMS.Services.Admin.Contracts;
using AMS.Services.Managers.Contracts;

namespace AMS.Web.Pages
{
    public class CreateUserModel : BasePageModel
    {
        #region Private Fields

        private readonly IUserService _userService;
        private readonly ICacheManager _cache;
        private readonly IDepartmentService _deptService;

        #endregion

        #region Properties

        [BindProperty]
        public CreateUserRequest FormData { get; set; }
        public List<RoleEntity> RolesLookup { get; set; }
        public IList<DepartmentEntity> Department { get; set; }

        #endregion

        #region Constructors

        public CreateUserModel(IUserService userService, ICacheManager cache, IDepartmentService deptService)
        {
            _userService = userService;
            _cache = cache;
            _deptService = deptService;
            RolesLookup = new List<RoleEntity>();
            Department = new List<DepartmentEntity>();
            FormData = new CreateUserRequest();
        }

        #endregion

        public async Task OnGet()
        {
            RolesLookup = await _cache.Roles();
            var deptResponse = await _deptService.GetAllDepartments();
            Department = deptResponse.Departments;
            FormData = new CreateUserRequest();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var response = await _userService.CreateUser(FormData);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToPage("/Admin/Users/Index");
                }
                AddFormErrors(response);
            }
            RolesLookup = await _cache.Roles();
            return Page();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMS.Models.DomainModels;
using AMS.Models.ServiceModels.Admin.Users;
using AMS.Services.Admin.Contracts;
using AMS.Services.Managers.Contracts;

namespace AMS.Web.Pages
{
    public class EditUserModel : BasePageModel
    {
        #region Private Fields

        private readonly IUserService _userService;
        private readonly IDepartmentService _deptService;
        private readonly ICacheManager _cache;

        #endregion

        #region Properties

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public UpdateUserRequest FormData { get; set; }

        public UserEntity? UserEntity { get; set; }

        public List<RoleEntity> RolesLookup { get; set; }
        public IList<DepartmentEntity> Department { get; set; }

        #endregion

        #region Constructors

        public EditUserModel(IUserService userService, ICacheManager cache, IDepartmentService deptService)
        {
            _userService = userService;
            _cache = cache;
            _deptService = deptService;
            RolesLookup = new List<RoleEntity>();
            Department = new List<DepartmentEntity>();
            FormData = new UpdateUserRequest();
        }

        #endregion

        public async Task<IActionResult> OnGet()
        {
            var response = await _userService.GetUser(new GetUserRequest()
            {
                Id = Id
            });

            if (!response.IsSuccessful)
            {
                return NotFound();
            }

            RolesLookup = await _cache.Roles();
            var deptResponse = await _deptService.GetAllDepartments();
            Department = deptResponse.Departments;

            UserEntity = response.User;
            FormData = new UpdateUserRequest()
            {
                Username = response.User.Username,
                EmailAddress = response.User.Email_Address,
                FirstName = response.User.First_Name,
                LastName = response.User.Last_Name,
                MobileNumber = response.User.Mobile_Number,
                RoleIds = response.Roles.Select(r => r.Id).ToList(),
                DepartmentId = response.User.Department_Id
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
                var response = await _userService.UpdateUser(FormData);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToPage("/Admin/Users/Index");
                }
                AddFormErrors(response);
            }
            var userResponse = await _userService.GetUser(new GetUserRequest()
            {
                Id = Id
            });
            RolesLookup = await _cache.Roles();
            UserEntity = userResponse.User;
            return Page();
        }

        public async Task<IActionResult> OnPostDisableUser(int id)
        {
            var response = await _userService.DisableUser(new Models.ServiceModels.Admin.Users.DisableUserRequest()
            {
                Id = id
            });
            AddNotifications(response);
            return RedirectToPage("/Admin/Users/Index");
        }

        public async Task<IActionResult> OnPostEnableUser(int id)
        {
            var response = await _userService.EnableUser(new Models.ServiceModels.Admin.Users.EnableUserRequest()
            {
                Id = id
            });
            AddNotifications(response);
            return RedirectToPage("/Admin/Users/Index");
        }

        public async Task<IActionResult> OnPostUnlockUser(int id)
        {
            var response = await _userService.UnlockUser(new Models.ServiceModels.Admin.Users.UnlockUserRequest()
            {
                Id = id
            });
            AddNotifications(response);
            return RedirectToPage("/Admin/Users/Index");
        }

        public async Task<IActionResult> OnPostConfirmRegistration(int id)
        {
            var response = await _userService.ConfirmRegistration(new Models.ServiceModels.Admin.Users.ConfirmRegistrationRequest()
            {
                Id = id
            });
            AddNotifications(response);
            return RedirectToPage("/Admin/Users/Index");
        }

        public async Task<IActionResult> OnPostGenerateResetPasswordUrl([FromBody] GenerateResetPasswordUrlRequest request)
        {
            ModelState.Clear(); // needed to prevent other forms being included in validation
            if (TryValidateModel(request))
            {
                var response = await _userService.GenerateResetPasswordUrl(request);
                if (response.IsSuccessful)
                {
                    return new JsonResult(response);
                }
                AddFormErrors(response);
                return new UnprocessableEntityObjectResult(ModelState);
            }
            return new BadRequestObjectResult(ModelState);
        }

        public async Task<IActionResult> OnPostSendResetPasswordEmail([FromBody] SendResetPasswordEmailRequest request)
        {
            ModelState.Clear(); // needed to prevent other forms being included in validation
            if (TryValidateModel(request))
            {
                var response = await _userService.SendResetPasswordEmail(request);
                if (response.IsSuccessful)
                {
                    return new JsonResult(response);
                }
                AddFormErrors(response);
                return new UnprocessableEntityObjectResult(ModelState);
            }
            return new BadRequestObjectResult(ModelState);
        }
    }
}

using AMS.Services.Admin.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AMS.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService _department;

        public DepartmentController(IDepartmentService department)
        {
            _department = department;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var response = await _department.GetAllDepartments();
            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartmentsJoinUser()
        {
            var response = await _department.GetAllDepartmentsJoiningUser();
            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartmentsJoinUserByConfiguration()
        {
            var response = await _department.GetAllDepartmentsJoinUserByConfiguration();
            return new JsonResult(response);
        }
    }
}

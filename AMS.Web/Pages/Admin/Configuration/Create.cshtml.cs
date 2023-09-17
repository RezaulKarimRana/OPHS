using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AMS.Models.ServiceModels.Admin.Configuration;
using AMS.Services.Admin.Contracts;

namespace AMS.Web.Pages
{
    public class CreateConfigurationModel : BasePageModel
    {
        #region Private Fields

        private readonly IConfigurationService _configService;

        #endregion

        #region Properties

        [BindProperty]
        public CreateConfigurationItemRequest FormData { get; set; }

        #endregion

        #region Constructors

        public CreateConfigurationModel(IConfigurationService configService)
        {
            _configService = configService;
            FormData = new CreateConfigurationItemRequest();
        }

        #endregion

        public async Task OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var response = await _configService.CreateConfigurationItem(FormData);
                if (response.IsSuccessful)
                {
                    AddNotifications(response);
                    return RedirectToPage("/Admin/Configuration/Index");
                }
                AddFormErrors(response);
            }
            return Page();
        }
    }
}

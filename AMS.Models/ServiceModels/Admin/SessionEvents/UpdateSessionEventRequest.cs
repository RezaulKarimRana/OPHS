using System.ComponentModel.DataAnnotations;

namespace AMS.Models.ServiceModels.Admin.SessionEvents
{
    public class UpdateSessionEventRequest
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }
    }
}

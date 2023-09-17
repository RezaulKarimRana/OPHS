using System.ComponentModel.DataAnnotations;

namespace AMS.Models.ServiceModels
{
    public class DuplicateRoleCheckRequest
    {
        [Required]
        public string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Models.ServiceModels
{
    public class DuplicateRoleCheckRequest
    {
        [Required]
        public string Name { get; set; }
    }
}

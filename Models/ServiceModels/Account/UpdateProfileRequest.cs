using System.ComponentModel.DataAnnotations;

namespace Models.ServiceModels.Account
{
    public class UpdateProfileRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile number")]
        public string MobileNumber { get; set; }
        [Display(Name = "Department name")]
        public int DepartmentId { get; set; }
        [Display(Name = "Designation name")]
        public int DesignationId { get; set; }

    }
}


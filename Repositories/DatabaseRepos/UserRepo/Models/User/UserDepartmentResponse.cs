using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.DatabaseRepos.UserRepo.Models.User
{
    public class UserDepartmentResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public string Email_Address { get; set; }

        public bool Registration_Confirmed { get; set; }

        public string First_Name { get; set; }

        public string Last_Name { get; set; }

        public string Mobile_Number { get; set; }

        public string Password_Hash { get; set; }

        public DateTime? Lockout_End { get; set; }

        public int Invalid_Login_Attempts { get; set; }

        public bool Is_Locked_Out { get; set; }

        public bool Is_Enabled { get; set; }

        public int CanEdit { get; set; }

        public int Department_Id { get; set; }
        public string DepartmentName { get; set; }
        public bool DepartmentEditPermission { get; set; }

        public int Created_By { get; set; }

        public DateTime Created_Date { get; set; }

        public int Updated_By { get; set; }

        public DateTime Updated_Date { get; set; }

        public bool Is_Deleted { get; set; }
    }
}

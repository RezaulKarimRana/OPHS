using System;

namespace Models.DomainModels
{
    public class UserEntity : BaseEntity
    {
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

        // Initially this 'CanEdit' value can be 0 , 1 , 2.
        // '0' means readonly mode. User can just approve.
        // '1' means self-edited mode. User can edit just their responsible department's items.
        // '2' means all item edited mode.
        public int CanEdit { get; set; }
        
        public int Department_Id { get; set; }
        public bool CanRollBack { get; set; }
    }
}

using System;

namespace AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models
{
    public class CreateEmailContantRequest
    {
        public CreateEmailContantRequest()
        {
            VersionNumber = 2;
            FromEmail = "AMS.SComm@summitcommunications.net";
            ToBcc = "asif.ahmed@summitcommunications.net,nowshin.laila@summitcommunications.net";
            Status = "Pending";
            CreationDate = DateTime.Now;
            ModificationDate = DateTime.Now;
        }
        public int VersionNumber { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string ToCc { get; set; }
        public string ToBcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModificationDate { get; set; }
        public string AMSID { get; set; }
        public int Department { get; set; }
    }
}

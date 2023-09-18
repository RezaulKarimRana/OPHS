using System;

namespace Models.DomainModels
{
    public class EmailContentsEntity
    {
        public EmailContentsEntity()
        {
            VersionNumber = 2;
            ToBcc = "asif.ahmed@summitcommunications.net,nowshin.laila@summitcommunications.net,rolloutplannersystem@summitcommunications.net";
            Status = "Pending";
            CreationDate = DateTime.Now;
            ModificationDate = DateTime.Now;
        }
        public long Id { get; set; }
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

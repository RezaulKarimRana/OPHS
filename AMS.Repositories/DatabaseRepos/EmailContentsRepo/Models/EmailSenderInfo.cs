using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Repositories.DatabaseRepos.EmailContentsRepo.Models
{
    public class EmailSenderInfo
    {
        public string NextApprover { get; set; }
        public string AllApproverExceptNextApprover { get; set; }
        public string AllApproverMail { get; set; }
        public string creatorEmailAddress { get; set; }
    }
}

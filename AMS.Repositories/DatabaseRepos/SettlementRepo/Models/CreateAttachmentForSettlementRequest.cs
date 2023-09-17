using AMS.Repositories.DatabaseRepos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Repositories.DatabaseRepos.SettlementRepo.Models
{
    public class CreateAttachmentForSettlementRequest : CreatedBy
    {
        public string URL { get; set; }
        public string FileName { get; set; }
        public int EstimationSettlement_Id { get; set; }
    }
}

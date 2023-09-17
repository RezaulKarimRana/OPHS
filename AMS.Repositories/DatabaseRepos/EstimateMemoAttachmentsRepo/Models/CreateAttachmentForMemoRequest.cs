using AMS.Repositories.DatabaseRepos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Repositories.DatabaseRepos.EstimateMemoAttachmentsRepo.Models
{
    public class CreateAttachmentForMemoRequest : CreatedBy
    {
        public string URL { get; set; }
        public string FileName { get; set; }
        public int EstimationMemo_Id { get; set; }
    }
}

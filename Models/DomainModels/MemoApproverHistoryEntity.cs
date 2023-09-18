using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DomainModels
{
    public class MemoApproverHistoryEntity : BaseEntity
    {
        public int EstimateMemoId { get; set; }
        public int UserId { get; set; }
        public int RolePriorityId { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; }
    }
}

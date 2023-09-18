using System;
using System.Collections.Generic;
using System.Text;

namespace Models.CustomModels
{
    public class LoadMemoApproverFeedBackDetails
    {
        public int MemoApproverTableRowId { get; set; }
        public int MemoId { get; set; }
        public int ApproverUserId { get; set; }
        public string ApproverUserName { get; set; }
        public string ApproverFullName { get; set; }
        public string FeedBack { get; set; }
        public int ApproverFeedBackStatus { get; set; }
        public string ApproverFeedBackStatusString { get; set; }
        public string FeedBackDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Common.Constants
{
    public static class SettlementStatus
    {
        public static int Active { get { return 1; } }
        public static int InActive { get { return -1; } }
        public static int Pending { get { return 2; } }
        public static int Completed { get { return 100; } }
        public static int CR { get { return -404; } }
        public static int Draft { get { return 5; } }
        public static int Reject { get { return -500; } }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Common.Constants
{
    public static  class FundDisburseStatus
    {
        public static int FundDisbursePending { get { return 2; } }
        public static int FundDisburseSuccessfullyReceived { get { return 100; } }
        public static int FundDisburseRollBack { get { return -404; } }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AMS.Common.Constants
{
    public  static class EstimationType
    {
      
            public static int Factsheet { get { return 8; } }
            public static int PROCUREMENT { get { return 7; } }
            [DisplayName("New Budget")]
            public static int NEW_BUDGET { get { return 2; } }
            public static int MEMO { get { return 3; } }

            public static string GetEstimationTypeByTypeId(int typeId)
            {
                string EstimationTypeName = string.Empty;

                if (typeId == NEW_BUDGET)
                {
                    EstimationTypeName = "New Budget";
                }
                if (typeId == PROCUREMENT)
                {
                    EstimationTypeName = "Procurment";
                }
                if (typeId == Factsheet)
                {
                    EstimationTypeName =  "Factsheet";
                }
                if (typeId == MEMO)
                {
                    EstimationTypeName = "Memo";
                }
                return EstimationTypeName;
            }
        
    }

}

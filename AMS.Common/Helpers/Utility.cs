using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMS.Common.Helpers
{
    public static class Utility
    {
        public  static bool IsEmpty<T>(List<T> list)
        {
            if (list == null)
            {
                return true;
            }

            return !list.Any();
        }



        public static string CurrencyTypeConvertToStringFormat(int currencyType)
        {
            var currency = "";

            if (currencyType < 2)
            {
                currency = " BDT(৳)";
            }else if (currencyType == 2)
            {
                currency = " USD($)";
            }else if (currencyType == 3)
            {
                currency = " EURO(€)";
            }
            return currency;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHtmlGenerate
{
   public class ItemParam
    {
        public string Style { get; set; }
        public List<TdParam> Data { get; set; }
    }

   public class TdParam
   {
       public string Style { get; set; }
       public string Data { get; set; }
   }
    public class HtmlParameter
    {
        public List<string> TableHeader { get; set; }
        public List <ItemParam> items
        {
            get;
            set;

        }

        public string TableStyle { get; set; }
}
}

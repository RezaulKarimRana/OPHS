using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHtmlGenerate
{
    public static class TestHtmlGenerate
    {
      

        public  static string getHtmlMainTable(HtmlParameter htmlParameter)
        {
            string mainTable = "";
            mainTable += @"<table" + @" style=""" + htmlParameter.TableStyle + "\">";
            if (htmlParameter.TableHeader != null)
            {
                mainTable += getHtmlMainTableHeader(htmlParameter);
            }

            mainTable +=@"<tbody>";

            mainTable += getTableRow(htmlParameter);
            mainTable += @"</tbody>";
            mainTable += "</table>";
            return mainTable;



        }

        public static string getHtmlMainTableHeader(HtmlParameter htmlParameter)
        {
            string mainTableHeader = "";
            mainTableHeader += @"<th>";
            foreach (var headerTd in htmlParameter.TableHeader)
            {
                mainTableHeader += "<td>" + headerTd +"<td>";
            }

            mainTableHeader += @"</th>";
            return mainTableHeader;
        }

        public static string getTableRow(HtmlParameter htmlParameter)
        {
            string tableRow = "";
         
            foreach (var row in htmlParameter.items)
            {
                tableRow += @"<tr" + @" style="""  + row.Style + "\">";
                foreach (var td in row.Data)
                {

                    tableRow += @"<td" + @" style=""" + td.Style + "\">" + td.Data + "</td>";


                }
                tableRow += @"</tr>";


            }

            tableRow += @"</th>";
            return tableRow;

        }
    }

}

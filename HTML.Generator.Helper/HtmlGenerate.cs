using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML.Generator.Helper;

namespace HtmlGenerate
{
    public static class HtmlGenerate
    {
        public static string getEmailBody(HtmlTableParameter htmlTableParameter, EmailFooterContent emailFooterContent)
        {
            string emailBodyContent = "";
            emailBodyContent += @"<div align='center' style='background-color:#ffffff'>";
                emailBodyContent += getBodyHeaderContent();
                emailBodyContent += getHtmlTable(htmlTableParameter);
                emailBodyContent += getBodyFooterContent(emailFooterContent);
            emailBodyContent += @"</div>";
            return emailBodyContent;

        }
        public static string getHtmlTable(HtmlTableParameter htmlTableParameter)
        {
            string mainTable = "";
             //mainTable += @"<head><link rel=""stylesheet"" type=""text/css"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" /></head>";
             if (htmlTableParameter.items.Count == 0)
             {
                 mainTable += "<h6 style='margin-left : 20px;'>No Items Avaiable</h6>";
             }
             else
             {
                 if (htmlTableParameter.TableStyle is null)
                 {
                     htmlTableParameter.TableStyle = @" font-family: Arial, Helvetica, sans-serif; border-collapse: collapse; width: 100%; ";
                 }
                 else
                 {
                     htmlTableParameter.TableStyle += @" font-family: Arial, Helvetica, sans-serif; border-collapse: collapse; width: 100%; ";
                }

                 mainTable += @"<table" + " " + htmlTableParameter.ExtraTableProperty + @" style=""" + htmlTableParameter.TableStyle + "\">";
                 if (htmlTableParameter.TableHeader != null)
                 {
                     mainTable += getHtmlMainTableHeader(htmlTableParameter);
                 }

                 mainTable += @"<tbody>";

                 mainTable += getTableRow(htmlTableParameter);
                 mainTable += @"</tbody>";
                 mainTable += "</table>";
            }
            
            return mainTable;
        }

        public static string getBodyFooterContent(EmailFooterContent emailFooterContent)
        {
            string bodyFooterContent = "";
            bodyFooterContent += @"
            <table id='last_email-content' width='100%' align='center' border='0' cellspacing='0' cellpadding='0' style='padding:15px 30px 10px 50px; margin-left: 30px'>
                  <tbody style='text-align: justify'>
                     <tr>
                        <td>
                           <br /><br />
                           <p><strong> "+ emailFooterContent.bestRegards+@"</strong>
                              <br />"+ emailFooterContent.CompanyName+@" 
                              <br />"+ emailFooterContent.CompanyAddress +@"
                           </p>
                           <br /><br />
                        </td>
                     </tr>
                  </tbody>
               </table>";
            bodyFooterContent +=
                @"<table id='email-footer' width='100%' align='center' border='0' cellspacing='0' cellpadding='0'>
                  <tbody>
                     <tr>
                        <td>
                           <table id='email-footer-body' width='100%' align='center' border='0' cellspacing='0' cellpadding='0' style='background-color:#000000'>
                              <tbody>
                                 <tr>
                                    <td id='bottom_bottom-left' width='100%' height='50'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;                            
                                       <span style='padding-left: 20px;color:#ffffff;font-size: small;font-weight:1200;'> " + emailFooterContent.FooterContent + @"</span>
                                    </td>
                                 </tr>
                              </tbody>
                           </table>
                        </td>
                     </tr>
                  </tbody>
                </table>";
            return bodyFooterContent;
        }
        public static string getBodyHeaderContent()
        {
            string bodyHeaderContent = "";
            bodyHeaderContent +=
                @"<table id='email-header' width='100%' align='center' border='0' cellspacing='0' cellpadding='0' style='background-color:#f5f6f7'>
                      <tbody>
                         <tr>
                            <td>
                               <table id='email-header-body' width='100%' align='center' border='0' cellspacing='0'
                                  cellpadding='0'>
                                  <tbody>
                                     <tr>
                                        <td height='5' colspan='2' style='background-color:rgba(2,1,1,0.76)'></td>
                                     </tr>
                                     <tr>
                                        <td id='top-left' width='50%' height='80'
                                           style='text-align:left;padding:0 0 0 30px'>
                                           <img src='http://www.summitcommunications.net/images/logo.png'
                                              alt='Logo' border='0' width='123' height='48' id='logo' class='CToWUd'>
                                        </td>
                                        <td id='top-right' width='50%'
                                           style='text-align:left;padding:0 30px 0 0'>                          
                                        </td>
                                     </tr>
                                  </tbody>
                               </table>
                            </td>
                         </tr>
                      </tbody>
                   </table>";
            return bodyHeaderContent;
        }

        public static string getBodyHeaderMessage(string message)
        {
            string bodyHeaderMessage = "";
            bodyHeaderMessage +=
                @"<table  border='0' cellspacing='0' cellpadding='0'>
                  <tbody>
                     <tr>
                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>
                           <h4 style='text-align: left; padding-left : 20px;'>Dear Concern, </h4>
                           <p style='text-align: left; padding-left : 20px;'> " + message+ @"</p>
                        </td>
                     </tr>
                  </tbody>
               </table>";
            return bodyHeaderMessage;
        }

        public static string getHtmlMainTableHeader(HtmlTableParameter htmlTableParameter)
        {
            string mainTableHeader = "";
            mainTableHeader += @"<thead style='position: sticky; top: 0; display: table-header-group;  ' class='thead-dark'><tr style='padding-top: 12px; padding-bottom: 12px; text-align: left;background-color:#D3D3D3; color: #000000;'>";
            foreach (var headerTd in htmlTableParameter.TableHeader)
            {
                mainTableHeader += "<th scope='col'   style='border: 1px solid #ddd; padding:8px;' >" + headerTd + "</th>";
            }

            mainTableHeader += @"</tr></thead>";
            return mainTableHeader;
        }

        public static string getTableRow(HtmlTableParameter htmlTableParameter)
        {
            string tableRow = "";

            foreach (var row in htmlTableParameter.items)
            {
                if (row.Style is null)
                {
                    row.Style = @"page-break-inside: avoid; ";
                }
                else
                {
                    row.Style += @" page-break-inside: avoid; ";
                }
                tableRow += @"<tr" + " " + row.ExtraProperty  + @" style=""" + row.Style + "\">";
                foreach (var td in row.Data)
                {
                    if (td.Style is null)
                    {
                        td.Style = " border: 1px solid #ddd; padding:8px; vertical-align:top; ";
                    }
                    else
                    {
                        td.Style += " border: 1px solid #ddd; padding:8px; vertical-align:top; ";

                    }
                    tableRow += @"<td" + " "+ td.ExtraColumnProperty + @" style=""" + td.Style + "\">" + td.Data + "</td>";
                }

                tableRow += @"</tr>";
            }

            tableRow += @"";
            return tableRow;
        }


        public static string bootstrapCardContent(string headerName, string cardInnerContent)
        {
            
            string content = "";
           // content += @"<head><link rel=""stylesheet"" type=""text/css"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" /></head>";
            content += @" <div class=""card border-secondary mb-4"" > ";
            content += @"<div class=""card-header"" ><h4 style='margin-left : 25px;'><b>" + headerName +@"</b></h4> </ div>" ; 
            content += @"<div class=""card-body"" > <div class=""container-fluid"" > ";
            content += @"<div class=""table-responsive large-table-container-3"" > <div style ='padding : 10px;'> " + cardInnerContent +" ";
            content += @"</div> </div> </div> </div> </div>";

            return content;


        }

        public static string divWrappper(string data, string style, string extraProperty)
        {
            return @"<div "+ extraProperty + @"style = """ + style + "\">" + data + "</div>";
        }

        public static string divWrappperForTableData(string header  , string innderData)
        {
            return @"<div style='border: 1px solid gray; padding: 3px; margin-top : 10px ; border-radius: 15px; '>
                     <div style='margin: 0px; padding: 12px; border-bottom :1px solid black; background : #D3D3D3;'>
                        <h2 style='padding-left : 10px; color: black; '>" + header +@" </h2> 
                      </div>
                    <div>
                    "+ innderData + @"
                    </div>
            </div>";
        }
    }
}
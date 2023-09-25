namespace Common.Helpers
{
    public static class PdfReportHelper
    {
        public static string GenericPdfHtmlBody(string upperBodyPart, string estimateType, string Identification, string subject, string objective, string details, string fromTime, string toTime, string itemDetailsBodyPart,
            string totalPrice, string departmentSummaryPart, string particularSummaryPart, string approverListPart, string approverFeedBacks, string procurementApprovalPart)
        {
            string secondPart = "";
            string forthPart = "";
            #region EmailFirstPart
            string firstPart =
                @" <style>  tr {
         page-break-inside: avoid;
      }
</style>
<div align='center' style='font-family:Calibri; font-size:100%;  overflow:hidden; page-break-after: always;  ' width='1000px'>
                    <table id='email-header' width='100%' align='center' border='0' cellspacing='0' cellpadding='0' style=''>
                        <tbody>
                        <tr>
                            <td>
                                <!-- SComm Logo-->
					            <table id='email-header-body' width='100%' align='center' border='0' cellspacing='0' cellpadding='0'>
						            <tbody>
							        <tr>
								        <td height='5' colspan='2' style='background-color:rgba(2,1,1,0.76)'></td>
							        </tr>
							        </tbody>
						        </table>
                            </td>
				        </tr>
                        <tbody>
                    </table>
                    <table id='email-content' width='100%' align='center' border='0' cellspacing='0' cellpadding='0' style='padding:5px 30px 0px 30px'>
                        <tbody style='text-align: justify'>
                        <!-- Email TOP-->
                       
                        
                        <tr>
                            <td style='padding-bottom:30px 0px 0px 0px; width: 100%; background-color:#ededed;'>
                                <!--Estimation Information Table-->
                                <table align='left' border='0' cellspacing='0' cellpadding='0' style='border-spacing:10px;'>
                                    <tbody>
                                    <tr>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px; width: 20%;'><b>Approval For</b></td>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + estimateType + @"</td>
                                    </tr>
                                    <tr>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;'><b>Identification</b></td>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'><mark>" + Identification + @"</mark></td>
                                    </tr>
                                    <tr>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Subject</b></td>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + subject + @"</td>
                                    </tr>" + procurementApprovalPart + @"
                                    <tr>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Objective</b></td>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + objective + @"</td>
                                    </tr>
                                    <tr>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Details</b></td>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>" + details + @"</td>
                                    </tr>
                                    <tr>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;'><b>Time Period</b></td>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>From: <strong>" + fromTime + @"</strong> To: <strong>" + toTime + @"</strong></td>
                                    </tr>
                                    <tr>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px;padding-right:20px;' valign='top'><b>Total Price</b></td>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'><b>" + totalPrice + @"</b></td>
                                    </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>";
            #endregion

            #region EmailSecondPartItemDetailAndSummary
            if (!string.IsNullOrEmpty(itemDetailsBodyPart))
            {
                secondPart = itemDetailsBodyPart +
                        @"<tr>
                            <td style='width:100%;padding:5px'>
                                <table align='center' border='0' cellspacing='0' cellpadding='0' style='width:100%'>
                                    <tbody>
                                    <tr>
                                        <td style='background-color:rgba(0,0,0,0);'><br><br></td>
                                    </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                        <tr>        
                            <td style='padding:5px; background-color:#e1e6de; width:100%;font-family:Calibri; font-size:100%;' align='center'>
                                <b>Budget Summary</b>
                            </td>
                        </tr>
                        <tr>
                            <!--Summary Tables-->        
                            <td style='padding-bottom:30px 0px 0px 0px; width: 100%; background-color:#ededed;'>
                                <table align='center' border='0' cellspacing='0' cellpadding='0' style='width:100%'>
                                    <tr>
                                        <td style='width:40%;'>
                                            <table align='center' border='0' cellspacing='0' cellpadding='0' style='width:100%'>
                                                <tr>
                                                    <td style='background-color:#d4dcd0;' align='center' colspan='2'>
                                                        <b>Department Wise Summary</b>
                                                        <hr>
                                                    </td>
                                                </tr>
                                                <tr align='center'>
                                                    <td><b><u>Department</u></b></td>
                                                    <td><b><u>Total Price(TK.)</u></b></td>
                                                </tr>"
                                                   + departmentSummaryPart +
                                            @"</table>
                                        </td>
                                        <td style='width=20%;'>

                                        </td>
                                        <td style='width=40%;'>
                                            <table align='center' border='0' cellspacing='0' cellpadding='0' style='width:100%'>
                                                <tr>
                                                    <td style='background-color:#d4dcd0;' align='center' colspan='2'>
                                                        <b>Particular Wise Summary</b>
                                                        <hr>
                                                    </td>
                                                </tr>
                                                <tr align='center'>
                                                    <td><b><u>Particular</u></b></td>
                                                    <td><b><u>Total Price(TK.)</u></b></td>
                                                </tr>"
                                                   + particularSummaryPart +
                                            @"</table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style='width:100%;padding:5px'>
                                <table align='center' border='0' cellspacing='0' cellpadding='0' style='width:100%'>
                                    <tbody>
                                    <tr><td colspan='16' style='background-color:rgba(0,0,0,0);'><br><br></td></tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>";
            }
            #endregion


            #region EmailThirdPartApprover
            string thirdPart = @"<tr>        
                            <td style='padding:5px; background-color:#d4dcd0; width:100%;font-family:Calibri; font-size:100%;' align='center'>
                                <b>Approver List</b>
                                <hr>
                            </td>
                        </tr>
                        <tr>
                            <td style='padding-bottom:30px 0px 0px 0px; width: 100%; background-color:#ededed;'>
                                <table align='center' border='0' cellspacing='0' cellpadding='0' style='width:100%'>
                                    <tr align='center'>
                                        <td><b><u>Approver Name</u></b></td>
                                        <td><b><u>Priority</u></b></td>
                                        <td><b><u>Department</u></b></td>
                                        <td><b><u>Expected Time</u></b></td>
                                    </tr>"
                                    + approverListPart +
                                @"</table>
                            </td>
                        </tr>";
            #endregion

            #region EmailApproverFeedback
            if (!string.IsNullOrEmpty(approverFeedBacks))
            {
                forthPart =
                        @"<tr>
                            <td style='width:100%;padding:5px'>
                                <table align='center' border='0' cellspacing='0' cellpadding='0' style='width:100%'>
                                <tbody>
                                    <tr><td style='background-color:rgba(0,0,0,0);'><br><br></td></tr>
                                </tbody>
                                </table>
                            </td>
                        </tr>
                        <tr>        
                            <td style='padding:5px; background-color:#d4dcd0; width:100%;font-family:Calibri; font-size:100%;' align='center'>
                                <b>Approver Feedbacks</b><hr>
                            </td>
                        </tr>
                        <tr>
                            <td style='padding-bottom:30px 0px 0px 0px; width: 100%; background-color:#ededed;'>
                                <table align='center' border='0' cellspacing='0' cellpadding='0' style='width:100%'>
                                    <tr align='center'>
                                        <td><b><u>Approver Name</u></b></td>
                                        <td><b><u>Status</u></b></td>
                                        <td><b><u>Feedback Date</u></b></td>
                                        <td><b><u>Feedback</u></b></td>
                                    </tr>"
                                        + approverFeedBacks +
                                @"</table>
                            </td>
                        </tr>";
            }

            #endregion

            #region EmailLastPart
            string lastPart =
                        @"</tbody>
                    </table>
                    <br/>
                     <br/>  
                     <table align='left' border='0' cellspacing='0' cellpadding='0'>
                                    <tbody>
                                    <tr>
                                        <td style='color:rgba(31,6,4,0.96);padding-bottom:15px'>
                                            "
                                    + upperBodyPart +
                                    @"<p></p>
                                    </tr>
                                    </tbody>
                                </table>
                    
                </div>";
            #endregion

            string wholeBody = firstPart + secondPart + thirdPart + forthPart + lastPart;

            return wholeBody;
        }
    }
}

$('.autoSuggestionSelect').css('width', '100%');
$(".autoSuggestionSelect").select2({});

var rowIdx = 0;
var storeUsersWithDepartmentName;
var formData;
var filelist = [];
var approverPriorityIndex = 999;

$('#justificationId').summernote({
    placeholder: 'Enter Memo Remarks',
    tabsize: 2,
    height: 300,
    toolbar: [
        ['style', ['style']],
        ['font', ['bold', 'italic', 'underline', 'strikethrough', 'superscript', 'subscript', 'clear']],
        ['fontname', ['fontname']],
        ['fontsize', ['fontsize']],
        ['color', ['color']],
        ['para', ['ol', 'ul', 'paragraph', 'height']],
        ['table', ['table']],
        ['insert', ['link', 'picture', 'video']],
        ['view', ['undo', 'redo', 'fullscreen', 'codeview', 'help']]
    ]
});
$("#viewEstimationDetails").click(function () {
    var id = $("#estimateIdProtected").val();
    var url = "/BudgetEstimation/ViewEstimation?id=" + id;
    window.open(url, '_blank');
});
var uploadField = document.getElementById("estimationfiles");
uploadField.onchange = function () {
    if (this.files[0].size > 10000000) {
        alertify.alert("File is too big!");
        this.value = "";
    };
};

$("input[name=attachedFiles]").change(function () {
    var files = $(this).get(0).files;
    for (var x = 0; x < files.length; x++) {
        filelist.push(files[x]);
        $("#attachmentsTableNew").append('<li id="liAttach' + x + '"><a href="#">' + files[x].name + '</a><span id="' + x + '" class="removeAttachLocal" style="font-size:15px;color: red;">&#10060;</span></li>');
    }
});
$("body").on('click', ".removeAttachLocal", function () {
    var index = this.id;
    filelist[index] = {};
    $(this).parent().remove();
});
function loadDepartmentWiseRunningSummary(value) {
    var formatter = new Intl.NumberFormat('en-IN', {
        style: 'currency',
        currency: 'BDT'
    });

    $.ajax({
        type: "GET",
        url: "/Settlement/LoadDepartmentWiseSummaryForASettledEstimationWithBudgetData?estimationId=" + value,
        data: "{}",
        success: function (data) {
            var deptSummary = data;

            var totalAllowedBudget = 0;
            var totalBudgetCost = 0;
            var totalDeviation = 0;
            var totalParcentage = 0;

            let rowDepartmentWiseSummaryTableIndex = 1;
            for (var i = 0; i < deptSummary.length; i++) {
                var deviation = 0;
                var percentage = 0;
                totalAllowedBudget += deptSummary[i].totalAllowableBudget;
                totalBudgetCost += deptSummary[i].totalCost;
                deviation = parseFloat(deptSummary[i].totalCost) - parseFloat(deptSummary[i].totalAllowableBudget);
                totalDeviation += deviation;
                var divide = 0;
                if (deptSummary[i].totalAllowableBudget > 0) {
                    divide = parseFloat(deptSummary[i].totalCost) / parseFloat(deptSummary[i].totalAllowableBudget);
                }
                percentage = parseFloat(divide) * 100;
                totalParcentage += percentage;

                $('#tDeptWiseRunningSummary').append(
                    `<tr class="deletedDepartmentRow" id="RowDepartmentSummary${rowDepartmentWiseSummaryTableIndex}">
                                                                        <td lass="text-left">
                                                                            <span id="nameDepartmentId${rowDepartmentWiseSummaryTableIndex}"
                                                                                class="departmentWiserSummaryDepartmentColumn"
                                                                                style="color:chocolate">` + deptSummary[i].departmentName + `</span>
                                                                        </td>
                                                                        <td id="idDepartmentId${rowDepartmentWiseSummaryTableIndex}" class="departmentSummaryDeptId" hidden>` + deptSummary[i].departmentId + `</td>
                                                                        <td class="text-right">
                                                                            <span id="totalPriceDepartmentId${rowDepartmentWiseSummaryTableIndex}"
                                                                                class="departmentWiserSummaryTablePriceColumn" hidden>` + deptSummary[i].totalAllowableBudget + `</span>
                                                                            <span style="color:chocolate">` + ((deptSummary[i].totalAllowableBudget > 0) ? formatter.format(deptSummary[i].totalAllowableBudget) : 'N/A') + `</span>
                                                                        </td>
                                                                        <td class="text-right">
                                                                            <span id="totalcostDepartmentId${rowDepartmentWiseSummaryTableIndex}"
                                                                                class="departmentWiserSummaryTablePriceColumn" hidden>` + deptSummary[i].totalCost + `</span>
                                                                            <span style="color:chocolate">` + formatter.format(deptSummary[i].totalCost) + `</span>
                                                                        </td>
                                                                        <td class="text-right">
                                                                            <span style="color:chocolate">` + formatter.format(deviation) + `</span>
                                                                        </td>
                                                                        <td class="text-right">
                                                                            <span style="color:chocolate">` + ((deptSummary[i].totalAllowableBudget > 0) ? percentage.toFixed(2) : 'N/A') + ` % </span>
                                                                        </td>
                                                                    </tr>`
                );
                rowDepartmentWiseSummaryTableIndex++
            }
            totalParcentage = (parseFloat(totalBudgetCost) / parseFloat(totalAllowedBudget)) * 100;
            $('#tDeptWiseRunningSummary').append(
                `<tr class="deletedDepartmentRow" id="RowDepartmentSummary${rowDepartmentWiseSummaryTableIndex}">
                                                                    <td lass="text-left"><span style="color: #23098B ">Grand Total(<span style="color: blue ">Exclude SCM & Regulatory Affairs</span>)</span></td>
                                                                    <td class="text-right">
                                                                        <span style="color:#23098B">` + (totalAllowedBudget ? formatter.format(totalAllowedBudget) : 'N/A') + `</span>
                                                                    </td>
                                                                    <td class="text-right">
                                                                        <span style="color:#23098B">` + formatter.format(totalBudgetCost) + `</span>
                                                                    </td>
                                                                    <td class="text-right">
                                                                        <span style="color:#23098B">` + formatter.format(totalDeviation) + `</span>
                                                                    </td>
                                                                    <td class="text-right">
                                                                        <span style="color:#23098B">` + (totalParcentage.toFixed(2)) + ` % </span>
                                                                    </td>
                                                                </tr>`
            );
            rowDepartmentWiseSummaryTableIndex++
        }
    });
}
function loadUserList() {
    $.ajax({
        type: "GET",
        url: "/User/GetAllUsersWithDepartmentName",
        data: "{}",
        success: function (data) {
            var users = data.users;
            storeUsersWithDepartmentName = users;

            for (var i = 0; i < users.length; i++) {
                var o = new Option(users[i].fullName, users[i].userID);
                $(o).html(users[i].fullName + "/ " + users[i].departmentName);
                $("#employeeListforApprover").append(o);
            }

            $('#employeeListforApprover option[value="16"]').prop('disabled', true);
            $('#employeeListforApprover option[value="20"]').prop('disabled', true);
        }
    });
}
function formateCurrency() {
    var formatter = new Intl.NumberFormat('en-IN', {
        style: 'currency',
        currency: 'BDT'
    });

    var totalPriceWithoutItemTable = $('#totalPriceText').val();
    var formatedtotalPriceWithoutItemTable = formatter.format(totalPriceWithoutItemTable);
    $('#totalPriceText').val(formatedtotalPriceWithoutItemTable);

    var totalPrice = $('#sumOfTotalPrice').text();
    var formatedtotalPrice = formatter.format(totalPrice);
    $('#sumOfTotalPriceShow').text("Allowed Budget " + formatedtotalPrice);

    var totalcost = $('#sumOfTotalCost').text();
    var formatedtotalcost = formatter.format(totalcost);
    $('#sumOfTotalCostShow').text("Settled Cost " + formatedtotalcost);

    var totalBudgetPrice = $('#budget').text();
    var formatedtotalBudgetPrice = formatter.format(totalBudgetPrice);
    $('#strbudget').text(formatedtotalBudgetPrice);

    var totalAllowedPrice = $('#allowable').text();
    var formatedtotalAllowedPrice = formatter.format(totalAllowedPrice);
    $('#strallowable').text(formatedtotalAllowedPrice);

    var totalCost = $('#cost').text();
    var formatedtotalCost = formatter.format(totalCost);
    $('#strcost').text(formatedtotalCost);

    var totalDeviation = $('#deviation').text();
    var formatedtotalDeviation = formatter.format(totalDeviation);
    $('#strdeviation').text(formatedtotalDeviation);
}

$("#employeeListforApprover").change(function () {
    showErrorMessageBelowCtrl('employeeListforApprover', 'please add employee', false);
    var selectorID = $("#employeeListforApprover").val();
    if (selectorID == 16) {
        var o = new Option("1", 1);
        $(o).html(1);
        $("#priorityforApprover").append(o);
        $("#priorityforApprover").val(1);
        $("#priorityforApprover").prop("disabled", true);
    }
    else {
        $("#priorityforApprover").prop("disabled", false);
        $("#priorityforApprover option[value='1']").remove();
    }

    for (var x = 0; x < storeUsersWithDepartmentName.length; x++) {
        if (storeUsersWithDepartmentName[x].userID == selectorID) {
            $("#userDepartmentID").val(storeUsersWithDepartmentName[x].departmentName);
            $("#DeptIdUserID").val(storeUsersWithDepartmentName[x].departmentId);
            break;
        }
    }
});

$("#addApproverForm").change(function () {
    showErrorMessageBelowCtrl('priorityforApprover', 'please add priority', false);
    showErrorMessageBelowCtrl('employeeListforApprover', 'please add employee', false);
    showErrorMessageBelowCtrl('employeeListforApprover', 'please select MD Sir For Approval', false);

});

//add row in approver list
var rowApproverIdx = 0;
$('#addApprover').on('click', function () {
    showErrorMessageBelowCtrl('app_err-container', 'please add alteast one approver', false);

    var selectedsequenORparallleVal = "";
    var selectedArrproverTyepVal = "";

    var selectedsequenORparallle = $("#sequenORparalllerRadioDiv input[type='radio']:checked");
    var selectedArrproverTyep = $("#approverTypeRadioDiv input[type='radio']:checked");
    if (selectedsequenORparallle.length > 0 && selectedArrproverTyep.length > 0) {

        selectedsequenORparallleVal = selectedsequenORparallle.val();
        console.log(selectedsequenORparallleVal);

        selectedArrproverTyepVal = selectedArrproverTyep.val();
        console.log(selectedArrproverTyepVal);

        if (selectedsequenORparallleVal == "Sequencial" && selectedArrproverTyepVal == "2") {
            approverPriorityIndex--;
            addApproverinApproverList(approverPriorityIndex, selectedArrproverTyepVal, "Recomendor");
        }
        else if (selectedsequenORparallleVal == "Parallel" && selectedArrproverTyepVal == "2") {
            addApproverinApproverList(approverPriorityIndex, selectedArrproverTyepVal, "Recomendor");
        }
        else if (selectedArrproverTyepVal == "1") {
            addApproverinApproverList(1, selectedArrproverTyepVal, "Final Approver");
        }
        else if (selectedArrproverTyepVal == "3") {
            addApproverinApproverList(400, selectedArrproverTyepVal, "Informed");
        }
    }

});

function addApproverinApproverList(priorityParameter, priorityType, priorityTypeTxt) {
    let checkFlag = false;

    var priority = priorityParameter;
    var username = $('#employeeListforApprover').find(":selected").text();
    var userId = $('#employeeListforApprover').find(":selected").val();
    var userDepartment = $('#userDepartmentID').val();
    var DeptIdUserID = $('#DeptIdUserID').val();

    var priorityValue = $('#priorityforApprover').find(":selected").val();
    var usernameValue = $('#employeeListforApprover').find(":selected").val();


    if (priorityValue <= 0) { showErrorMessageBelowCtrl('priorityforApprover', 'please add priority', true); return; } else { showErrorMessageBelowCtrl('priorityforApprover', 'please select priority', false); }
    if (usernameValue <= 0) {
        showErrorMessageBelowCtrl('employeeListforApprover', 'please add employee', true);
        return;
    } else {
        showErrorMessageBelowCtrl('employeeListforApprover', 'please select employee', false);
    }

    $("#TableAprroverList tr").each(function (a, b) {
        if (a > 0) {
            var userIdFromtb = $('.approverId', b).text();
            var priorityFromtb = $('.approverPriority', b).text()
            if (priorityFromtb == 1 && priority == 1) {
                showErrorMessageBelowCtrl('employeeListforApprover', 'Already added the approved.', true);
                checkFlag = true;
                approverPriorityIndex++;
            }
            if (userIdFromtb == userId) {
                checkFlag = true;
                console.log("Already Added scl");
                showErrorMessageBelowCtrl('employeeListforApprover', 'Already this user exits in the approve list.', true);

            }

            if (userIdFromtb == userId && priority == priorityFromtb) {
                checkFlag = true;
                console.log("Already Added");
                showErrorMessageBelowCtrl('priorityforApprover', 'Already added this approve with same priority', true);
                approverPriorityIndex++;
            }
        }
    });
    if (checkFlag == false) {
        $('#tApproverbody').append(
            `<tr id="RowApprover${++rowApproverIdx}">
                                                            <td>&nbsp;</td>
                                                            <td id="nameApproverId${rowApproverIdx}" class="approverName">` + username + `</td>
                                                            <td id="idApproverId${rowApproverIdx}" class="approverId" hidden>` + userId + `</td>
                                                            <td id="priorityApproverId${rowApproverIdx}" class="approverPriority" hidden>` + priority + `</td>
                                                            <td id="priorityApproverTypeId${rowApproverIdx}" class="approverPriorityType" hidden>` + priorityType + `</td>
                                                            <td id="priorityApproverTypeTextId${rowApproverIdx}" class="approverPriorityTypeText">` + priorityTypeTxt + `</td>
                                                            <td id="departmentApproverId${rowApproverIdx}" class="approverDepartment" hidden>` + userDepartment + `</td>
                                                            <td id="departmentIdApproverId${rowApproverIdx}" class="approverDepartmentId" hidden>` + DeptIdUserID + `</td>
                                                            <td class="text-center">Due</td>
                                                            <td class="text-center">
                                                                <button class="btn btn-danger remove" type="button"><i class="fa fa-trash" aria-hidden="true"></i></button>
                                                            </td>
                                                        </tr>`
        );
    }
}
$('#tApproverbody').on('click', '.remove', function () {
    $(this).closest('tr').remove();
    rowApproverIdx--;
});
$("#submitBudgeMemo").click(function () {
    var result = validationCheckForSubmit();
    if (result == false) {
        $('#submitBudgeMemo').prop('disabled', false);
        return;
    }
    alertify.confirm('Confirm Submit', 'Do you want to initiate this Memo?', function () {
        $('#submitBudgeMemo').attr('disabled', 'disabled');
        var data = ConstructResponseObject();
        if (data == false)
            return;

        var estimationAttachment = [];

        formData = new FormData();
        for (var i = 0; i < filelist.length; i++) {
            if (jQuery.isEmptyObject(filelist[i])) {
                console.log("skip");
                continue;
            }
            formData.append(filelist[i].name, filelist[i]);
        }
        estimationAttachment = formData;

        postDataForBudgetEstimation(data, estimationAttachment);
    }
        , function () { alertify.error('Cancel') });
});

$("#justificationId").keypress(function () {
    showErrorMessageBelowCtrl('justificationId', 'please add Justification', false);
});
function autoApproverAddorRemove() {
    document.getElementById("tApproverbody").innerHTML = "";
    rowApproverIdx = 0;
    $('#finalApproverBtn').show();
    $('#employeeListforApprover option[value="16"]').prop('disabled', false);
    $('#employeeListforApprover option[value="20"]').prop('disabled', false);
    $('#employeeListforApprover option[value="16"]').prop('disabled', true);
    $('#employeeListforApprover option[value="20"]').prop('disabled', true);

    $('#finalApproverBtn').hide();
    $('#tApproverbody').append(
        `<tr id="RowApprover${++rowApproverIdx}">
            <td>&nbsp;</td>
            <td id="nameApproverId${rowApproverIdx}" class="approverName"> Md Sayed Nazmul Hasan / Business Controll</td>
            <td id="idApproverId${rowApproverIdx}" class="approverId" hidden> 20 </td>
            <td id="priorityApproverId${rowApproverIdx}" class="approverPriority" hidden>2</td>
            <td id="priorityApproverTypeId${rowApproverIdx}" class="approverPriorityType" hidden>2</td>
            <td id="priorityApproverTypeTextId${rowApproverIdx}" class="approverPriorityTypeText">Recomendor</td>
            <td id="departmentApproverId${rowApproverIdx}" class="approverDepartment" hidden>Business Controll</td>
            <td id="departmentIdApproverId${rowApproverIdx}" class="approverDepartmentId" hidden>5</td>
            <td class="text-center">Due</td>
        </tr>`
    );

    $('#tApproverbody').append(
        `<tr id="RowApprover${++rowApproverIdx}">
            <td>&nbsp;</td>
            <td id="nameApproverId${rowApproverIdx}" class="approverName"> Md Arif Al Islam / MD's Office</td>
            <td id="idApproverId${rowApproverIdx}" class="approverId" hidden> 16 </td>
            <td id="priorityApproverId${rowApproverIdx}" class="approverPriority" hidden>1</td>
            <td id="priorityApproverTypeId${rowApproverIdx}" class="approverPriorityType" hidden>1</td>
            <td id="priorityApproverTypeTextId${rowApproverIdx}" class="approverPriorityTypeText">Final Approver</td>
            <td id="departmentApproverId${rowApproverIdx}" class="approverDepartment" hidden>MD's Office</td>
            <td id="departmentIdApproverId${rowApproverIdx}" class="approverDepartmentId" hidden>23</td>
            <td class="text-center">Due</td>
        </tr>`
    );
}
function validationCheckForSubmit() {

    var response = true;

    var justification = $("#justificationId").val();

    if (!justification || justification == '') {
        showErrorMessageBelowCtrl('justificationId', 'please add Justification', true);
        response = false;
        $('html, body').animate({
            scrollTop: $("#justificationId").offset().top
        }, 800);
    }
    else
        showErrorMessageBelowCtrl('justificationId', 'please add Justification', false);

    let checkFlagMD = false;
    var approverDeptList = [];
    $("#TableAprroverList tr").each(function (a, b) {
        var priorityFromtb = $('.approverPriority', b).text()
        approverDeptList.push($('.approverDepartmentId', b).text());
        if (priorityFromtb == 1) {
            checkFlagMD = true;
        }
    });

    var tableApproverRowCount = $('#TableAprroverList tr').length;
    if (tableApproverRowCount <= 1) {
        showErrorMessageBelowCtrl('app_err-container', 'please add alteast one approver', true);
        response = false;
        $('html, body').animate({
            scrollTop: $("#app_err-container").offset().top
        }, 800);
    }
    else {
        let recomendorAdded = 0;
        let finalAcknowledgerAdded = 0;

        $("#TableAprroverList tr").each(function (a, b) {
            var role_piority = $('.approverPriorityType', b).text();

            if (Number(role_piority) == 1) {
                finalAcknowledgerAdded = 1;
            }

            if (Number(role_piority) == 2) {
                recomendorAdded = 1;
            }

        });

        if (finalAcknowledgerAdded == 0) {
            showErrorMessageBelowCtrl('employeeListforApprover', 'please add atlest one Final Approver', true);
            validFlag = 0;
            response = false;
        }
        if (recomendorAdded == 0) {
            showErrorMessageBelowCtrl('employeeListforApprover', 'please add atlest one  Recomendor', true);
            validFlag = 0;
            response = false;
        }
        showErrorMessageBelowCtrl('app_err-container', 'please add alteast one approver', false);
    }
    return response;
}

function ConstructResponseObject() {
    var validFlag = 1;
    var estimateMemo = {};
    var estimateApproverList = [];

    $("#TableAprroverList tr").each(function (a, b) {
        if (a > 0 && !isNaN(parseInt($('.approverId', b).text()))) {
            estimateApproverList.push({
                user_Id: $('.approverId', b).text(),
                approverFullName: $('.approverName', b).text(),
                priority: $('.approverPriority', b).text(),
                rolePriority_Id: $('.approverPriorityType', b).text(),
                approverRole: $('.approverPriorityTypeText', b).text(),
                approverDepartment: $('.approverDepartment', b).text()
            });
        }
    });

    var justification = $("#justificationId").val();
    var deviation = $("#deviation").text();
    var estimattionId = $("#estimateId").val();
    var estimateReferenceId = $("#estimateReferId").val();

    estimateMemo = {
        estimateId: estimattionId,
        estimateReferId: estimateReferenceId,
        totalDeviation: deviation,
        justificaitonText: justification
    };

    var data = new RequestObject(estimateMemo, estimateApproverList);
    if (validFlag == 1) {
        return data;
    }
    else {
        return false;
    }
}

function RequestObject(estimateMemo, estimateApproverList) {
    this.estimateMemo = estimateMemo;
    this.estimateApproverList = estimateApproverList;
}
function postDataForBudgetEstimation(data, estimationMemoAttachment) {
    var dataJson = JSON.stringify(data);
    var postData = {
        "requestDto": dataJson
    };
    console.log(postData);

    $.ajax({
        type: "POST",
        traditional: true,
        async: false,
        cache: false,
        url: "/BudgetMemo/PostEstimationMemo",
        context: document.body,
        data: postData,
        beforeSend: function () {
            $.blockUI({
                timeout: 0,
                message: 'Processing..'
            });
        },
        success: function (response) {
            if (parseInt(response) > 0) {
                postEstimateAttachment(parseInt(response), estimationMemoAttachment);
                $.unblockUI();
                alertify.alert('SUCCESS', 'Saved Successfully!', function () { window.location = "/BudgetMemo/MyList"; });
            }
            else {
                alertify.alert('Something went wrong! please try again.');
            }
        },
        complete: function () {
            $.unblockUI();
        },
        failure: function (response) {
            $.unblockUI();
            alert(response.responseText);
        },
        error: function (response) {
            $.unblockUI();
            alert(response.responseText);
        }
    });
}
function postEstimateAttachment(estimationMemoId, data) {
    $.ajax({
        type: "POST",
        url: "/BudgetMemo/UploadFilesOfMemo?estimationMemoId=" + estimationMemoId,
        contentType: false,
        processData: false,
        data: data,
        async: false,
        beforeSend: function () {
            $.blockUI({
                timeout: 0,
                message: 'Processing..'
            });
        },
        success: function (message) {
            if (parseInt(message) == 1) {
                return;
            }
            else {
                alertify.alert('Something went wrong to save the file.');
            }
        },
        error: function () {
            alert("Error!");
        }
    });
}

function loadEstimateApprovers(memoId) {
    $.ajax({
        type: "GET",
        url: "/BudgetMemo/LoadAllApproverByMemo?memoId=" + memoId,
        data: "{}",
        success: function (data) {
            var approvers = data;
            console.log(approvers);

            var statusTag = "";
            for (var i = 0; i < approvers.length; i++) {
                if (approvers[i].approverRoleId == 1 || approvers[i].approverRoleId == 2) {
                    if (approvers[i].approverStatus == "2") {
                        statusTag = "<span style='padding:2px 10px 2px 10px;border-radius: 25px;border: 2px solid royalblue;background-color:royalblue;color:white'>Pending</span>";
                    }
                    else if (approvers[i].approverStatus == "100") {
                        statusTag = "<span style='padding:2px 10px 2px 10px;border-radius: 25px;border: 2px solid green;background-color:green;color:white'>Approved</span>";
                    }
                    else if (approvers[i].approverStatus == "-404") {
                        statusTag = "<span style='padding:2px 10px 2px 10px;border-radius: 25px;border: 2px solid orangered;background-color:orangered;color:white'>Rollbacked</span>";
                    }
                    else if (approvers[i].approverStatus == "-500") {
                        statusTag = "<span style='padding:2px 10px 2px 10px;border-radius: 25px;border: 2px solid red;background-color:red;color:white'>Rejected</span>";
                    }
                }
                $('#tApproverListbody').append(
                    `<tr>
                                    <td>&nbsp;</td>
                                    <td class="approverName">` + approvers[i].approverFullName + ` / ` + approvers[i].approverDepartment + ` </td>
                                    <td class="approverId" hidden>` + approvers[i].approverId + `</td>
                                    <td class="approverPriority" hidden>` + approvers[i].approverPriority + `</td>
                                    <td class="approverPriorityType" hidden>` + approvers[i].approverRoleId + `</td>
                                    <td class="approverPriorityTypeText">` + approvers[i].approverRoleName + `</td>
                                    <td class="approverDepartment" hidden>` + approvers[i].approverDepartment + `</td>
                                    <td class="approverDepartmentId" hidden>` + approvers[i].approverDepartmentId + `</td>
                                    <td class="status">`+ statusTag + `</td>
                                </tr>`);
            }
        }
    });
}
function loadEstimateApproverRemarkList(memoId) {
    $.ajax({
        type: "GET",
        url: "/BudgetMemo/LoadApproverFeedBackByMemo?memoId=" + memoId,
        data: "{}",
        success: function (data) {
            var approvers = data;
            if (approvers < 1) {
                $("#divApproverRemarksList").hide();
            }
            for (var i = 0; i < approvers.length; i++) {
                var d = new Date(approvers[i].feedBackDate);
                if (approvers[i].approverFeedBackStatus == "-500") {
                    statusTag = "<span style='padding:2px 10px 2px 10px;border-radius: 25px;border: 2px solid red;background-color:red;color:white'>Rejected</span>";
                }
                else if (approvers[i].approverFeedBackStatus == "100") {
                    statusTag = "<span style='padding:2px 10px 2px 10px;border-radius: 25px;border: 2px solid green;background-color:green;color:white'>Approved</span>";
                }
                else if (approvers[i].approverFeedBackStatus == "-404") {
                    statusTag = "<span style='padding:2px 10px 2px 10px;border-radius: 25px;border: 2px solid orangered;background-color:orangered;color:white'>Rollbacked</span>";
                }
                if (approvers[i].feedBack == null)
                    approvers[i].feedBack = 'N/A';
                $('#tApproverRemarksbody').append(
                    `<tr>
                        <td>&nbsp;</td>
                        <td class="approverNameFromFeedBack">` + approvers[i].approverFullName + ` </td>
                        <td>`+ statusTag + `</td>
                        <td class="approverApprovalDate">`+ d.toDateString() + ` ` + d.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true }) + `</td>
                        <td class="approverFeedbackFromFeedBack">` + approvers[i].feedBack + `</td>
                    </tr>`);
            }
        }
    });
}
$('#approved_remarks').bind('input propertychange',
    function () {
        showErrorMessageBelowCtrl('approved_remarks', 'please input less then 250 character', false);
        if (this.value.length > 256) {
            showErrorMessageBelowCtrl('approved_remarks', 'please input less then 250 character', true);
            $("#approved_remarks").val($("#approved_remarks").val().substring(0, 256));
        }
    });
$("#forwardToMdSirBtn").click(function () {

    alertify.confirm('Confirm Approve', 'Do you want to Forward this Memo to MD Sir?', function () {
        $('#forwardToMdSirBtn').attr('disabled', 'disabled');

        var memoId = $('#estimateMemoId').val();
        var remarks = $('#approved_remarks').val();

        var requestData = {
            memoId: memoId,
            feedback: '100',
            remarks: remarks,
            isFinalApproved: false
        }

        var dataJson = JSON.stringify(requestData);
        var postData = {
            "requestDto": dataJson
        };
        $.ajax({
            url: "/BudgetMemo/SaveMemoApproval",
            type: "POST",
            data: postData,
            beforeSend: function () {
                $.blockUI({
                    timeout: 0,
                    message: 'Processing..'
                });
            },
            success: function (response) {
                if (parseInt(response) > 0) {
                    $.unblockUI();
                    alertify.alert('SUCCESS', 'Successfully Approved Memo!', function () { window.location = "/BudgetMemo/MyApprovals"; });
                }
                else {
                    alertify.alert(response, function () { window.location = "/BudgetMemo/MyApprovals"; });
                }
            },
            complete: function () {
                $.unblockUI();
            },
            error: function (data) {
                $.unblockUI();
            }
        });
    }
        , function () { alertify.error('Cancel') });
});
$("#approverBtn").click(function () {

    alertify.confirm('Confirm Approve', 'Do you want to Approve this Memo?', function () {
        $('#approverBtn').attr('disabled', 'disabled');

        var memoId = $('#estimateMemoId').val();
        var remarks = $('#approved_remarks').val();

        var requestData = {
            memoId: memoId,
            feedback: '100',
            remarks: remarks,
            isFinalApproved: false
        }

        var dataJson = JSON.stringify(requestData);
        var postData = {
            "requestDto": dataJson
        };
        $.ajax({
            url: "/BudgetMemo/SaveMemoApproval",
            type: "POST",
            data: postData,
            beforeSend: function () {
                $.blockUI({
                    timeout: 0,
                    message: 'Processing..'
                });
            },
            success: function (response) {
                if (parseInt(response) > 0) {
                    $.unblockUI();
                    alertify.alert('SUCCESS', 'Successfully Approved Memo!', function () { window.location = "/BudgetMemo/MyApprovals"; });
                }
                else {
                    alertify.alert(response, function () { window.location = "/BudgetMemo/MyApprovals"; });
                }
            },
            complete: function () {
                $.unblockUI();
            },
            error: function (data) {
                $.unblockUI();
            }
        });
    }
        , function () { alertify.error('Cancel') });
});
$("#rollBackButton").click(function () {

    alertify.confirm('Confirm RollBack', 'Do you want to RollBack this Memo?', function () {
        $('#rollBackButton').attr('disabled', 'disabled');

        var memoId = $('#estimateMemoId').val();
        var remarks = $('#approved_remarks').val();

        var requestData = {
            memoId: memoId,
            feedback: '-404',
            remarks: remarks,
            isFinalApproved: false
        }

        var dataJson = JSON.stringify(requestData);
        var postData = {
            "requestDto": dataJson
        };
        $.ajax({
            url: "/BudgetMemo/SaveMemoApproval",
            type: "POST",
            data: postData,
            beforeSend: function () {
                $.blockUI({
                    timeout: 0,
                    message: 'Processing..'
                });
            },
            success: function (response) {
                if (parseInt(response) > 0) {
                    $.unblockUI();
                    alertify.alert('SUCCESS', 'Successfully RollBack Memo!', function () { window.location = "/BudgetMemo/MyApprovals"; });
                }
                else {
                    alertify.alert(response, function () { window.location = "/BudgetMemo/MyApprovals"; });
                }
            },
            complete: function () {
                $.unblockUI();
            },
            error: function (data) {
                $.unblockUI();
            }
        });
    }
        , function () { alertify.error('Cancel') });
});
$("#rejectButton").click(function () {

    alertify.confirm('Confirm Reject', 'Do you want to Reject this Memo?', function () {
        $('#rejectButton').attr('disabled', 'disabled');

        var memoId = $('#estimateMemoId').val();
        var remarks = $('#approved_remarks').val();

        var requestData = {
            memoId: memoId,
            feedback: '-500',
            remarks: remarks,
            isFinalApproved: false
        }

        var dataJson = JSON.stringify(requestData);
        var postData = {
            "requestDto": dataJson
        };
        console.log(postData);


        $.ajax({
            url: "/BudgetMemo/SaveMemoApproval",
            type: "POST",
            data: postData,
            beforeSend: function () {
                $.blockUI({
                    timeout: 0,
                    message: 'Processing..'
                });
            },
            success: function (response) {
                if (parseInt(response) > 0) {
                    $.unblockUI();
                    alertify.alert('SUCCESS', 'Memo Successfully Rejected!', function () { window.location = "/BudgetMemo/MyApprovals"; });
                }
                else {
                    alertify.alert(response, function () { window.location = "/BudgetMemo/MyApprovals"; });
                }
            },
            complete: function () {
                $.unblockUI();
            },
            error: function (data) {
                $.unblockUI();
            }
        });
    }
        , function () { alertify.error('Cancel') });
});
$("#finalApproverBtn").click(function () {

    alertify.confirm('Final Approval', 'Do you want really want to finally  approved this Memo without MD sir Approval ?', function () {
        $('#finalApproverBtn').attr('disabled', 'disabled');

        var memoId = $('#estimateMemoId').val();
        var remarks = $('#approved_remarks').val();

        var requestData = {
            memoId: memoId,
            feedback: '100',
            remarks: remarks,
            isFinalApproved: true
        }

        var dataJson = JSON.stringify(requestData);
        var postData = {
            "requestDto": dataJson
        };
        console.log(postData);


        $.ajax({
            url: "/BudgetMemo/SaveMemoApproval",
            type: "POST",
            data: postData,
            beforeSend: function () {
                $.blockUI({
                    timeout: 0,
                    message: 'Processing..'
                });
            },
            success: function (response) {
                if (parseInt(response) > 0) {
                    $.unblockUI();
                    alertify.alert('SUCCESS', 'Successfully Approved the Memo as a Final Approver!', function () { window.location = "/BudgetMemo/MyApprovals"; });
                }
                else {
                    alertify.alert(response, function () { window.location = "/BudgetMemo/MyApprovals"; });
                }
            },
            complete: function () {
                $.unblockUI();
            },
            error: function (data) {
                $.unblockUI();
            }
        });
    }
        , function () { alertify.error('Cancel') });
});
function validationCheckForUpdate() {

    var response = true;

    var justification = $("#justificationId").val();

    if (!justification || justification == '') {
        showErrorMessageBelowCtrl('justificationId', 'please add Justification', true);
        response = false;
        $('html, body').animate({
            scrollTop: $("#justificationId").offset().top
        }, 800);
    }
    else
        showErrorMessageBelowCtrl('justificationId', 'please add Justification', false);

    let checkFlagMD = false;
    var approverDeptList = [];
    $("#TableAprroverList tr").each(function (a, b) {
        var priorityFromtb = $('.approverPriority', b).text()
        approverDeptList.push($('.approverDepartmentId', b).text());
        if (priorityFromtb == 1) {
            checkFlagMD = true;
        }
    });

    var tableApproverRowCount = $('#TableAprroverList tr').length;
    if (tableApproverRowCount <= 1) {
        showErrorMessageBelowCtrl('app_err-container', 'please add alteast one approver', true);
        response = false;
        $('html, body').animate({
            scrollTop: $("#app_err-container").offset().top
        }, 800);
    }
    else {
        let recomendorAdded = 0;
        let finalAcknowledgerAdded = 0;

        $("#TableAprroverList tr").each(function (a, b) {
            var role_piority = $('.approverPriorityType', b).text();

            if (Number(role_piority) == 1) {
                finalAcknowledgerAdded = 1;
            }

            if (Number(role_piority) == 2) {
                recomendorAdded = 1;
            }

        });

        if (finalAcknowledgerAdded == 0) {
            showErrorMessageBelowCtrl('employeeListforApprover', 'please add atlest one Final Approver', true);
            validFlag = 0;
            response = false;
        }
        if (recomendorAdded == 0) {
            showErrorMessageBelowCtrl('employeeListforApprover', 'please add atlest one  Recomendor', true);
            validFlag = 0;
            response = false;
        }
        showErrorMessageBelowCtrl('app_err-container', 'please add alteast one approver', false);
    }
    return response;
}
$("#updateBudgeMemo").click(function () {
    var result = validationCheckForUpdate();
    if (result == false) {
        $('#updateBudgeMemo').prop('disabled', false);
        return;
    }
    alertify.confirm('Confirm Submit', 'Do you want to update this Memo?', function () {
        $('#updateBudgeMemo').attr('disabled', 'disabled');
        var data = ConstructResponseObject();
        if (data == false)
            return;

        var estimationAttachment = [];

        formData = new FormData();
        for (var i = 0; i < filelist.length; i++) {
            if (jQuery.isEmptyObject(filelist[i])) {
                console.log("skip");
                continue;
            }
            formData.append(filelist[i].name, filelist[i]);
        }
        estimationAttachment = formData;

        postDataForBudgetEstimation(data, estimationAttachment);
    }
        , function () { alertify.error('Cancel') });
});
$(".removeAttachServer").click(function () {
    $(this).parent().remove();
    Swal.fire({
        title: 'Are you sure?',
        text: "You will not be able to recover this attachment!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "GET",
                url: "/BudgetMemo/DeleteAttachmentsById?id=" + this.id,
                data: "{}",
                success: function (response) {
                    if (response) {
                    }
                }
            });
        }
    });
});
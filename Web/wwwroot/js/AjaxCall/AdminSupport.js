var itemList = [];
var storeUsersWithDepartmentName;
$('.autoSuggestionSelect').css('width', '100%');
$(".autoSuggestionSelect").select2({});
function getInitData() {
    $.ajax({
        type: "GET",
        url: "/AdminSetUp/GetItemInitData",
        data: "{}",
        success: function (data) {

            var paritcularList = data.paritcularList;
            var itemCategoryList = data.itemCategoryList;
            var unitList = data.unitList;
            var moduleList = data.moduleList;
            var statusList = data.statusList;
            var roleList = data.roleList;
            var departmentList = data.departmentList;
            var users = data.users;
            storeUsersWithDepartmentName = users;

            var s = '<option value="-1">--Please Select--</option>';
            for (var i = 0; i < paritcularList.length; i++) {
                s += '<option value="' + paritcularList[i].id + '">' + paritcularList[i].name + '</option>';
            }
            $("#ddlParticular").html(s);

            var s = '<option value="-1">--Please Select--</option>';
            for (var i = 0; i < itemCategoryList.length; i++) {
                s += '<option value="' + itemCategoryList[i].id + '">' + itemCategoryList[i].name + '</option>';
            }
            $("#ddlItemCategory").html(s);

            var s = '<option value="-1">--Please Select--</option>';
            for (var i = 0; i < unitList.length; i++) {
                s += '<option value="' + unitList[i].id + '">' + unitList[i].name + '</option>';
            }
            $("#ddlUnit").html(s);

            var s = '<option value="-1">--Please Select--</option>';
            for (var i = 0; i < moduleList.length; i++) {
                s += '<option value="' + moduleList[i].id + '">' + moduleList[i].name + '</option>';
            }
            $("#ddlModules").html(s);

            var s = '<option value="-1">--Please Select--</option>';
            for (var i = 0; i < statusList.length; i++) {
                s += '<option value="' + statusList[i].id + '">' + statusList[i].name + '</option>';
            }
            $("#ddlStatus").html(s);

            var s = '<option value="-1">--Please Select--</option>';
            for (var i = 0; i < roleList.length; i++) {
                s += '<option value="' + roleList[i].id + '">' + roleList[i].name + '</option>';
            }
            $("#ddlRole").html(s);

            var s = '<option value="-1">--Please Select--</option>';
            for (var i = 0; i < departmentList.length; i++) {
                s += '<option value="' + departmentList[i].id + '">' + departmentList[i].name + '</option>';
            }
            $("#ddlDepartment").html(s);

            for (var i = 0; i < users.length; i++) {
                var o = new Option(users[i].fullName, users[i].userID);
                $(o).html(users[i].fullName + "/ " + users[i].departmentName);
                $("#employeeListforApprover").append(o);
            }
        }
    });
}
$("#submitButton").click(function () {
    var data = new FormData();
    data.append('ItemName', $('#itemName').val());
    data.append('ItemCode', $('#itemCode').val());
    data.append('ParticularId', $('#ddlParticular').find(":selected").val());
    data.append('ItemCategoryId', $('#ddlItemCategory').find(":selected").val());
    data.append('UnitId', $('#ddlUnit').find(":selected").val());

    $.ajax({
        processData: false,
        contentType: false,
        type: "POST",
        url: "/AdminSetUp/SaveItem",
        data: data,
        enctype: 'multipart/form-data',
        beforeSend: function () {
        },
        success: function (response) {
            if (response.success) {
                alertify.notify('Saved Successfully!', 'success', 1);
            }
            else {
                alertify.error(response.message);
            }
        },
        complete: function () {
            console.log("complete");
        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });
});
$("#submitAMSButton").click(function () {
    var data = new FormData();
    data.append('ItemName', $('#itemName').val());
    data.append('IndicatingUnitPrice', $('#unitPrice').val());
    data.append('ParticularId', $('#ddlParticular').find(":selected").val());
    data.append('ItemCategoryId', $('#ddlItemCategory').find(":selected").val());
    data.append('UnitId', $('#ddlUnit').find(":selected").val());

    $.ajax({
        processData: false,
        contentType: false,
        type: "POST",
        url: "/AdminSetUp/AMSSaveItem",
        data: data,
        enctype: 'multipart/form-data',
        beforeSend: function () {
        },
        success: function (response) {
            if (response.success) {
                alertify.notify('Saved Successfully!', 'success', 1);
            }
            else {
                alertify.error(response.message);
            }
        },
        complete: function () {
            console.log("complete");
        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });
});
$("#TableAprroverList").sortable({
    items: 'tr:not(tr:first-child)',
    cursor: 'pointer',
    axis: 'y',
    dropOnEmpty: false,
    start: function (e, ui) {
        ui.item.addClass("selected");
    },
    stop: function (e, ui) {
        ui.item.removeClass("selected");
        $(this).find("tr").each(function (index) {
            if (index >= 0) {
                $(this).find("td").eq(2).html(index);
            }
        });
    }
});
$("#particularSubmit").click(function () {
    var data = new FormData();
    data.append('Name', $('#particularName').val());

    $.ajax({
        processData: false,
        contentType: false,
        type: "POST",
        url: "/AdminSetUp/SaveParticular",
        data: data,
        enctype: 'multipart/form-data',
        beforeSend: function () {
        },
        success: function (response) {
            if (response.success) {
                alertify.notify('Saved Successfully!', 'success', 1);
            }
            else {
                alertify.error(response.message);
            }
        },
        complete: function () {
            console.log("complete");
        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });
});
$("#itemCategorySubmit").click(function () {
    var data = new FormData();
    data.append('ParticularId', $('#ddlParticular').find(":selected").val());
    data.append('Name', $('#itemCategoryName').val());

    $.ajax({
        processData: false,
        contentType: false,
        type: "POST",
        url: "/AdminSetUp/SaveItemCategory",
        data: data,
        enctype: 'multipart/form-data',
        beforeSend: function () {
        },
        success: function (response) {
            if (response.success) {
                alertify.notify('Saved Successfully!', 'success', 1);
            }
            else {
                alertify.error(response.message);
            }
        },
        complete: function () {
            console.log("complete");
        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });
});
$("#ddlParticular").change(function () {

    var particularId = $('#ddlParticular').find(":selected").val();

    $.ajax({
        type: "GET",
        url: "/AdminSetup/GetItemCategoryByParticularId?particularId=" + particularId,
        success: function (response) {
            if (response != null) {
                $("#ddlItemCategory").empty();
                var s = '<option value="-1">--Please Select--</option>';
                for (var i = 0; i < response.length; i++) {
                    s += '<option value="' + response[i].id + '">' + response[i].name + '</option>';
                }
                $("#ddlItemCategory").html(s);
            }
        }
    });
});
$("#showApprover").click(function () {
    var moduleId = $('#ddlModules').find(":selected").val();
    var requestNo = $('#requestNo').val();
    $.ajax({
        type: "GET",
        url: "/AdminSetup/GetAllApprover?moduleId=" + moduleId + '&requestNo=' + requestNo,
        data: "{}",
        success: function (data) {
            var approvers = data;
            $('#tApproverbody').empty();
            for (var i = 0; i < approvers.length; i++) {
                $('#tApproverbody').append(
                    `<tr>
                        <td>&nbsp;</td>
                        <td class="approverName">` + approvers[i].approverFullName + ` </td>
                        <td class="approverId" hidden>` + approvers[i].approverId + `</td>
                        <td class="approverStatus" hidden>` + approvers[i].approverStatus + `</td>
                        <td class="approverPriority" hidden>` + approvers[i].approverPriority + `</td>
                        <td class="approverPriorityType" hidden>` + approvers[i].approverRoleId + `</td>
                        <td class="approverPriorityTypeText">` + approvers[i].approverRoleName + `</td>
                        <td class="approverDepartment">` + approvers[i].approverDepartment + `</td>
                        <td class="approverDepartmentId" hidden>` + approvers[i].approverDepartmentId + `</td>
                        <td class="approverExpectedTime">` + approvers[i].planDateString + `</td>
                        <td><input class="checkbox" type="checkbox"/></td>
                    </tr>`);
            }
        }
    });
});
$("#employeeListforApprover").change(function () {
    var selectorID = $("#employeeListforApprover").val();
    for (var x = 0; x < storeUsersWithDepartmentName.length; x++) {
        if (storeUsersWithDepartmentName[x].userID == selectorID) {
            $("#userDepartmentID").val(storeUsersWithDepartmentName[x].departmentName);
            $("#DeptIdUserID").val(storeUsersWithDepartmentName[x].departmentId);
            break;
        }
    }
});
$('#addApprover').on('click',
    function () {
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
                addApproverinApproverList(selectedArrproverTyepVal,2, "Recommender");
            } else if (selectedsequenORparallleVal == "Parallel" && selectedArrproverTyepVal == "2") {
                addApproverinApproverList(selectedArrproverTyepVal,2, "Recommender");
            } else if (selectedArrproverTyepVal == "1") {
                addApproverinApproverList(selectedArrproverTyepVal,2, "Final Approver");
            } else if (selectedArrproverTyepVal == "3") {
                addApproverinApproverList(selectedArrproverTyepVal,100, "Informed");
            }
        }

    });
function addApproverinApproverList(priorityType, statusId, priorityTypeTxt) {

    var username = $('#employeeListforApprover').find(":selected").text();
    var userId = $('#employeeListforApprover').find(":selected").val();
    var userDepartment = $('#userDepartmentID').val();
    var DeptIdUserID = $('#DeptIdUserID').val();
    var expectedDate = $('#datetimepickerApproverTimeLine').val();
    var usernameValue = $('#employeeListforApprover').find(":selected").val();

    if (usernameValue <= 0) {
        showErrorMessageBelowCtrl('employeeListforApprover', 'please add employee', true);
        return;
    } else {
        showErrorMessageBelowCtrl('employeeListforApprover', 'please select employee', false);
    }
    if (expectedDate.length <= 0) {
        showErrorMessageBelowCtrl('datetimepickerApproverTimeLine', 'please select expected timeline', true);
        return;
    } else {
        showErrorMessageBelowCtrl('datetimepickerApproverTimeLine', 'please select expected timeline', false);
    }

    $('#tApproverbody').append(
        `<tr>
            <td>&nbsp;</td>
            <td class="approverName">` + username + `</td>
            <td class="approverId" hidden>` + userId + `</td>
            <td class="approverStatus" hidden>` + statusId + `</td>
            <td class="approverPriorityType" hidden>` + priorityType + `</td>
            <td class="approverPriorityTypeText">` + priorityTypeTxt + `</td>
            <td class="approverDepartment">` + userDepartment + `</td>
            <td class="approverDepartmentId" hidden>` + DeptIdUserID + `</td>
            <td class="approverExpectedTime">` + expectedDate + `</td>
            <td class="text-center">
                <input class="checkbox" type="checkbox"/>
            </td>
        </tr>`
    );
}
$("#updateApprover").click(function () {

    var data = new FormData();

    data.append('ModuleId', $('#ddlModules').find(":selected").val());
    data.append('RequestNo', $('#requestNo').val());

    $("#TableAprroverList tr").each(function (a, b) {
        if (a > 0) {
            itemList.push({
                ApproverId: $('.approverId', b).text(),
                StatusId: $('.approverStatus', b).text(),
                RolePriority_Id: $('.approverPriorityType', b).text(),
                PlanDate: $('.approverExpectedTime', b).text(),
                IsApproved: $(".checkbox", b).is(":checked")
            });
        }
    });
    data.append('Items', JSON.stringify(itemList));
    $.ajax({
        processData: false,
        contentType: false,
        type: "POST",
        url: "/AdminSetup/UpdateApproverModification",
        data: data,
        enctype: 'multipart/form-data',
        beforeSend: function () {
        },
        success: function (response) {
            if (response.success) {
                alertify.notify('Approver Modification Success!', 'success', 1);
                $('#tApproverbody').empty();
            }
            else {
                alertify.error(response.message);
            }
        },
        complete: function () {
            console.log("complete");
        },
        failure: function (response) {
            alertify.error('Something went wrong! please try again.');
        },
        error: function (response) {
            alertify.error('Something went wrong! please try again.');
        }
    });
});
$("#updateRequestStatus").click(function () {

    var moduleId = $('#ddlModules').find(":selected").val();
    var statusId = $('#ddlStatus').find(":selected").val();
    var requestNo = $('#requestNo').val();

    if (moduleId == -1 || statusId == -1 || requestNo == null || requestNo == '') {
        alertify.notify('Please Fill all required field!', 'error', 1);
        return;
    }

    var data = new FormData();

    data.append('ModuleId', moduleId);
    data.append('StatusId', statusId);
    data.append('RequestNo', requestNo);

    $.ajax({
        processData: false,
        contentType: false,
        type: "POST",
        url: "/AdminSetup/UpdateRequestStatus",
        data: data,
        enctype: 'multipart/form-data',
        beforeSend: function () {
        },
        success: function (response) {
            if (response.success) {
                alertify.notify('Status Updated Successfully!', 'success', 1);
            }
            else {
                alertify.error(response.message);
            }
        },
        complete: function () {
            console.log("complete");
        },
        failure: function (response) {
            alertify.error('Something went wrong! please try again.');
        },
        error: function (response) {
            alertify.error('Something went wrong! please try again.');
        }
    });
});
$("#updateApproverRole").click(function () {
    var data = new FormData();
    data.append('ModuleId', $('#ddlModules').find(":selected").val());
    data.append('RequestNo', $('#requestNo').val());
    data.append('ApproverId', $('#employeeListforApprover').find(":selected").val());
    data.append('RoleId', $('#ddlRole').find(":selected").val());

    $.ajax({
        processData: false,
        contentType: false,
        type: "POST",
        url: "/AdminSetUp/UpdateApproverRole",
        data: data,
        enctype: 'multipart/form-data',
        beforeSend: function () {
        },
        success: function (response) {
            if (response.success) {
                alertify.notify('Saved Successfully!', 'success', 1);
            }
            else {
                alertify.error(response.message);
            }
        },
        complete: function () {
            console.log("complete");
        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });
});
$("#updateUserDepartment").click(function () {
    var data = new FormData();
    data.append('UserId', $('#employeeListforApprover').find(":selected").val());
    data.append('DepartmentId', $('#ddlDepartment').find(":selected").val());

    $.ajax({
        processData: false,
        contentType: false,
        type: "POST",
        url: "/AdminSetUp/UpdateUserDepartment",
        data: data,
        enctype: 'multipart/form-data',
        beforeSend: function () {
        },
        success: function (response) {
            if (response.success) {
                alertify.notify('Saved Successfully!', 'success', 1);
            }
            else {
                alertify.error(response.message);
            }
        },
        complete: function () {
            console.log("complete");
        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });
});
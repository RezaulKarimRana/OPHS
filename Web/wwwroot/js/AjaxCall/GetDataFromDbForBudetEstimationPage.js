//get All District from backend function
validateDropDownField('approvalForDropDown', 'approvalForDropDown', 'please select approval for');
validateTextField('subjectTextId', 'please add subject');
validateTextField('datetimepickerEstimationStartDate', 'please add start date');
validateTextField('datetimepickerEstimationEndDate', 'please add end date');

function getDistrictsFrmDb(parameter) {
    $.ajax({
        type: "GET",
        url: "/District/GetAllDistricts",
        data: "{}",
        success: function (data) {
            var districts = data.districts;
            var s = '<option value="-1">Select District</option>';
            for (var i = 0; i < districts.length; i++) {
                s += '<option value="' + districts[i].id + '">' + districts[i].name + '</option>';
            }
            $("#districtDropdown" + parameter).html(s);
        }
    });
}

//get ThanaByDistrictId from backend function
function getThanaByDistIdFromDB(value, rowTrack) {
    $.ajax({
        type: "GET",
        url: "/Thana/GetThanaByDistrictId?distId=" + value,
        data: "{}",
        success: function (data) {
            var thanas = data.thanaS;
            var s = '<option value="-1">Select Thana</option>';
            for (var i = 0; i < thanas.length; i++) {
                s += '<option value="' + thanas[i].id + '">' + thanas[i].name + '</option>';
            }
            $("#thanaDropdown" + rowTrack).html(s);
        }
    });
}

//get all Department from backend function
function getDepartmentsFrmDb(parameter) {
    $.ajax({
        type: "GET",
        url: "/Department/GetAllDepartmentsJoinUser",
        data: "{}",
        success: function (data) {
            var departments = data.departments;
            var s = '<option value="-1">Select Department</option>';
            for (var i = 0; i < departments.length; i++) {
                s += '<option value="' + departments[i].id + '">' + departments[i].name + '</option>';
            }
            $("#departmentDropdown" + parameter).html(s);
            
            //if (parameter == 0)
            //    $('#deptOrDivTextId').html(s);
        }
    });
}

function getDepartmentsForDropDownFrmDb() {
    $.ajax({
        type: "GET",
        url: "/Department/GetAllDepartmentsJoinUser",
        data: "{}",
        success: function (data) {
            var departments = data.departments;
            var s = '<option value="-1">Select Department</option>';
            for (var i = 0; i < departments.length; i++) {
                s += '<option value="' + departments[i].id + '">' + departments[i].name + '</option>';
            }
            $('#deptOrDivTextId').html(s);
                
        }
    });
}

//get ItemDetailsByParticularID from backend function
function getItemCategoryById(value, rowTrack) {
    $.ajax({
        type: "GET",
        url: "/ItemCategory/GetAllItemCategoriesBYparticularId?particularId=" + value,
        data: "{}",
        success: function (data) {
            var itemsCategories = data.itemCategories;
            var s = '<option value="-1">Select Item Category</option>';
            for (var i = 0; i < itemsCategories.length; i++) {
                s += '<option value="' + itemsCategories[i].id + '">' + itemsCategories[i].name + '</option>';
            }
            $("#itemCategoryDropdown" + rowTrack).html(s);
        }
    });
}

//get all particulars from backend function
function getParticularsFromDb(parameter) {
    $.ajax({
        type: "GET",
        url: "/Particular/GetParticulars",
        data: "{}",
        success: function (data) {
            var particulars = data.particulars;
            var s = '<option value="-1">Select Particular</option>';
            for (var i = 0; i < particulars.length; i++) {
                s += '<option value="' + particulars[i].id + '">' + particulars[i].name + '</option>';
            }
            $("#paticularDropdown" + parameter).html(s);
        }
    });
}

//get saved estimation's subject name from db
function getEstimationSubjectNamesFromDb() {
    $.ajax({
        type: "GET",
        url: "/Estimation/GetAllEstimationName",
        data: "{}",
        success: function (data) {
            var estimationNames = data.estimationNames;
            var s = '<option value="">Select</option>';
            for (var i = 0; i < estimationNames.length; i++) {
                s += '<option value="' + estimationNames[i].id + '">' + estimationNames[i].subject + '</option>';
            }
            $("#LinkEstimateSubjectListForDropDown").html(s);
        }
    });
}

//get users from back end
function getUserWithDepartmentNameFromDb() {
    $.ajax({
        type: "GET",
        url: "/User/GetAllUsersWithDepartmentName",
        data: "{}",
        success: function (data) {
            var users = data.users;
            storeUsersWithDepartmentName = users;
            var s = '<option value="-1">Select Approver</option>';
            for (var i = 0; i < users.length; i++) {
                s += '<option value="' + users[i].userID + '">' + users[i].fullName + '/ ' + users[i].departmentName + '</option>';
            }
            $("#employeeListforApprover").html(s);
        }
    });
}

//get ItemDetailsByParticularID from backend function
function getItemUnitDetails(value, rowTrack) {
    $.ajax({
        type: "GET",
        url: "/Item/GetItemUnitDetails?itemCategoryId=" + value,
        data: "{}",
        success: function (data) {
            var items = data.itemUnitModel;
            storeItemDetails = items;
            var s = '<option value="-1">Select Item</option>';
            for (var i = 0; i < items.length; i++) {
                s += '<option value="' + items[i].id + '">' + items[i].name + '</option>';
            }
            $("#itemDropdown" + rowTrack).html(s);
        }
    });

}
$('.autoSuggestionSelect').css('width', '100%');
$(".autoSuggestionSelect").select2({});
var districts = [];
var particulars = [];
var areas = [];
var departments = [];
function GetSettlementInitData(parameter) {
    $.ajax({
        type: "GET",
        url: "/Settlement/GetSettlementInitData",
        data: "{}",
        success: function (data) {
            districts = data.districtList;
            particulars = data.particularList;
            areas = data.areaTypeList;
            departments = data.departmentList;

            var s = '<option value="-1">Select District</option>';
            for (var i = 0; i < districts.length; i++) {
                s += '<option value="' + districts[i].id + '">' + districts[i].name + '</option>';
            }
            $("#districtDropdown" + parameter).html(s);

            var s = '<option value="-1">Select Particular</option>';
            for (var i = 0; i < particulars.length; i++) {
                s += '<option value="' + particulars[i].id + '">' + particulars[i].name + '</option>';
            }
            $("#paticularDropdown" + parameter).html(s);

            var s = '<option value="-1">Select Area</option>';
            for (var i = 0; i < areas.length; i++) {
                s += '<option value="' + areas[i].id + '">' + areas[i].name + '</option>';
            }
            $("#areaTypeDropdown" + parameter).html(s);

            var s = '<option value="-1">Select Department</option>';
            for (var i = 0; i < departments.length; i++) {
                s += '<option value="' + departments[i].id + '">' + departments[i].name + '</option>';
            }
            $("#departmentDropdown" + parameter).html(s);
        }
    });
}
function getParticularsDropdown(parameter) {

    var s = '<option value="-1">Select Particular</option>';
    for (var i = 0; i < particulars.length; i++) {
        s += '<option value="' + particulars[i].id + '">' + particulars[i].name + '</option>';
    }
    $("#paticularDropdown" + parameter).html(s);
}
function getDepartmentsDropdown(parameter) {
    var s = '<option value="-1">Select Department</option>';
    for (var i = 0; i < departments.length; i++) {
        s += '<option value="' + departments[i].id + '">' + departments[i].name + '</option>';
    }
    $("#departmentDropdown" + parameter).html(s);
}
function getAreasDropdown(parameter) {
    var s = '<option value="-1">Select Area</option>';
    for (var i = 0; i < areas.length; i++) {
        s += '<option value="' + areas[i].id + '">' + areas[i].name + '</option>';
    }
    $("#areaTypeDropdown" + parameter).html(s);
}
function getDistrictsDropdown(parameter) {
    var s = '<option value="-1">Select District</option>';
    for (var i = 0; i < districts.length; i++) {
        s += '<option value="' + districts[i].id + '">' + districts[i].name + '</option>';
    }
    $("#districtDropdown" + parameter).html(s);
}
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
function getDepartmentsForDropDownFrmDb() {
    var s = '<option value="-1">Select Department</option>';
    for (var i = 0; i < departments.length; i++) {
        s += '<option value="' + departments[i].id + '">' + departments[i].name + '</option>';
    }
    $("#deptOrDivTextId").html(s);
}
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
function addTableRow(idx) {
    $('#tbody').
        append(`<tr id="R${idx}" class="newAddRowColor">
                <td>&nbsp;</td>
                <td class="estimateSettleItemId" hidden>-1</td>
                <td>
                    <select class="form-control-sm particularSelectorFromTable  autoSuggestionSelect" id="paticularDropdown${idx}" style="width:100%" name="particularSelector"></select>
                </td>
                <td>
                    <select class="form-control-sm itemCategoryDropdownSelector autoSuggestionSelect" id="itemCategoryDropdown${idx}" name="itemCategoryDropdown"></select>
                </td>
                <td>
                    <select class="form-control-sm itemSelectorFromTable autoSuggestionSelect" id="itemDropdown${idx}" name="itemDropdown">
                </td>
                <td>
                    <span id="itemCodeTextId${idx}" class="itemCodeText" style="color:chocolate"></span>
                </td>
                <td>
                    <span id="itemUnitTextId${idx}" class="itemUnitText" style="color:chocolate"></span>
                </td>
                <td>
                    <input type="text" class="form-control-sm findTotalPrice nomuct singleRowFindTotalPrice${idx} numbersOnly" id="NoOfMaUTCaTextId${idx}" value="N/A" style="width:75px" disabled />
                </td>
                <td>
                    <input type="text" class="form-control-sm findTotalPrice nodut singleRowFindTotalPrice${idx} numbersOnly" id="NoOfDTUTextId${idx}" value="N/A" style="width:75px"  disabled/>
                </td>
                <td>
                    <input type="text" class="form-control-sm findTotalPrice rqt singleRowFindTotalPrice${idx} floatNumbersOnly" id="requiredQuantityTextId${idx}" value="N/A" style="width:75px" disabled/>
                </td>
                <td>
                    <input type="text" class="form-control-sm itemUnitPrice floatNumbersOnly" id="unitPriceTextId${idx}" value="N/A" style="width:100px" disabled />
                </td>
                    <td class="form-control-sm totalPriceColumn" id="TotalPriceTextId${idx}" style="color:chocolate" hidden> 0
                </td>
                    <td class="form-control-sm" id="TotalPriceFormateTextId${idx}" style="text-align:right; color:chocolate"> 0
                </td>
                <td>
                    <select class="form-control-sm departmentSelector autoSuggestionSelect" id="departmentDropdown${idx}" name="demartmentDropdown">
                </td>
                <td>
                    <select class="form-control-sm distSelectorFromTable autoSuggestionSelect" id="districtDropdown${idx}" name="districtDropdown">
                </td>
                <td>
                    <select class="form-control-sm particularTableThana autoSuggestionSelect" id="thanaDropdown${idx}" name="thanaDropdown">
                </td>
                <td>
                    <select class="form-control-sm areaType autoSuggestionSelect" id="areaTypeDropdown${idx}" name="areaTypeDropdown">
                </td>
                <td>
                    <input type="text" class="onlyNumber findTotalPrice rqt singleRowFindTotalPrice${idx}" style="width:50px"
                                    id="settleActualQuantityTextId${idx}" value="" disabled />
                </td>
                <td>
                    <input type="text" class="onlyNumber findTotalPrice nodut singleRowFindTotalPrice${idx}" style="width:50px"
                                    id="alreadySettleCost${idx}" value="" disabled />
                </td>
                <td class="settleItemId" hidden>
                ` + -1 + `
                </td>
                <td >
                        <input type="text" class="onlyNumber actualQuantity  actualQuantity{idx}" style="width:50px"
                            id="actualQuantity${idx}" value=""  />
                    </td>
                    <td >
                        <input type="text" class="onlyNumber actualUnitPrice  actualUnitPrice${idx}" style="width:50px"
                            id="actualUnitPrice${idx}" value=""  />
                    </td>
                    <td >
                        <input type="text" class="onlyNumber actualTotalPrice  actualTotalPrice${idx}" style="width:50px"
                            id="actualTotalPrice${idx}" value="" disabled />
                    </td>
                        <td colspan="2">
                        <input type="text" class="settleItemRemarks  settleItemRemarks${idx}"
                            id="settleItemRemarks${idx}" value="
                "  />
                    </td>
                <td class="text-center">
                    <button class="btn btn-danger remove" type="button"><i class="fa fa-trash" aria-hidden="true"></i></button>
                </td>
            </tr>`);
    $('.autoSuggestionSelect').css('width', '100%');
    $(".autoSuggestionSelect").select2({});
}
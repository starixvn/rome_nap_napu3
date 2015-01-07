/// <reference path="../../../../js/jQuery/jquery.js" />
/// <reference path="../../../../js/jQuery/json2.js" />
/// <reference path="../../../../js/PT/CommonPT.js" />
/// <reference path="../../../../js/common.js" />

//load function
$(function () {
    eda.init();
});

var customer;
var context;
var ptGrid = null;
var bettingLimitGrid = null;
var myCustomizeValue = null;
var isShowTransferOption = true;
var isInsterTwice = false;
var isUpdateAllMembers = false;
var newPositionTakingValue = 0;

var eda = {
    ptGrid: null
};
//init
eda.init = function () {
    eda.initContext();
};
//biding for knockout
eda.initForKnockout = function () {
    var CustomerModel = function (customer) {
        this.saveCustomer = function () {
            //console.log(JSON.stringify(ko.toJS(customer), null, '\t'));
        };
    };
    ko.applyBindings(new CustomerModel(customer));
};
//get context of form
eda.initContext = function () {
    $.ajax({
        type: "POST",
        url: "GetContextObject",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '{"masterId":' + JSON.stringify(masterId) + ', "agentId": ' + JSON.stringify(agentId) + ',"isDisplaySportbooks":' + JSON.stringify(isDisplaySportbooks) + '}',
        success: function (stringContext) {
            context = JSON.parse(stringContext);
            eda.initCustomer();
        }
        , error: function (e) {
            showAlertMessage("err", "Init Context Error", 1);
        }
    });
};
//get default super value
eda.initCustomer = function () {
    $.ajax({
        type: "POST",
        url: "GetCustomerObject",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '{"requestContext":' + JSON.stringify(context) + '}',
        success: function (stringCustomer) {
            customer = JSON.parse(stringCustomer);
            eda.initPtGrid();
            eda.initForKnockout();
            eda.initCommission();
            storeOldPTSetting = Fanex.copy(customer.ptSetting);
            $()
        }
        , error: function (e) {
            showAlertMessage("err", "Init Customer Error", 1);
        }
    });
};
//init PT Object Grid
eda.initPtGrid = function () {
    Fanex.PTEngine.CURRENT_CONTEXT = context;
    Fanex.PTEngine.CURRENT_CONTEXT.ia3 = !customer.statusSetting.disableAutoMPT;
    ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);
    var divPtGrid = document.getElementById('divPtGrid');
    ptGrid.Render(divPtGrid);

    bettingLimitGrid = new Fanex.PTEngine.BettingLimit(customer.bettingLimit);
    var bettingLimitContainer = document.getElementById('divBettingLimit');
    bettingLimitContainer.innerHTML = "";
    bettingLimitGrid.Render(bettingLimitContainer);
};
//render Commission combobox
eda.initCommission = function () {
    var commissionSetting = customer.commissionSetting;
    Fanex.PTEngine.PT_SetSelect(document.getElementById('listGameComm'), 0, Math.max(commissionSetting.discountNumberMax, commissionSetting.discountNumber), Fanex.PTEngine.COMMISSION_STEP, commissionSetting.discountNumber);
};
//render Transfer Option
// check valid user input
eda.checkValidInput = function () {
    //get commision
   var commissionSetting = customer.commissionSetting;
    commissionSetting.discountNumber = $('#listGameComm').val();
    return true;
};
// handel change auto pt
var isCheckChangeAuto = false;
var isShowPopUpSetPTMember = false;
var storeOldPTSetting = null;

// We need check have any PT value change from auto to none auto. And Set new value to Member
eda.CheckChangeAuto = function (ptSettingOld, ptSettingNew) {
    var sportTypes = ptSettingOld.sportTypes;
    for (var sportTypeIndex = 0; sportTypeIndex < sportTypes.length; sportTypeIndex++) {
        var sportTypesItem = sportTypes[sportTypeIndex]
        var betTypeGroupSets = sportTypesItem.betTypeGroupSets;

        for (var groupSetIndex = 0; groupSetIndex < betTypeGroupSets.length; groupSetIndex++) {
            var betTypeGroupSetsItem = betTypeGroupSets[groupSetIndex];
            var betTypeGroups = betTypeGroupSetsItem.betTypeGroups;

            for (var groupIndex = 0; groupIndex < betTypeGroups.length; groupIndex++) {
                var betTypeGroupItem = betTypeGroups[groupIndex];
                var betTypes = betTypeGroupItem.betTypes;

                for (var betTypeIndex = 0; betTypeIndex < betTypes.length; betTypeIndex++) {
                    betTypeItem = betTypes[betTypeIndex];
                    var isAutoOld = betTypeItem.pt.im2;
                    var isAutoNew = ptSettingNew.sportTypes[sportTypeIndex].betTypeGroupSets[groupSetIndex].betTypeGroups[groupIndex].betTypes[betTypeIndex].pt.im2;

                    if (isAutoOld && !isAutoNew) {
                        return true;
                    }
                }
            }
        }
    }
    return false;
};

// Get selected and m4 value of Soccer HdpD Master Preset
eda.MemberMaximumPT = function () {
    var ptSetting = customer.ptSetting;
    var l3 = ptSetting.sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].pt.l3;
    var a2 = ptSetting.sportTypes[0].betTypeGroupSets[0].betTypeGroups[0].betTypes[0].pt.a2;
    return Fanex.Round(l3 - a2);
};

// Confirm new value for all member before transfer to server
eda.SetNewValueToMember = function (isUpdateAllMember) {
    isShowPopUpSetPTMember = true;
    newPositionTakingValue = $('#selectedMemberPositionTaking').val();
    isUpdateAllMembers = isUpdateAllMember;
    //        var customerModel = new CustomerModel(customer);
    //        customerModel.saveCustomer();
    Popups.Close();
    isShowPopUpSetPTMember = false;
    eda.SendAjaxEditAgent();
};

function ConfirmYes() {
    eda.SetNewValueToMember(true);
}

function ConfirmNo() {
    eda.SetNewValueToMember(false);
}

eda.handleAutoChangeForMember = function () {
    if (context.targetCustId > 0) {
        if (!isShowPopUpSetPTMember) {
            //get pt setting
            customer.ptSetting = ptGrid.Save();
            isCheckChangeAuto = eda.CheckChangeAuto(storeOldPTSetting, customer.ptSetting);
        }
        if (isCheckChangeAuto && !isShowPopUpSetPTMember) {
            Popups.Confirm(eda.MemberMaximumPT(), Language.CONFIRM_MASTER_NOT_TO_TAKE_AUTO_POSITION_TAKING);
            return;
        }
        else if (isCheckChangeAuto == false) {
            eda.SendAjaxEditAgent();
        }
        else if (isCheckChangeAuto && isShowPopUpSetPTMember) {
            Popups.Close();
        }
    }
};
eda.SendAjaxEditAgent = function () {
    try { parent.BlockUI(); } catch (e) { }
    $.ajax({
        type: "POST",
        url: "DoEditAgent",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '{"stringCustomer":' + JSON.stringify(JSON.stringify(customer)) + ', "currentContext":' + JSON.stringify(context) + ', "isUpdateAllMembers":' + isUpdateAllMembers + ', "newPositionTakingValue":' + newPositionTakingValue + ',"isDisplaySportbooks":' + JSON.stringify(isDisplaySportbooks) + '}',
        success: function (result) {
            $('#divShowInfo').show();
            try { parent.jQuery.unblockUI(); } catch (e) { }
            if (typeof result != 'undefined' && result != null) {
                if (result.ErrorCode > 0) {
                    alert_error(result.ErrorMessage, 'err');
                    parent.scroll(0, 0);
                    window.scroll(0, 0);
                }
                else {
                    showAlertMessage("err", "Updated Agent Successfully!", 0);
                    //redirect to current page
                    setTimeout(function () { location.href = location.href; }, 3000);
                    try {
                        parent.afterUpdate(false, 2000);
                    } catch (e) { };
                }
            }
            else {
                showAlertMessage("err", "Update Agent Unsuccessfully!", 1);
            }
            //try { parent.parent.jQuery.unblockUI(); } catch (e) { }
        }
            , error: function (e) {
                $('#divShowInfo').show();
                if (e.status == '404' || e.statusText == 'Not Found') {
                    if (isInsterTwice == false) {
                        eda.SendAjaxEditMaster();
                        isInsterTwice = true;
                    }
                    else {
                        showAlertMessage("err", "Request Error", 1);
                    }
                }
                else {
                    showAlertMessage("err", "Request Error", 1);
                }
                //try { parent.parent.jQuery.unblockUI(); } catch (e) { }
                try { parent.jQuery.unblockUI(); } catch (e) { }
            }
    });
};
// do edit Agent
eda.editAgent = function () {
    if (eda.checkValidInput()) {
        //get betting limit
        if (bettingLimitGrid.Validate()) {
            customer.bettingLimit = bettingLimitGrid.Save();
        }
        else {
            return;
        }
        eda.handleAutoChangeForMember();
    }
    else {
        $('#divShowInfo').show();
    }
};

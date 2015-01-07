/// <reference path="../../../../js/jQuery/jquery.js" />
/// <reference path="../../../../js/jQuery/json2.js" />
/// <reference path="../../../../js/PT/CommonPT.js" />
//load function
$(function () {
    eds.init();
});

var customer;
var context;
var ptGrid = null;
var bettingLimitGrid = null;
var isInsterTwice = false;

var eds = {
    ptGrid: null
};
//init
eds.init = function () {
    eds.initContext();
};
//biding for knockout
eds.initForKnockout = function () {
    var CustomerModel = function (customer) {
        this.saveCustomer = function () {
            //console.log(JSON.stringify(ko.toJS(customer), null, '\t'));
        };
    };
    ko.applyBindings(new CustomerModel(customer));
};
//get context of form
eds.initContext = function () {
    $.ajax({
        type: "POST",
        url: "GetContextObject",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '{"superId":' + JSON.stringify(superId) + ',"isDisplaySportbooks":' + JSON.stringify(isDisplaySportbooks) + '}',
        success: function (stringContext) {
            context = JSON.parse(stringContext);
            eds.initCustomer();
        }
        , error: function (e) {
            showAlertMessage("err", "Init Context Error", 1);
        }
    });
};
//get default super value
eds.initCustomer = function () {
    $.ajax({
        type: "POST",
        url: "GetCustomerObject",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: '{"requestContext":' + JSON.stringify(context) + '}',
        success: function (stringCustomer) {
            customer = JSON.parse(stringCustomer);
            eds.initPtGrid();
            eds.initForKnockout();
            eds.initCommission();
        }
        , error: function (e) {
            showAlertMessage("err", "Init Customer Error", 1);
        }
    });
};
//init PT Object Grid
eds.initPtGrid = function () {
    Fanex.PTEngine.CURRENT_CONTEXT = context;
    ptGrid = new Fanex.PTEngine.PTGrid(customer.ptSetting);
    var divPtGrid = document.getElementById('divPtGrid');
    ptGrid.Render(divPtGrid);

    bettingLimitGrid = new Fanex.PTEngine.BettingLimit(customer.bettingLimit);
    var bettingLimitContainer = document.getElementById('divBettingLimit');
    bettingLimitContainer.innerHTML = "";
    bettingLimitGrid.Render(bettingLimitContainer);
};
//render Commission combobox
eds.initCommission = function () {
    var commissionSetting = customer.commissionSetting;
    Fanex.PTEngine.PT_SetSelect(document.getElementById('listGameComm'), 0, Math.max(commissionSetting.discountNumberMax, commissionSetting.discountNumber), Fanex.PTEngine.COMMISSION_STEP, commissionSetting.discountNumber);
};
// check valid user input
eds.checkValidInput = function () {
    var commissionSetting = customer.commissionSetting;
    commissionSetting.discountNumber = $('#listGameComm').val();

    //get pt setting
    customer.ptSetting = ptGrid.Save();

    return true;
};
// do add new super
eds.editSuper = function () {
    if (eds.checkValidInput()) {
        //get betting limit
        if (bettingLimitGrid.Validate()) {
            customer.bettingLimit = bettingLimitGrid.Save();
        }
        else {
            return;
        }

        try { parent.parent.BlockUI(); } catch (e) { }
        try { parent.BlockUI(); } catch (e) { }
        $.ajax({
            type: "POST",
            url: "DoEditSuper",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{"stringCustomer":' + JSON.stringify(JSON.stringify(customer)) + ', "currentContext":' + JSON.stringify(context) + '}',
            success: function (result) {
                $('#divShowInfo').show();
                if (typeof result != 'undefined' && result != null) {
                    if (result.ErrorCode > 0) {
                        alert_error(result.ErrorMessage, 'err');
                        parent.scroll(0, 0);
                        window.scroll(0, 0);
                    }
                    else {
                        showAlertMessage("err", "Updated Super Successfully!", 0);
                        //redirect to current page
                        setTimeout(function () { location.href = location.href; }, 3000);
                        try {
                            parent.afterUpdate(false, 2000);
                        } catch (e) { };
                    }
                }
                else {
                    showAlertMessage("err", "Update Super Unsuccessfully!", 1);
                }
                try { parent.parent.jQuery.unblockUI(); } catch (e) { }
                try { parent.jQuery.unblockUI(); } catch (e) { }
            }
            , error: function (e) {
                $('#divShowInfo').show();
                if (e.status == '404' || e.statusText == 'Not Found') {
                    if (isInsterTwice == false) {
                        eds.editSuper();
                        isInsterTwice = true;
                    }
                    else {
                        showAlertMessage("err", "Request Error", 1);
                    }
                }
                else {
                    showAlertMessage("err", "Request Error", 1);
                }
                try { parent.parent.jQuery.unblockUI(); } catch (e) { }
                try { parent.jQuery.unblockUI(); } catch (e) { }
            }
        });
    }
    else {
        $('#divShowInfo').show();
    }
};
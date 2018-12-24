function obj(id) {
    return document.getElementById(id);
}

function showMP(refno, listType) {
    var spEvent = obj('spEvent_' + refno);
    if (spEvent.className == 'hide') {
        if (spEvent.innerHTML == "") {
            BlockUI();
            $.ajax({
                url: '../Handlers/GenMixParlay.ashx?refno=' + refno + '&listType=' + listType + '&type=1',
                dataType: 'html',
                success: function (data) {
                    spEvent.innerHTML = data;
                    spEvent.className = '';
                    $.unblockUI();
                }
                , error: function () {
                    $.unblockUI();
                }
            });

        }
        else {
            spEvent.className = '';
        }
    }
    else {
        spEvent.className = 'hide';
    }
}

function showCMP(event, refno, listType) {
    var spEvent = obj('spEvent_' + refno);
    var target = event.target ? event.target : event.srcElement;
    $(target).parents("td:first").find("div").each(function (index, element) {
        if ($(element).attr("id").indexOf("spEvent") >= 0) {
            spEvent = element;
        }
    });
    if (spEvent.className == 'hide') {
        if (spEvent.innerHTML == "") {
            BlockUI();
            $.ajax({
                url: '../Handlers/GenMixParlay.ashx?refno=' + refno + '&listType=' + listType + '&type=1',
                dataType: 'html',
                success: function (data) {
                    spEvent.innerHTML = data;
                    spEvent.className = '';

                    $(event.target).parents("tr:first").find("td").each(function (index, element) {
                        element.style.verticalAlign = "top";
                    });

                    $.unblockUI();
                }
                , error: function () {
                    $.unblockUI();
                }
            });

        }
        else {
            spEvent.className = '';
            $(event.target).parents("tr:first").find("td").each(function (index, element) {
                element.style.verticalAlign = "top";
            });
        }
    }
    else {
        spEvent.className = 'hide';
        $(event.target).parents("tr:first").find("td").each(function (index, element) {
            element.style.verticalAlign = "middle";
        });
    }
}

function showDetailsCMP(refno, title, transid, betid, custid) {
    //OpenPopup(url, title, width, height, modal, resizable, currWindow, duration, fnRecall, fnOnClose) {
    if (custid == null) {
        parent.OpenNextPopup("systemPartlay", "BetList/CombinationMPDetail.aspx?refno=" + refno + "&transid=" + transid + "&betid=" + betid, title, 720, 500, true, false, window, 0, null, null);
    }
    else {
        parent.OpenNextPopup("systemPartlay", "BetList/CombinationMPDetail.aspx?refno=" + refno + "&transid=" + transid + "&betid=" + betid + "&custid=" + custid, title, 720, 500, true, false, window, 0, null, null);
    }
}

function cmpMouseOutDetailButton(event) {
    var target = event.target ? event.target : event.srcElement;
    $(target).parents("table:first").find("div").get(0).style.backgroundPosition = "0px 0px";
}

function cmpMouseOverDetailButton(event) {
    var target = event.target ? event.target : event.srcElement;
    $(target).parents("table:first").find("div").get(0).style.backgroundPosition = "0px -14px";
}

function confirm_delete(betid, betfrom, matchid, bettype) {
    var url = window.rootUrl + 'Betlist/DeleteBet.aspx?betid=' + betid + '&betfrom=' + betfrom + '&matchid=' + matchid + '&bettype=' + bettype;
    var NewWindow = window.open(url, 'DeleteBet', 'toolbar=no,location=no,status=no,menubar=no,resizable=yes,scrollbars=yes,width=720,height=450,left=400');
    if (NewWindow) NewWindow.focus();
}

function OpenByIP(custid, fdate, ip, type) {
    //window.location= 'BetList.aspx?custid='+custid+'&fdate='+fdate+'&ip='+ip+'&bltype=3&type='+type;
    try { parent.BlockUI(); } catch (e) { }
    var urlLink = '../Handlers/GenBetList.ashx?custid=' + custid + '&requesttype=4' + '&fdate=' + fdate + '&ip=' + ip + '&type=' + type;
    GeneralAjaxCall(urlLink);
}

function viewFinanceResult(transid) {
    var width = 420;
    if (parent.ibcWnd)
        parent.ibcWnd.open('Report/ViewFinanceResult.aspx?transid=' + transid, '', 50, 100, width, 200, 'class1', 'class2');
    else
        ibcWnd.open('../Report/ViewFinanceResult.aspx?transid=' + transid, '', 50, 100, width, 200, 'class1', 'class2');
}

function ViewMainReportByBettype(fdate, tdate, bettype) {
    var urlLink = '../Handlers/GenBetList.ashx?fdate=' + fdate + '&tdate=' + tdate + '&bettype=' + bettype + '&requesttype=1';
    GeneralAjaxCall(urlLink);
}

function ViewMainReportByBettype2(fdate, tdate, bettype, page) {
    var urlLink = '../Handlers/GenBetList.ashx?fdate=' + fdate + '&tdate=' + tdate + '&bettype=' + bettype + '&requesttype=1' + '&page=' + page;
    GeneralAjaxCall(urlLink);
}

function ViewMainReportByTransID(transid) {
    var urlLink = '../Handlers/GenBetList.ashx?transid=' + transid + '&requesttype=2';
    GeneralAjaxCall(urlLink);
}

function ViewMainReportByTransIDFromCheckBet(transid, custId) {
    var urlLink = '../Handlers/GenBetList.ashx?transid=' + transid + '&custId=' + custId + '&requesttype=2';
    GeneralAjaxCall(urlLink);
}

function ViewMainReportByTransID(transid, page) {
    var urlLink = '../Handlers/GenBetList.ashx?transid=' + transid + '&requesttype=2' + '&page=' + page;
    GeneralAjaxCall(urlLink);
}

function ViewMainReport() {
    try { parent.BlockUI(); } catch (e) { }
    var requesttype = 1;
    var rdTransID = obj('rdTransID').checked;
    var rdBettype = obj('rdBetType').checked;
    if (rdTransID == true) {
        requesttype = 2;
        var transid = obj('txtTransID').value;
        if (transid != "") {
            if (isNaN(transid) == true) {
                alert('Ref No should be a number!');
                obj('txtTransID').focus();
                try { parent.jQuery.unblockUI(); } catch (e) { }
            }
            else if (IsNumeric(transid) == false) {
                alert('Ref No should be a big Integer!');
                obj('txtTransID').focus();
                try { parent.jQuery.unblockUI(); } catch (e) { }
            }
            else

                ViewMainReportByTransID(transid);
        }
        else {
            alert('Please input a Ref No!');
            obj('txtTransID').focus();
            try { parent.jQuery.unblockUI(); } catch (e) { }
        }
    }
    else {
        requesttype = 1;
        var bettype = obj('dlBettype').value;
        var fdate = obj('fdate').value;
        var tdate = obj('tdate').value;
        ViewMainReportByBettype(fdate, tdate, bettype);
        obj('txtTransID').value = '';
    }
}

function IsNumeric(sText) {
    var ValidChars = "0123456789";
    var IsNumber = true;
    var Char;


    for (i = 0; i < sText.length && IsNumber == true; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) == -1) {
            IsNumber = false;
        }
    }
    return IsNumber;

}

function ViewMainReport2(page) {
    try { parent.BlockUI(); } catch (e) { }

    var requesttype = 1;
    var rdTransID = obj('rdTransID').checked;
    var rdBettype = obj('rdBetType').checked;

    if (rdTransID == true) {
        requesttype = 2;
        var transid = obj('txtTransID').value;
        if (transid != "") {
            if (isNaN(transid) == true) {
                alert('Ref No should be a number!');
                obj('txtTransID').focus();
                try { parent.jQuery.unblockUI(); } catch (e) { }
            }
            else if (IsNumeric(transid) == false) {
                alert('Ref No should be a big Integer!');
                obj('txtTransID').focus();
                try { parent.jQuery.unblockUI(); } catch (e) { }
            }
            else
                ViewMainReportByTransID(transid, page);
        }
        else {
            alert('Please input a Ref No!');
            obj('txtTransID').focus();
            try { parent.jQuery.unblockUI(); } catch (e) { }
        }
    }
    else {
        requesttype = 1;
        var bettype = obj('dlBettype').value;
        var fdate = obj('fdate').value;
        var tdate = obj('tdate').value;
        ViewMainReportByBettype2(fdate, tdate, bettype, page);
        obj('txtTransID').value = '';
    }
}

function DoReset() {
    obj('rdBetType').checked = true;
    obj('txtTransID').value = '';
    obj('dlBettype').selectedIndex = 0;
    reset_date();
    BL_ChangeOption();
    ChangeDateTime();
}

function PreSubmit(e) {
    e = (e) ? e : window.event;
    var key = (e.keyCode) ? e.keyCode : e.which;
    if (key == 13)
        obj("btnSubmit").click();
}

String.prototype.trimLeft = function () {
    return this.replace(/^\s*/, "");
}

String.prototype.trimRight = function () {
    return this.replace(/\s*$/, "");
}

String.prototype.trim = function () {
    return this.trimRight().trimLeft();
}

function ViewBLIPReport() {
    try { parent.BlockUI(); } catch (e) { }
    var ip = obj('ip').value;
    if (ip.trim() == "") {
        showAlertMessage('errMsg', 'Please input IP!', 1);
        obj('ip').focus();
        try { parent.jQuery.unblockUI(); } catch (e) { }
    }
    else {
        $('#errMsg').html('');
        var urlLink = '../Handlers/GenBetList.ashx?ip=' + ip + '&requesttype=3' + '&fdate=' + obj('fdate').value;
        GeneralAjaxCall(urlLink);
    }
}

function ViewBLByStatement(fdate) {
    try { parent.BlockUI(); } catch (e) { }
    var userName = gup("custname");
    var transTypeId = gup("transtypeid");

    var urlLink = '../Handlers/GenBetList.ashx?requesttype=5' + '&fdate=' + fdate + "&username=" + userName + "&transtypeid=" + transTypeId;
    GeneralAjaxCall(urlLink);
}

function GeneralAjaxCall(urlLink) {
    $.ajax({
        url: urlLink + '&nocache=' + new Date().getTime(),
        dataType: 'html',
        success: function (data) {
            obj('sContent').innerHTML = data;

            $(".pagenav_ibtn").click(function () {
                if (isNumberNotNull("txtpage")) {
                    var pageIndex = $("#txtpage").val();
                    GotoPage(pageIndex);
                }
            });

            $("#divcontainer").removeClass("allhidden");
            //check heigh of table
            var tbl = $("#btable");
            if (tbl.height() < 560) {
                //$("#btable").width("905px");
                //$("#htable").width("905px");
                //$(".ui-jqgrid-sdiv").css("margin-top", "-1px");

                //                $("#tdSTitle").width("685px");
                //                $("#tdSWL").width("105px");
                //                $("#tdSEnd").width("170px");

                //                $("#tdIpTitle").width("724px");
                //                $("#tdIpTotalWin").width("100px");
                //                $("#tdIpEnd").width("165px");
                //$("#tfoot").width("905px");
            }
            //-------

            try { parent.jQuery.unblockUI(); } catch (e) { }
        }
    });
}

function GotoPage1() {
    var txtPage = obj('txtpage');
    if (txtPage) {
        GotoPage(txtPage.value);
    }
}

function GotoPage(num) {
    ViewMainReport2(num);
}

function BL_ChangeOption() {
    var rdTransID = obj('rdTransID').checked;
    var rdBettype = obj('rdBetType').checked;
    if (rdTransID == true) {
        obj('dlBettype').disabled = true;
        obj('spanFromDate').disabled = true;
        obj('spanToDate').disabled = true;
        obj('fdate_trigger').disabled = true;
        obj('tdate_trigger').disabled = true;
        obj('txtTransID').disabled = false;
    }
    else {
        obj('dlBettype').disabled = false;
        obj('spanFromDate').disabled = false;
        obj('spanToDate').disabled = false;
        obj('fdate_trigger').disabled = false;
        obj('tdate_trigger').disabled = false;
        obj('txtTransID').disabled = true;
    }
}

function ChangeDateTime() {
    var fdate = obj('fdate').value;
    var tdate = obj('tdate').value;
    var urlLink = '../Handlers/GenBetList.ashx?requesttype=6' + '&fdate=' + fdate + '&tdate=' + tdate;
    $.ajax({
        url: urlLink + '&nocache=' + new Date().getTime(),
        dataType: 'html',
        success: function (data) {
            if (data != "") {
                obj('spanBettype').innerHTML = "<select id='dlBettype'><option value='0'>All</option>" + data + "</select>";
                obj('bettypeArea').style.display = 'inline';
            }
            else
                obj('bettypeArea').style.display = 'none';
        }
    });
}

function reset_date() {
    var dayNoReset = 7;
    var fdateId = 'fdate';
    var tdateId = 'tdate';
    var td = new Date();
    var fd = new Date(td.getTime() - dayNoReset * 24 * 60 * 60 * 1000);
    document.getElementById(fdateId).value = (fd.getMonth() + 1) + "/" + fd.getDate() + "/" + fd.getFullYear();
    document.getElementById(tdateId).value = (td.getMonth() + 1) + "/" + td.getDate() + "/" + td.getFullYear();
}

function GetBetListFromCheckBet(transId, custId) {
    $('#boxheader').addClass('hidden');
    $('#maincontent_betlist').addClass('hidden');
    $('#maincontent_systemparlay').addClass('hidden');
    ViewMainReportByTransIDFromCheckBet(transId, custId);

}

function ExportBetListToExcel() {
    //try { parent.BlockUI(); } catch (e) { }
    var fdate = gup("fdate");
    var userName = gup("custname");
    var transTypeId = gup("transtypeid");

    var downloadUrl = 'ExportExcel.aspx?requesttype=5' + '&fdate=' + fdate + "&username=" + userName + "&transtypeid=" + transTypeId;
    var iframe = $('<iframe width="0px" height="0px" class="display-none"></iframe>');
    iframe.appendTo('body').attr('src', downloadUrl);
    setTimeout(function () { iframe.remove(); }, 5000);
    // GeneralAjaxCall(urlLink);
}

function showLucky3Details(transId, custId) {
    parent.OpenNextPopup(
        "Lucky3Details",
        "Lucky3/ViewDetail?TransactionId=" + transId + "&CustomerId=" + custId,
        "Lucky 3 Details",
        830,
        500,
        true,
        false,
        window,
        0,
        null,
        null);
}

// --------------------
function ViewResult(matchid, race, bettype, sporttype, refno, username, winlostdate, refno_mixparlay, league, is_outright, betid, matchcode, betcheck, transdesc) {
    var resultParam = getResultParamBySportType(matchid, race, bettype, sporttype, league, betid, matchcode, transdesc);

    resultParam = resultParam || getResultParamByBetType(matchid, race, bettype, sporttype, refno, username, winlostdate, refno_mixparlay, league, is_outright, betid);

    var defaultResultParam = {
        url: "Report/ViewResult.aspx",
        jsonParam: { matchid: matchid, bettype: bettype, betid: betid, sporttype: sporttype },
        title: "Result",
        width: 750,
        height: 300
    };

    if (resultParam) {
        $.extend(defaultResultParam, resultParam);
    }

    openResultsPopUp(defaultResultParam);
}

function getResultParamBySportType(matchid, race, bettype, sporttype, league, betid, matchcode, transdesc) {
    switch (sporttype) {
        case 5: // Tennis.
            return getTennisParam();

        case 8: // Baseball.
            return getBaseballParam();

        case 160: // Lucky 3.
            return getLucky3Param(matchid);

        case 161: // Number Game.
            return getNumberGameParam(matchid);

        case 162: // Live Casino.
            return getLiveCasinoParam(matchid, bettype, league);

        case 164: // Happy5
            return getHappy5Param(matchid);

        case 181:
        case 182:
        case 183:
        case 184:
        case 185:
            return getVirtualHRParam(matchid, race, sporttype);

        case 186: // Virtual Tennis.
            return getVirtualHRAndTennisParam(matchid, race, sporttype);

        case 211: // Allbet.
            return getAllbetParam(transdesc, matchcode, bettype);

        default:
            return undefined;
    }
}

function getResultParamByBetType(matchid, race, bettype, sporttype, refno, username, winlostdate, refno_mixparlay, league, is_outright, betid) {
    switch (true) {
        case bettype === 9 || bettype === 29: // System Parlay
            return getSystemParlayParam(matchid, refno_mixparlay, bettype, betid);

        case bettype === 31 || bettype === 32 || bettype === 31 ||
            bettype === 41 || bettype === 42 || bettype === 43: // Horse Racing.
            return getHRParam(race, winlostdate, league, betid);

        case bettype === 1701 || bettype === 1702 || bettype === 1703: // Gold Deluxe.
            return getGoldDeluxeParam(bettype, refno);

        case bettype === 1231 || bettype === 1232 || bettype === 1233: // Virtual Horse Racing.
            return getVirtualHRAndTennisParam(matchid, race, sporttype);

        default:
            return undefined;
    }
}

function getTennisParam() {
    return {
        title: "Tennis"
    };
}

function getBaseballParam() {
    return {
        title: "Baseball"
    };
}

function getLucky3Param(matchid) {
    return {
        url: "Betlist/BetListLucky3Result.aspx",
        title: "Lucky 3",
        jsonParam: { matchid: matchid }
    };
}

function getNumberGameParam(matchid) {
    return {
        url: "Betlist/BetListNumberGameResult.aspx",
        title: "Number Game",
        jsonParam: { matchid: matchid }
    };
}

function getHappy5Param(matchid) {
    return {
        url: "Betlist/BetListHappy5Result.aspx",
        title: "Happy 5",
        jsonParam: { matchid: matchid },
        width: 800,
        height: 300
    };
}

function getLiveCasinoParam(matchid, bettype, league) {
    return {
        url: "LiveCasino/Betlist/BetListLiveCasinoResult.aspx",
        title: "Live Casino",
        jsonParam: { matchid: matchid, bettype: bettype, league: league }
    };
}

function getVirtualHRParam(matchid, race, sporttype) {
    var resultParam = {
        url: "Report/GetVirtualHRPopup.aspx",
        title: "",
        jsonParam: { matchid: matchid, raceno: race, sporttype: sporttype }
    };

    switch (sporttype) {
        case 181:
            resultParam.title = "Virtual Horse Racing";
            break;
        case 182:
            resultParam.title = "Virtual Greyhound";
            break;
        case 183:
            resultParam.title = "Virtual Speedway";
            break;
        case 184:
            resultParam.title = "Virtual Motor Racing";
            break;
        case 185:
            resultParam.title = "Virtual Cycling";
            break;
    }

    return resultParam;
}

function getVirtualHRAndTennisParam(matchid, race, sporttype) {
    return {
        url: "Report/GetVirtualHRPopup.aspx",
        title: "Tennis - Virtual Tennis",
        jsonParam: { matchid: matchid, raceno: race, sporttype: sporttype }
    };
}

function getAllbetParam(transdesc, betcheck, bettype) {
    return {
        url: "Report/ViewAllBetResult.aspx",
        title: "Allbet Result",
        jsonParam: { transDesc: transdesc, matchCode: betcheck, betTypeId: bettype },
        width: 945,
        height: 500
    };
}

function getSystemParlayParam(matchid, refno, bettype, betid) {
    return {
        url: "Report/ViewMixParlayResult.aspx",
        title: "System Parlay",
        jsonParam: { refno: refno, matchid: matchid, bettype: bettype, betid: betid },
        width: 945,
        height: 500
    };
}

function getHRParam(race, winlostdate, league, betid) {
    return {
        url: "Report/HRRaceResult.aspx",
        title: "Race Result",
        jsonParam: { leagueid: league, raceno: race, eventdate: winlostdate, hrbettype: betid }
    };
}

function getGoldDeluxeParam(bettype, refno) {
    return {
        url: "Report/ViewGoldDeluxeResult.aspx",
        title: "Gold Deluxe",
        jsonParam: { transId: refno, bettype: bettype },
        width: 945,
        height: 500
    };
}

function openResultsPopUp(resultParam) {
    var url = resultParam.url + "?" + $.param(resultParam.jsonParam);
    var width = resultParam.width || 600;
    var height = resultParam.height || 500;
    var popupName = resultParam.title.replace(/\s/g, "").replace(/\//g, "");

    parent.OpenNextPopup(popupName, url, resultParam.title, width, height, true, false, window, 0, null, null);
}

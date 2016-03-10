'use strict';

function LoadOTRequest(isFirstLoad, createdBy, fromDate, toDate, status) {
    $.ajax({
        url: _global.root + 'overtime/overtimerequest',
        type: 'GET',
        data: { CreatedBy: createdBy, FromDateOT: fromDate, ToDateOT: toDate, Status: status },
        dataType: 'html',
        async: !isFirstLoad,
        beforeSend: function () {
            if (!isFirstLoad) {
                $('#OTRequest .loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#OTRequest').html(response);

            InitFormOTRequest();
            if (createdBy !== undefined) {
                $('select').material_select();
            }
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('#OTRequest .loading-process').fadeOut(300);
            }
        }
    });
}

function InitFormOTRequest() {
    initDatePicker("#OTRequest .datepicker");
    InitButtonForOTRequest();
    initAutocomplete("#OTRequest #CreatedName", "#OTRequest #CreatedBy");
    initAutocomplete("#OTRequest #Employee", "#OTRequest #EmployeeId");

    $('#OTRequest #Date').on('change', function () {
        setDateTimeRangeForOTRequest($(this).val());
    });

    LoadOTRequestDetail();
}

function LoadMemberOvertime(isFirstLoad) {
    $.ajax({
        url: _global.root + 'overtime/memberovertime',
        type: 'GET',
        dataType: 'html',
        async: !isFirstLoad,
        beforeSend: function () {
            if (!isFirstLoad) {
                $('#memberOT .loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#memberOT').append(response);

            InitFormMemberOT();
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('#memberOT .loading-process').fadeOut(300);
            }
        }
    });
}

function InitFormMemberOT() {
    initDatePicker("#memberOT .datepicker");
    InitButtonForMemberOT();
    initAutocomplete("#memberOT #CreatedName", "#memberOT #CreatedBy");
    initAutocomplete("#memberOT #Employee", "#memberOT #EmployeeId");

    $('#memberOT #Date').on('change', function () {
        setDateTimeRange($(this).val());
    });

    LoadMemberOTDetail();

    bindPendingMenu();
    bindApproveMenu();
}

function LoadMyOvertime(isFirstLoad) {
    $.ajax({
        url: _global.root + 'overtime/myovertime',
        type: 'GET',
        dataType: 'html',
        async: !isFirstLoad,
        beforeSend: function () {
            if (!isFirstLoad) {
                $('#myOT .loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#myOT').append(response);
            InitButtonForMyOT();
            LoadDetailMyOT(true);
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('#myOT .loading-process').fadeOut(300);
            }
        }
    });
}

function setDateTimeRangeForOTRequest(selectedIndex) {
    var fromDate = new Date($('#FromDateOT').val()),
        toDate = new Date($('#ToDateOT').val()),
        now = $('#DateTime-Now').val();

    fromDate = $.format.date(fromDate, 'dd MMM, yyyy');
    toDate = $.format.date(toDate, 'dd MMM, yyyy');

    switch (parseInt(selectedIndex)) {
        case 0: //default
            break;
        case 1: //today
            toDate = fromDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
        case 2: // yesterday
            toDate = fromDate = Date.parse(now).add(-1).days().toString('dd MMM, yyyy');
            break;
        case 3: //current week
            fromDate = Date.parse(now).moveToDayOfWeek(0, -1).toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
        case 4: //last week
            fromDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-7).days().toString('dd MMM, yyyy');
            toDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-1).days().toString('dd MMM, yyyy');
            break;
        case 5: //current month
            fromDate = Date.parse(now).moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
        case 6: //last month
            fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).add(-1).months().moveToLastDayOfMonth().toString('dd MMM, yyyy');
            break;
        case 7: //since last month
            fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
    }

    $('#FromDateOT').val(fromDate);
    $('#ToDateOT').val(toDate);
}

function LoadDetailTotalMembers(isFirstload, createdBy, fromDate, toDate, status) {
    LoadOTRequest(isFirstload, createdBy, fromDate, toDate, status)
    $('ul.tabs').tabs('select_tab', 'OTRequest');
}

$(document).ready(function () {
    LoadMyOvertime(true);

    if (_global.roles !== 0) {
        LoadMemberOvertime(true)
    }

    LoadOTRequest(true)

    //init material select
    $('select').material_select();
});
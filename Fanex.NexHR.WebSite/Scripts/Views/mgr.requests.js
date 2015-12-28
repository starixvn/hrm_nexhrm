/*global _global, showMessage, fadeOutMessage, tooltip*/
/*global actingButton: false, sleep: false, reloadButtonText: false*/
/*exported bindProcessedInfo*/
'use strict';

var pickerFromDate, pickerToDate;

function processRequest(command, eventElement) {
    var requestId = $(eventElement).attr('requestId');
    showMessage('Processing');
    $.post(_global.root + 'ajax/processrequest',
    { command: command, id: requestId },
        function (response) {
            fadeOutMessage();
            if (response !== true) {
                alert(response);
                return;
            }
            else {
                window.location.reload();
            }
        }
    );
}

function bindMenuPending(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            switch (key) {
                case 'Approve':
                    confirm('Are you sure you want to Approve?', function () {
                        processRequest(key, element);
                    });
                    break;
                case 'reject':
                    confirm('Are you sure you want to Reject?', function () {
                        processRequest(key, element);
                    });
                    break;
            }
        },
        items: {
            'Approve': { name: 'Approve', icon: 'edit' },
            'Reject': { name: 'Reject', icon: 'delete' }
        }
    });
}

function bindMenuApproved(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            confirm('Are you sure you want to Disapprove?', function () {
                processRequest(key, element);
            });
        },
        items: {
            'Disapprove': { name: 'Disapprove', icon: 'delete' }
        }
    });
}

function bindProcessedInfo(element, type) {
    var now = new Date(), msg;

    switch (type) {
        case 'Approve':
            msg = 'Approved by ' + _global.nickName + '( ' + now.format('dd mmm yyyy- HH') + 'h' + now.format('MM\'') + ')';
            $(element).addClass('tooltip').attr('title', msg);
            tooltip(element, true);
            break;
        case 'Reject':
            msg = 'Rejected by ' + _global.nickName + '( ' + now.format('dd mmm yyyy- HH') + 'h' + now.format('MM\'') + ')';
            $(element).addClass('tooltip').attr('title', msg);
            tooltip(element, true);
            break;
        case 'Disapprove':
            $(element).removeClass('tooltip').removeAttr('original-title');
            tooltip(element, false);
            break;
    }
}

function setDateTimeRange(selectedIndex) {
    var fromDate = new Date($('#FromDate').val()).toString('dd MMM, yyyy'),
        toDate = new Date($('#ToDate').val()).toString('dd MMM, yyyy'),
        now = $('#DateTime-Now').val();

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

    $('#FromDate').val(fromDate);
    $('#ToDate').val(toDate);
}

$(document).ready(function () {
    // menu
    bindMenuPending('.Pending');
    bindMenuApproved('.Approved');

    $('.viewhistory').click(function () {
        var url = $(this).attr('id');

        $.get(url, function (data) {
            $('#dialoghistorycontent').html(data);

            $('#dialoghistorycontent').dialog({
                modal: true,
                minHeight: 0,
                width: 1100,
                title: 'Request History'
            });
        });
    });

    $('#Date').change(function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#Date').val($('#DateOption').val());
    $('#FromDate, #ToDate').change(function () {
        $('#Date').val('0');
        $('#DateOption').attr('value', '0');
    });

    $('#Date').val($('#DateOption').val());
    setDateTimeRange($('#DateOption').val());

    pickerFromDate = initDatePicker('#FromDate');
    pickerToDate = initDatePicker('#ToDate');

    $('#submit').click(function () {
        actingButton('#submit', true);
        sleep(1000);
        reloadButtonText('#submit');
    });
});
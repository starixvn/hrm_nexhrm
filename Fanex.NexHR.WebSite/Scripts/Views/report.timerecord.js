'use strict';
/*global _global*/

var pickerFromDate, pickerToDate;

var TimeRecord = (function ($) {
    return {
        teamIds: null,
        timeRecordContent: null,
        fromDate: null,
        toDate: null,
        employeeId: null,
        employee: null,
        btnSubmit: null,
        init: function () {
            $('#checkin').attr('checked', true);
            TimeRecord.groupId = $('#GroupId');
            TimeRecord.timeRecordContent = $('#TimeRecordContent');
            TimeRecord.fromDate = $('#FromDate');
            TimeRecord.toDate = $('#ToDate');
            TimeRecord.employeeId = $('#EmployeeId');
            TimeRecord.employee = $('#Employee');
            TimeRecord.btnSubmit = $('#btnSubmit');

            TimeRecord.fillData();

            TimeRecord.btnSubmit.on('click', function () {
                TimeRecord.fillData();
            });

            TimeRecord.employee.change(function () {
                if ($.trim(TimeRecord.employee.val()).length === 0) {
                    TimeRecord.employeeId.val('');
                }

                return false;
            });
        },
        fillData: function () {
            var employeeId = TimeRecord.employeeId.val();

            var params = {
                'fromDate': TimeRecord.fromDate.val(),
                'toDate': TimeRecord.toDate.val(),
                'employeeId': ((employeeId === '0') ? null : employeeId),
                'groupIds': TimeRecord.groupId.val()
            };

            if (TimeRecord.timeRecordContent.length > 0) {
                $.get(_global.root + 'Report/TimeRecordDetail', params)
                    .done(function (content) {
                        TimeRecord.timeRecordContent.empty();
                        TimeRecord.timeRecordContent.html(content);
                });
            }
        }
    };
} (jQuery));

function setDateTimeRange(selectedIndex) {
    var fromDate = new Date($("#FromDate").val()).toString('dd MMM, yyyy'),
        toDate = new Date($("#ToDate").val()).toString('dd MMM, yyyy'),
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
    TimeRecord.init();

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
});
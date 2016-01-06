'use strict';
/*global _global*/

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
            TimeRecord.timeRecordContent = $('#TimeRecordContent');
            TimeRecord.fromDate = $('#FromDate');
            TimeRecord.toDate = $('#ToDate');
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
            var params = {
                'fromDate': TimeRecord.fromDate.val(),
                'toDate': TimeRecord.toDate.val()
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

    initDatePicker('#FromDate');
    initDatePicker('#ToDate');
});
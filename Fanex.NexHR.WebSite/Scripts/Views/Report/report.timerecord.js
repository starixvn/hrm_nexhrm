'use strict';
/*global _global, fadeOutMessage, showMessage*/

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

function processInOut(inoutType, eventElement) {
    var recordTimeId = $(eventElement).attr('data-id');
    showMessage('Processing');

    $.post(_global.root + 'Report/ProcessInOut',
    { inoutType: inoutType, recordTimeId: recordTimeId },
    function (response) {
        if (response.Code === 0) {
            TimeRecord.fillData();
        }
        else {
            alert(response.Message);
            return;
        }
    });

    fadeOutMessage();
}

function bindMenuIn(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            confirm('Are you sure you change to in?', function () {
                processInOut(0, element);
            });
        },
        items: {
            'in': { name: 'In' }
        }
    });
}

function bindMenuOut(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            confirm('Are you sure you change to out?', function () {
                processInOut(1, element);
            });
        },
        items: {
            'out': { name: 'Out'}
        }
    });
}

function bindMenuInOut(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            switch (command) {
                case 'in':
                    confirm('Are you sure you change to in?', function () {
                        processInOut(0, element);
                    });

                    break;
                case 'out':
                    confirm('Are you sure you change to out?', function () {
                        processInOut(1, element);
                    });

                    break;
            }
        },
        items: {
            'in': { name: 'In' },
            'out': { name: 'Out' }
        }
    });
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

    initDatePicker('#FromDate');
    initDatePicker('#ToDate');

    bindMenuIn('.in-action');
    bindMenuOut('.out-action');
    bindMenuInOut('.inout-action')
});
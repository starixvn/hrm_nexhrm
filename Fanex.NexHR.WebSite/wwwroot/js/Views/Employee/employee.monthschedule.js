'use strict';
/*global _global:false, formatDate, formatJsonDate */

var _event = null;

function updateShiftDialog() {
    $('#dialog').dialog({
        modal: true,
        width: 'auto',
        height: 230,
        dialogClass: 'dialog-update-shift',
        title: 'Shift on ' + _event.start.format('MM/DD/YYYY'),
        open: function () {
            $('#current').text(_event.title);
        }
    });
}

$(document).ready(function () {
    var employeeId = $('#EmployeeId').val();
    $('#defaultschedule').on('click', function () {
        document.location = _global.root + 'employee/defaultschedule/' + employeeId;
    });

    $('#update').on('click', function () {
        $.post(_global.root + 'Calendar/UpdateEmployeeShiftDate',
            { employeeId: employeeId, date: _event.start.format('MM/DD/YYYY'), shiftId: $('#ShiftId').val() },
            function () {
                $('#row' + employeeId).remove();
            });

        _event.title = $('#ShiftId option:selected').text();
        $('#calendar').fullCalendar('updateEvent', _event);
        $('#dialog').dialog('close');
    });

    $('#calendar').fullCalendar({
        theme: true,
        header: {
            left: 'prev',
            center: 'title',
            right: 'next'
        },
        editable: false,
        events: function (start, end, timezone, callback) {
            $.ajax({
                url: _global.root + 'Calendar/GetWorkingSchedule',
                dataType: 'json',
                data: {
                    date: $('#calendar').fullCalendar('getDate').format("MM/DD/YYYY"),
                    employeeId: employeeId
                },
                success: function (data) {
                    var events = [];
                    $(data).each(function (index, item) {
                        var today = formatDate(new Date(), 'dd/MM/yyyy'),
                            currentDay = formatJsonDate(item.WorkDate);

                        var cssClass = item.Title;
                        if (today === currentDay) {
                            cssClass = cssClass + ' ' + 'today';
                        }

                        events.push({
                            title: item.Title,
                            start: formatJsonDate(item.WorkDate),
                            className: cssClass
                        });
                    });
                    callback(events);
                }
            });
        },
        eventClick: function (event) {
            _event = event;
            updateShiftDialog();
            $('#calendar').fullCalendar('updateEvent', event);
        }
    });
});
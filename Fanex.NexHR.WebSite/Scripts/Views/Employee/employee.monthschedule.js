'use strict';
/*global _global:false, id:false */

var _event = null;

function updateShiftDialog() {
    $('#dialog').dialog({
        modal: false,
        width: 400,
        title: 'Shift on ' + _event.start.format('MM/DD/YYYY'),
        open: function () {
            $('#current').text(_event.title);
        }
    });
}

$(document).ready(function () {
    $('#defaultschedule').on('click', function () {
        document.location = _global.root + 'employee/defaultschedule/' + id;
    });

    $('#update').on('click', function () {
        $.post(_global.root + 'ajax/updateshift',
        { id: id, date: _event.start.format('MM/DD/YYYY'), shift: $('#shifts').val() },
        function () {
            $('#row' + id).remove();
        });

        _event.title = $('#shifts option:selected').text();
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
                url: _global.root + 'api/employee/GetWorkingSchedule',
                dataType: 'json',
                data: {
                    date: $('#calendar').fullCalendar('getDate').format("MM/DD/YYYY"),
                    id: id
                },
                success: function (data) {
                    var events = [];
                    $(data).each(function (index, item) {
                        events.push({
                            title: item.Title,
                            start: item.Start,
                            className: item.Title
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
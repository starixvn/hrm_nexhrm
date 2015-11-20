/*global _global*/
"use strict";

$(document).ready(function () {
    $('#calendar').fullCalendar({
        theme: true,
        header:
        {
            left: '',
            center: 'title',
            right: 'prev,next'
        },
        editable: true,
        events: function (start, end, timezone, callback) {
            var date = $('#calendar').fullCalendar('getDate').format('MM/DD/YYYY');

            $.ajax({
                url: '/api/employee/GetWorkingSchedule',
                dataType: 'json',
                data:
                {
                    date: date,
                    id: _global.employeeId
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
                    $('.OFF').find('.fc-event-title').addClass('OFF');
                }
            });
        }
    });
});
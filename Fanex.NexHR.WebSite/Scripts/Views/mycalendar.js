/*global _global*/
"use strict";

$(document).ready(function () {
    $('#calendar').fullCalendar({
        theme: true,
        header:
        {
            left: 'prev',
            center: 'title',
            right: 'next'
        },
        editable: false,
        events: function (start, end, timezone, callback) {
            $.ajax({
                url: '/api/employee/GetWorkingSchedule',
                dataType: 'json',
                data:
                {
                    date: $('#calendar').fullCalendar('getDate').format('MM/DD/YYYY'),
                    id: _global.employeeId
                },
                success: function (data) {
                    var events = [];
                    $(data).each(function (index, item) {
                        var today = formatDate(new Date(), 'dd/MM/yyyy'),
                            currentDay = formatDate(new Date(item.Start), 'dd/MM/yyyy');

                        var cssClass = item.Title;
                        if (today === currentDay) {
                            cssClass = cssClass + ' ' + 'today';
                        }

                        events.push({
                            title: item.Title,
                            start: item.Start,
                            className: cssClass
                        });
                    });
                    callback(events);
                }
            });
        }
    });
});
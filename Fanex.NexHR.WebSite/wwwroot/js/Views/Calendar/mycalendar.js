/*global _global, formatDate, formatJsonDate*/
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
                url: _global.root + 'Calendar/GetWorkingSchedule',
                dataType: 'json',
                method: "GET",
                data:
                {
                    date: $('#calendar').fullCalendar('getDate').format('MM/DD/YYYY'),
                    employeeId: _global.employeeId
                },
                success: function (data) {
                    var events = [];
                    $(data).each(function (index, item) {
                        var today = formatDate(new Date(), 'dd/MM/yyyy');

                        var cssClass = item.Title;
                        if (today === formatDate(item.WorkDate, 'dd/MM/yyyy')) {
                            cssClass = cssClass + ' ' + 'today';
                        }

                        events.push({
                            title: item.Title,
                            start: item.WorkDate,
                            className: cssClass
                        });
                    });
                    callback(events);
                }
            });
        }
    });
});
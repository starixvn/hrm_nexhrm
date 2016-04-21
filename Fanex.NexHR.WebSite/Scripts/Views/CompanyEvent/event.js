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
        dayClick: function () {
            //alert('ok');
        },
        eventClick: function(event) {
            alert(event.id);
        },
        events: function (start, end, timezone, callback) {
            $.ajax({
                url: _global.root + 'CompanyEvent/GetAllEvents',
                dataType: 'json',
                method: "GET",
                success: function (data) {
                    var events = [];
                    $(data).each(function (index, item) {

                        events.push({
                            id: item.Id,
                            title: item.Title,
                            start: formatJsonDate(item.FromDate),
                            end: formatJsonDate(item.ToDate)
                        });
                    });
                    callback(events);
                }
            });
        }
    });
});
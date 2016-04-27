/*global _global, formatJsonDate, _messages, isHRRole*/
"use strict";
var eventId = 0;

function deleteEvent(id) {
    confirm('Are you sure you want to delete this event?', function () {
        $.ajax({
            url: _global.root + 'CompanyEvent/Delete/' + id,
            type: 'GET',
            success: function (response) {
                if (response.IsSuccessful) {
                    $('#calendar').fullCalendar('refetchEvents');
                }
            },
            error: function () {
                alert(_messages.errorTryAgain);
            }
        });
    });
}

function processMenu(command) {
    switch (command) {
        case 'edit':
            location.href = "/CompanyEvent/Edit/" + eventId;
            break;
        case 'delete':
            deleteEvent(eventId);
            break;
    }
}

function bindingContextMenu() {
    $.contextMenu({
        selector: '.fc-content',
        trigger: 'left',
        build: function ($trigger, e) {
            return {
                callback: function (key) {
                    processMenu(key, e.currentTarget);
                },
                items: {
                    'edit': { name: 'Edit', icon: 'edit' },
                    'delete': { name: 'Delete', icon: 'delete' }
                }
            };
        }
    });
}

function initEventCalendar() {
    $('#calendar').fullCalendar({
        theme: true,
        header:
        {
            left: 'prev',
            center: 'title',
            right: 'next'
        },
        editable: false,
        dayClick: function (date) {
            if (_global.roles === 4 || isHRRole) {
                location.href = "/CompanyEvent/Create?from=" + date.format() + "&to=" + date.format();
            }
        },
        eventLimit: true,
        views: {
            agenda: {
                eventLimit: 3
            }
        },
        eventClick: function (event) {
            eventId = event.id;
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
                            start: formatJsonDate(item.FromDateTime),
                            end: formatJsonDate(item.ToDateTime),
                            createdBy: item.CreatedBy,
                            location: item.Location
                        });
                    });
                    callback(events);
                    if (_global.roles === 4 || isHRRole) {
                        bindingContextMenu();
                    }
                }
            });
        },
        eventRender: function (event, element) {
            if (event.end === null) {
                event.end = event.start;
            }
            var from = event.start.hours() + ":" + (event.start.minutes() >= 10 ? event.start.minutes() : '0' + event.start.minutes());
            var to = event.end.hours() + ":" + (event.end.minutes() >= 10 ? event.end.minutes() : '0' + event.end.minutes());
            element.tipsy({
                className: 'detail-calendar',
                gravity: 'nw',
                offset: 0,
                html: true,
                title: function () {
                    return "<table class='table-tooltip'>" +
                        "<tr>" +
                            "<td colspan='2' class='primary-color txt-bold'>" + event.title + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td colspan='2'>" + (from == "0:00" ? "" : from) + " - " + (to == "0:00" ? "" : to) + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td>" + 'Creator:' + "</td>" +
                            "<td>" + event.createdBy + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td>" + 'Location:' + "</td>" +
                            "<td>" + event.location + "</td>" +
                        "</tr>" +
                    "</table>";
                }
            });
        },
        //dayRender: function (date, cell) {
        //    if (isHRRole) {
        //        var addButton = "<a href='#' style='display: none;'>+</a>";
        //        cell.html(addButton);
        //        cell.mouseover(function () {
        //            addButton = cell.children();
        //            $(addButton).css("display", "initial");
        //        });
        //        cell.mouseleave(function () {
        //            addButton = cell.children();
        //            $(addButton).css("display", "none");
        //        });
        //    }
        //}
    });
}

$(document).ready(function () {
    initEventCalendar();
});
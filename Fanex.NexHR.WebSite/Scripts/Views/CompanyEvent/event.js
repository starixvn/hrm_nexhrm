/*global _global, formatDate, formatJsonDate*/
"use strict";
var eventId = 0;

function deleteEvent(id) {
    $.ajax({
        url: _global.root + 'CompanyEvent/Delete/' + id,
        type: 'GET',
        success: function (response) {
            if (response.IsSuccessful) {
                confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>');
                $('#calendar').fullCalendar('refetchEvents');
            }
            else {

            }
        },
        error: function () {
            alert(_messages.errorTryAgain);
        }
    });
}

function processMenu(command, eventElement) {
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
            if (_global.roles = 4) {
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
                            start: formatJsonDate(item.FromDate),
                            end: formatJsonDate(item.ToDate)
                        });
                    });
                    callback(events);
                    if (_global.roles = 4) {
                        bindingContextMenu();
                    }
                }
            });
        }
    });
}

$(document).ready(function () {
    initEventCalendar();
});
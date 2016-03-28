"use strict";
/*global eventList, gapi, isAuthenGG, moment */
/*exported initCalendar */

function customizeHeader() {
    var currentDate = new Date();
    var day = $.format.date(currentDate, "ddd");
    var dayHTML = "<div class='day'>" + day + "</div>";

    var monthYear = $.format.date(currentDate, "MMMM yyyy");
    var monthYearHTML = "<div class='monthyear'>" + monthYear + "</div>";

    $('.day').remove();
    $('.monthyear').remove();
    $('.fc-left').before(dayHTML);
    $('.fc-center').after(monthYearHTML);
}

function createEvents(googleEvents) {
    eventList = [];
    if (googleEvents.length > 0) {
        for (var i = 0; i < googleEvents.length; i++) {
            eventList.push({
                title: googleEvents[i].summary,
                start: new Date(googleEvents[i].start.dateTime),
                end: new Date(googleEvents[i].end.dateTime),
                url: googleEvents[i].htmlLink,
                creator: googleEvents[i].creator.email,
                location: googleEvents[i].organizer.displayName !== undefined ? googleEvents[i].organizer.displayName : "none"
            });
        }
    }
    else {
        var selectedDate = $('#calendar').fullCalendar('getDate');
        var from = new Date(selectedDate.toISOString());
        var to = new Date(selectedDate.add(1, 'd').toISOString());

        eventList.push({
            title: 'No meeting for today',
            start: from,
            end: to,
            url: "",
        });
    }
}

function refreshEventCalendar(from, to) {

    var request = gapi.client.calendar.events.list({
        'calendarId': 'primary',
        'timeMin': from,
        'timeMax': to,
        'showDeleted': false,
        'singleEvents': true,
    });

    request.execute(function (resp) {
        createEvents(resp.items);
        $('#calendar').fullCalendar('removeEvents');
        $('#calendar').fullCalendar('addEventSource', eventList);

        if (resp.items.length === 0) {
            $(".fc-time").css("cssText", "display: none !important;");
        }
    });
}

function refreshDay(selectedDate) {
    var day = moment(selectedDate).format("dddd");
    $('.day').html(day);

    var monthYear = moment(selectedDate).format("MMMM YYYY");
    $('.monthyear').html(monthYear);
}

function initCalendar(googleEvents) {
    $('#calendar').fullCalendar({
        defaultView: 'basicDay',
        theme: true,
        header:
        {
            left: 'prev',
            center: 'title',
            right: 'next'
        },
        events: eventList,
        timeFormat: 'HH:mm',
        eventLimit: false,
        titleFormat: 'D',
        view: {
            day: {
                eventLimit: false,
            }
        },
        eventRender: function (event, element) {
            var from = event.start.hours() + ":" + (event.start.minutes() >= 10 ? event.start.minutes() : '0' + event.start.minutes());
            var to = event.end.hours() + ":" + (event.end.minutes() >= 10 ? event.end.minutes() : '0' + event.end.minutes());
            if (event.url !== "") {
                element.tipsy({
                    className: 'detail-calendar',
                    gravity: 'se',
                    offset: 0,
                    html: true,
                    title: function () {
                        return "<table class='table-tooltip'>" +
                            "<tr>" +
                                "<td colspan='2' class='primary-color txt-bold'>" + event.title + "</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td colspan='2'>" + from + " - " + to + "</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" + 'Creator:' + "</td>" +
                                "<td>" + event.creator + "</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td>" + 'Location:' + "</td>" +
                                "<td>" + event.location + "</td>" +
                            "</tr>" +
                        "</table>";
                    }
                });
            }
        },
        eventClick: function (event) {
            if (event.url) {
                window.open(event.url);
                return false;
            }
        },
        eventAfterAllRender: function () {
            $('.fc-head .fc-day-header').hide();
        }
    });

    createEvents(googleEvents);
    $('#calendar').fullCalendar('removeEvents');
    $('#calendar').fullCalendar('addEventSource', eventList);

    $('.fc-prev-button span').click(function () {
        if (isAuthenGG) {
            var selectedDate = $('#calendar').fullCalendar('getDate');
            var previousDay = selectedDate.subtract(1, 'd');
            refreshDay(previousDay);

            var from = $('#calendar').fullCalendar('getDate').subtract(1, 'd').toISOString();
            var to = $('#calendar').fullCalendar('getDate').toISOString();

            refreshEventCalendar(from, to);
        }
    });

    $('.fc-next-button span').click(function () {
        if (isAuthenGG) {
            var selectedDate = $('#calendar').fullCalendar('getDate');
            refreshDay(selectedDate.add(1, 'd'));

            var from = $('#calendar').fullCalendar('getDate').add(1, 'd').toISOString();
            var to = $('#calendar').fullCalendar('getDate').add(2, 'd').toISOString();

            refreshEventCalendar(from, to);
        }
    });

    if (isAuthenGG) {
        $('#authenGoogle').css("display", "none");
        $('.fc-body').show();
    }
    customizeHeader();
}
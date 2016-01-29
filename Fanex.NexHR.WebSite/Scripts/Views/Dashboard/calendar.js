function initCalendar(googleEvents) {
    var eventList = [];
    if (googleEvents.length > 0) {
        for (var i = 0; i < googleEvents.length; i++) {
            eventList.push({
                title: googleEvents[i].summary,
                start: new Date(googleEvents[i].start.dateTime),
                end: new Date(googleEvents[i].end.dateTime)
            });
        };
    }
    $('#calendar').fullCalendar({
        theme: true,
        header:
        {
            left: 'prev',
            center: 'title',
            right: 'next'
        },
        events: eventList,
        eventLimit: true,
        view: {
            agenda: {
                eventLimit: 2
            }
        },
        eventRender: function (event, element) {
            var from = event.start.hours() + ":" + event.start.minutes();
            var to = event.end.hours() + ":" + event.end.minutes();
            element.tipsy({
                className: 'detail-calendar',
                gravity: $.fn.tipsy.autoNS,
                html: true,
                title: function () {
                    var object = $(this);
                    return "<table class='table-tooltip'>" +
                        "<tr>" +
                            "<td>Title</td>" +
                            "<td>" + event.title + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td>Time</td>" +
                            "<td>" + from + " - " + to + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td colspan='2'><a href='#'>More details >></a></td>" +
                        "</tr>" +
                    "</table>";
                }
            });
        },
        eventClick: function (event) {
            if (event.url) {
                window.open(event.url);
                return false;
            }
        },

    });
}
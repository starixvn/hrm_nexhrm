function initCalendar(googleEvents) {
    var eventList = [];
    if (googleEvents.length > 0) {
        for (var i = 0; i < googleEvents.length; i++) {
            eventList.push({
                title: googleEvents[i].summary,
                start: new Date(googleEvents[i].start.dateTime),
                end: new Date(googleEvents[i].end.dateTime),
                url: googleEvents[i].htmlLink,
                creator: googleEvents[i].creator.email,
                location: googleEvents[i].organizer.displayName !== undefined ? googleEvents[i].organizer.displayName:"none"
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
            var from = event.start.hours() + ":" + (event.start.minutes()>=10?event.start.minutes():'0'+event.start.minutes());
            var to = event.end.hours() + ":" + (event.end.minutes()>=10?event.end.minutes():'0'+event.end.minutes());
            element.tipsy({
                className: 'detail-calendar',
                gravity: $.fn.tipsy.autoNS,
                html: true,
                title: function () {                    
                    return "<table class='table-tooltip'>" +
                        "<tr>" +                            
                            "<td colspan='2'>" + event.title + "</td>" +
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
                        "</tr>"
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
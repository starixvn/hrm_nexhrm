var mail = "harvey.nguyen@nexcel.vn";
var googleEvents = [];

$(document).ready(function () {    
   
});
var a = 0;
function initEventTooltip(item) {    
    //var from = item.start.hours() + ":" + item.start.minutes();
    //var to = item.end.hours() + ":" + item.end.minutes();
    $('.fc-content').tipsy({
        className: 'tipsy-request',
        gravity: 'ne',
        html: true,
        title: function () {
            var object = $(this);
            return "<table class='table-tooltip'>" +
                        "<tr>" +
                            "<td>Title</td>" +
                            "<td>" + 'item.title' + a + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td>Time</td>" +
                            "<td>" + 'from' + " - " + 'to' + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td>Location</td>" +
                            "<td>" + "Meeting room A2" + "</td>" +
                        "</tr>" +
                    "</table>";
        }
    });
    a = a + 1;
}

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
        }
    });
    initEventTooltip();
}
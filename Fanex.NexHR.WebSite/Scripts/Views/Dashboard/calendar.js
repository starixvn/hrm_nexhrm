var mail = "harvey.nguyen@nexcel.vn";
$(document).ready(function () {
    $('#calendar').fullCalendar({
        googleCalendarApiKey: 'AIzaSyDJZUsIlBFo1uJvrlrKhH0pXcYwDx645PM',
        theme: true,
        header:
        {
            left: 'prev',
            center: 'title',
            right: 'next'
        },
        events: {
            googleCalendarId: mail,
            eventMouseover: function (event, jsEvent, view) {
                alert('ok');
            }
        }
    });
});
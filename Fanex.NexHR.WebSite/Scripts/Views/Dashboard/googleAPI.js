var CLIENT_ID = '671733700422-oa3ghgv4fpbb14qoj25oe25melmr1l4b.apps.googleusercontent.com';
var SCOPES = ["https://www.googleapis.com/auth/calendar.readonly"];
var eventList = [];
var isAuthenGG = false;


/**
* Check if current user has authorized this application.
*/
function checkAuth() {
    gapi.auth.authorize(
          {
              'client_id': CLIENT_ID,
              'scope': SCOPES.join(' '),
              'immediate': true
          }, handleAuthResult);
}

/**
* Handle response from authorization server.
*
* @param {Object} authResult Authorization result.
*/
function handleAuthResult(authResult) {
    if (authResult && !authResult.error) {
        isAuthenGG = true;
        loadCalendarApi();
    }
    else {
        isAuthenGG = false;
        initCalendar(eventList);
        $('.fc-body').html("");
        $('#authenGoogle').css("display", "initial");
    }
}

/**
* Initiate auth flow in response to user clicking authorize button.
*
*/
function handleAuthClick() {
    gapi.auth.authorize(
          { client_id: CLIENT_ID, scope: SCOPES, immediate: false },
          handleAuthResult);
    return false;
}

/**
* Load Google Calendar client library. List upcoming events
* once client library is loaded.
*/
function loadCalendarApi() {
    gapi.client.load('calendar', 'v3', getListEvents);
}

/**
* Print the summary and start datetime/date of the next ten events in
* the authorized user's calendar. If no events are found an
* appropriate message is printed.
*/
function getListEvents() {
    var request = gapi.client.calendar.events.list({
        'calendarId': 'primary',
        'timeMin': moment().subtract(24, 'h').toISOString(),
        'timeMax': moment().add(24, 'h').toISOString(),
        'showDeleted': false,
        'singleEvents': true,
    });

    request.execute(function (resp) {
        initCalendar(resp.items);

        if (resp.items.length == 0) {
            $(".fc-time").css("cssText", "display: none !important;");
        }
    });
}
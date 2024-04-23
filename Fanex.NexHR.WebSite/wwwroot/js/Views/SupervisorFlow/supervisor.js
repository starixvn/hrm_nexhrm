
'use strict';

function loadTeamSupervisor(isFirstLoad) {
    $.ajax({
        url: _global.root + 'SupervisorFlow/TeamSupervisor',
        dataType: 'html',
        method: "POST",
        data: {
            Keyword: $('#Keyword').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('#team .loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#team').html(response);
        },
        error: function () {
            notifyError(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('#team .loading-process').fadeOut(300);
            }
        }
    });
}

$(document).ready(function () {

    loadTeamSupervisor(true);

});
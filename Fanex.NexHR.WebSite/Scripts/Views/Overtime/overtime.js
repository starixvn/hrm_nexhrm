'use strict';

function LoadMyOvertime(isFirstLoad) {
    $.ajax({
        url: _global.root + 'overtime/myovertime',
        type: 'GET',
        dataType: 'html',
        beforeSend: function () {
            if (!isFirstLoad) {
                $('#myOT .loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#myOT').append(response);
            initSubmitButton();
            LoadDetailMyOT(true);
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('#myOT .loading-process').fadeOut(300);
            }
        }
    });
}

$(document).ready(function () {
    LoadMyOvertime(false);
});
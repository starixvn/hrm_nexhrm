'use strict';

function LoadDetailMyOT(isFirstLoad) {
    $.ajax({
        url: _global.root + 'overtime/MyOvertimeData',
        type: 'POST',
        dataType: 'html',
        data: {
            year: $('#Year').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('#myOT .loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $("#myOT #data tbody").html(response);
            floatThead('table#data');
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
};

function initSubmitButton() {
    $("#btn-submit").click(function () {
        LoadDetailMyOT(false);
    });
};
"use strict";
/*global _global, _messages */
/* exported initButtonForMyOT */
function loadDetailMyOT(isFirstLoad) {
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
            $("#dataMyOT tbody").html(response);
            //floatThead('table#dataMyOT');
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

function initButtonForMyOT() {
    $("#myOT #btn-submit").click(function () {
        loadDetailMyOT(false);
    });
}
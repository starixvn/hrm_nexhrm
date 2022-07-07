'use strict';
/*global _global, floatThead*/

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'report/LeaveSummaryDetail',
        type: 'GET',
        dataType: 'html',
        data: $('.card-container-base form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            if (data) {
                $('#leave-summary-detail').html(data);

                floatThead('table#data');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
        }
    });
}

$(document).ready(function () {
    query(true);

    $('#submit').on('click', function () {
        query(false);
    });
});
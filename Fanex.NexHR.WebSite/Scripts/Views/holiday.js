'use strict';
/*global _global:false, autoIndex: false*/
/*exported index, deleteHoliday*/

var rowIndex = 1;
function index() {
    return rowIndex++;
}

function deleteHoliday(id) {
    confirm('Are you sure you want to delete this holiday?', function () {
        $.post(_global.root + 'ajax/DeleteHoliday', { id: id }, function () {
            $('#row' + id).remove();
            autoIndex();
        });
    });
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'api/holiday/getholidays',
        data: { year: $('#year').val() },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#data tbody').children().remove();

            if (data.length > 0) {
                rowIndex = 1;
                $('#template').tmpl(data).appendTo('#data');
            } else {
                $(noInformation(6)).appendTo('#data');
            }
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
        }
    });
}

$(document).ready(function () {
    $('#submit').click(function () {
        query(false);
    });

    query(true);
});
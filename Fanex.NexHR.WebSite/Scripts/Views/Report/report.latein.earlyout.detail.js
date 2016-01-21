'use strict';
/*global _global, _messages, floatThead, noInformation, formatDate*/
/*exported index, formatMinutes*/

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function formatMinutes(minutes) {
    return (minutes / 60).toFixed(1);
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Report/LateInEarlyOutDetail',
        type: 'POST',
        dataType: 'json',
        data: $('.card-container-base .form form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('table#data tbody').children().remove();

            if (response.data.length > 0) {
                rowIndex = 1;
                $('#template').tmpl(response.data).appendTo('table#data');
            } else {
                $(noInformation(7)).appendTo('#data');
            }

            $('.loading-process').fadeOut(300);

            floatThead('table#data');
        },
        error: function () {
            $('.loading-process').fadeOut(300);
            alert(_messages.errorTryAgain);
        }
    });
}

$(document).ready(function () {
    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#Date').val($('#DateOption').val());
    $('#Date').material_select();
    setDateTimeRange($('#DateOption').val());

    query(true);

    $('#submit').on('click', function () {
        query(false);
    });
});
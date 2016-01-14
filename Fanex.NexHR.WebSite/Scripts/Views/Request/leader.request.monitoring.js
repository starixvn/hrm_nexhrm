/*global _global, setDateTimeRange*/
/*exported index, formatTotalDays*/
'use strict';

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function formatTotalDays(totalDays) {
    return totalDays.toFixed(1);
};

function query() {
    var action = $('.form form').attr('action').split('/');

    $.ajax({
        url: _global.root + 'report/' + action[action.length - 1],
        type: 'POST',
        dataType: 'json',
        data: $('.form form').serialize(),
        beforeSend: function () {
            $('.loading-process').fadeIn(300);
        },
        success: function (response) {
            if (response.data.length > 0) {
                rowIndex = 1;
                $('#data tbody').children().remove();
                $('#template').tmpl(response.data).appendTo('#data');

                $('table#data tbody tr td a').click(function () {
                    var model = $(this).closest('td').find('.request-model').serialize(),
                        controller = $(this).attr('data-controller'),
                        action = $(this).attr('data-action');

                    document.location = _global.root + controller + '/' + action + '?' + model;
                });
            } else {
                $('#data tbody').children().remove();
                $(noInformation(10)).appendTo('#data');
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
    $('#Date').val($('#DateOption').val());
    setDateTimeRange($('#DateOption').val());

    initDatePicker('#FromDate');
    initDatePicker('#ToDate');

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#DateOption').attr('value', '0');
    });

    $('#submit').click(function () {
        query();
    });

    query();
});
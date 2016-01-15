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

function query(isFirstLoad) {
    var action = $('.form form').attr('action').split('/');

    ajaxLoadingProcess();

    $.ajax({
        url: _global.root + 'report/' + action[action.length - 1],
        type: 'POST',
        dataType: 'json',
        data: $('.form form').serialize(),
        success: function (response) {
            if (response.data.length > 0) {
                rowIndex = 1;
                $('#data tbody').children().remove();
                $('#template').tmpl(response.data).appendTo('#data');

                $("table#data .DateOption").val($('#Date').val());
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

            if (isFirstLoad) {
                floatThead('table#data');
            }
        },
        error: function () {
            alert(_messages.errorTryAgain);
        }
    });
}

$(document).ready(function () {
    $('#Date').val($('#DateOption').val());
    $('#Date').material_select();
    setDateTimeRange($('#DateOption').val());

    initDatePicker('#FromDate');
    initDatePicker('#ToDate');

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select();
        $('#DateOption').attr('value', '0');
    });

    $('#submit').click(function () {
        fromDate = $("#FromDate").val();
        toDate = $("#ToDate").val();

        query(false);
    });

    query(true);
});
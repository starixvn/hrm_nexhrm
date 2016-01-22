/*global _global, setDateTimeRange, initDatePicker, floatThead, noInformation*/
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
    var action = $('.card-container-base .form form').attr('action').split('/');

    $.ajax({
        url: _global.root + 'report/' + action[action.length - 1],
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

                $.template('template', $('#template'));
                $.tmpl('template', response.data).appendTo('table#data');

                $("table#data .DateOption").val($('#Date').val());
                $('table#data tbody tr td a').click(function () {
                    var model = $(this).closest('td').find('.request-model').serialize(),
                        controller = $(this).attr('data-controller'),
                        action = $(this).attr('data-action');

                    document.location = _global.root + controller + '/' + action + '?' + model;
                });
            } else {
                $(noInformation(10)).appendTo('table#data');
            }

            floatThead('table#data');
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

function initEventForFromDateAndToDate() {
    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select();
        $('#DateOption').attr('value', '0');
    });
}

$(document).ready(function () {
    $('#Date').val($('#DateOption').val());
    $('#Date').material_select();
    setDateTimeRange($('#DateOption').val());

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    initEventForFromDateAndToDate();

    $('#submit').click(function () {
        fromDate = $("#FromDate").val();
        toDate = $("#ToDate").val();

        query(false);
    });

    query(true);
});
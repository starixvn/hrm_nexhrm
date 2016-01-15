/*global _global, setDateTimeRange, initDatePicker, floatThead*/
/*exported index, formatTotalDays*/
'use strict';

var rowIndex = 1, pickerFromDate, pickerToDate;

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

            $('.loading-process').fadeOut(300);
            floatThead('table#data');
        },
        error: function () {
            $('.loading-process').fadeOut(300);
            alert(_messages.errorTryAgain);
        }
    });
}

function initEventForFromDateAndToDate() {
    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select();
        $('#DateOption').attr('value', '0');
    });

    $('.calendar-icon.from-date').on('click', function (event) {
        event.stopPropagation();
        event.preventDefault();

        pickerFromDate.open();
    });

    $('.calendar-icon.to-date').on('click', function (event) {
        event.stopPropagation();
        event.preventDefault();

        pickerToDate.open();
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

    pickerFromDate = initDatePicker('#FromDate');
    pickerToDate = initDatePicker('#ToDate');

    initEventForFromDateAndToDate();

    $('#submit').click(function () {
        fromDate = $("#FromDate").val();
        toDate = $("#ToDate").val();

        query();
    });

    query();
});
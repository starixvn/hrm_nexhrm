'use strict';
/*global _global, _messages, floatThead, noInformation, validateFromDateAndToDate, setDateTimeRange, formatJsonDate*/
/*exported index*/

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Report/GetLateInEarlyOutDetail',
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

            if (response.length > 0) {
                rowIndex = 1;
                var tmpl = $.templates('#template'),
                    helpers = {
                        index: index,
                        formatJsonDate: formatJsonDate
                    },
                    html = tmpl.render(response, helpers);

                $(html).appendTo('table#data');
            } else {
                $(noInformation(7)).appendTo('table#data');
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

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select(); // jshint ignore:line
        $('#DateOption').attr('value', '0');
    });

    $('#Date').val($('#DateOption').val()).change();

    query(true);

    $('#submit').on('click', function () {
        if (validateFromDateAndToDate()) {
            query(false);
        }
    });
});
/*global actingButton: false, sleep: false, reloadButtonText: false, setDateTimeRange: false*/

'use strict';

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
        actingButton('#submit', true);
        sleep(1000);
        reloadButtonText('#submit');
    });

    floatThead('table#data');
});
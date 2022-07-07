/*global _global, _messages, setDateTimeRange, floatThead, validateFromDateAndToDate*/
/*exported*/
'use strict';

function query(isFirstLoad) {
    var isLeader = $('#IsLeader').val();
    $.ajax({
        url: _global.root + 'report/' + isLeader ? 'ListLeaderRequestMonitoring' : 'ListHRRequestMonitoring',
        type: 'GET',
        dataType: 'html',
        data: $('.card-container-base form.form-filter').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#request-monitoring-detail').html(data);
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

$(document).ready(function () {
    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });
    $('#Date').val($('#DateOption').val()).change();

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select(); // jshint ignore:line
        $('#DateOption').attr('value', '0');
    });

    $('#submit').click(function () {
        if (validateFromDateAndToDate()) {
            query(false);
        }
    });

    query(true);
});
'use strict';
/*global _global, _messages, floatThead, noInformation*/

function initFromMonthPicker() {
    var month = $('#FromMonth').val(),
        year = $('#FromYear').val();

    $('#fromMonthPickr').monPickr({
        showDateRange: false,
        sortDesc: true,
        today: new Date(year, month - 1, 1),
        minDate: $('#minDate').val(),
        maxDate: $('#maxDate').val(),
        afterChangeMonth: function () {
            var month = $('#fromMonthPickr').monPickr('getMonth', 'MM'),
                year = $('#fromMonthPickr').monPickr('getMonth', 'yyyy');
            $('#FromMonth').val(month);
            $('#FromYear').val(year);
        }
    });

    $('#fromMonthPickr .monpickr-prev span.ui-icon-circle-triangle-w').empty();
    $('#fromMonthPickr .monpickr-next span.ui-icon-circle-triangle-e').empty();
}


function initToMonthPicker() {
    var month = $('#ToMonth').val(),
        year = $('#ToYear').val();

    $('#toMonthPickr').monPickr({
        showDateRange: false,
        sortDesc: true,
        today: new Date(year, month - 1, 1),
        minDate: $('#minDate').val(),
        maxDate: $('#maxDate').val(),
        afterChangeMonth: function () {
            var month = $('#toMonthPickr').monPickr('getMonth', 'MM'),
                year = $('#toMonthPickr').monPickr('getMonth', 'yyyy');
            $('#ToMonth').val(month);
            $('#ToYear').val(year);
        }
    });

    $('#toMonthPickr .monpickr-prev span.ui-icon-circle-triangle-w').empty();
    $('#toMonthPickr .monpickr-next span.ui-icon-circle-triangle-e').empty();
}

function initTooltip() {
    $('.tooltip-detail').tipsy({
        className: 'tipsy-latein-earlyout-summary-detail',
        gravity: $.fn.tipsy.autoNS,
        html: true,
        title: function () {
            return $(this).closest('td').find('.latein-earlyout-summary-detail').html();
        }
    });
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Report/ListLateInEarlyOutSummary',
        type: 'POST',
        dataType: 'html',
        data: $('.card-container-base .form-filter').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#list-detail').html(response);
            initTooltip();

            floatThead('table#data');
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            $('.loading-process').fadeOut(300);
        }
    });
}

$(document).ready(function () {
    initFromMonthPicker();
    initToMonthPicker();

    query(true);

    $('#submit').on('click', function () {
        query(false);
    });
});
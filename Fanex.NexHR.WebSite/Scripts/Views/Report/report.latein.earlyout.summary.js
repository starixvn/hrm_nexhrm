'use strict';
/*global _global, _messages, floatThead*/

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
        var fromDate = new Date($('#FromYear').val(), $('#FromMonth').val() - 1, 1),
            toDate = new Date($('#ToYear').val(), $('#ToMonth').val(), 0),
            maxDate = new Date(fromDate.getFullYear() + 1, fromDate.getMonth(), 1);

        if (toDate.getTime() < fromDate.getTime()) {
            $('#toast-container').children().remove();
            Materialize.toast('<i class="mdi mdi-close-circle"></i><span class="m-l--m">From Date must be less than or equal To Date<span>', 2000);
        } else if (toDate > maxDate) {
            $('#toast-container').children().remove();
            Materialize.toast('<i class="mdi mdi-close-circle"></i><span class="m-l--m">From Date and To Date must not exceed 365 days<span>', 2000);
        }
        else {
            query(false);
        }
    });
});
/*global noInformation, _messages, _global, floatThead*/
/*exported index*/
'use strict';

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function initMonthPicker() {
    $('#monthPickr').monPickr({
        showDateRange: false,
        sortDesc: true,
        today: new Date(),
        minDate: '2010/1/1',
        maxDate: $('#maxDate').val(),
        afterChangeMonth: function () {
            var Month = $('#monthPickr').monPickr('getMonth', 'MM'),
                Year = $('#monthPickr').monPickr('getMonth', 'yyyy');
            $('#Month').val(Month);
            $('#Year').val(Year);
        }
    });

    $('#monthPickr .monpickr-prev span.ui-icon-circle-triangle-w').empty();
    $('#monthPickr .monpickr-next span.ui-icon-circle-triangle-e').empty();
}

function getRequestDetail(requestType, fromDate, toDate, createddate, fromShift, toShift, totalDays, totalMins) {
    var result = '';
    $.ajax({
        url: _global.root + 'ajax/GetRequestDetail',
        async: false,
        type: 'post',
        data: { 'requesttype': requestType, 'fromdate': fromDate, 'todate': toDate, 'createddate': createddate,
            'fromshiftCode': fromShift, 'toshiftcode': toShift, 'totaldays': totalDays, 'totalmins': totalMins
        },
        success: function (data) {
            result = data;
        }
    });

    return result;
}

function initTooltip() {
    $('.tooltip-request').tipsy({
        className: 'tipsy-request',
        gravity: 'ne',
        html: true,
        title: function () {
            var object = $(this),
                result = getRequestDetail(
                    object.attr('data-requesttype'),
                    object.attr('data-fromdate'),
                    object.attr('data-todate'),
                    object.attr('data-createddate'),
                    object.attr('data-fromshift'),
                    object.attr('data-toshift'),
                    object.attr('data-totaldays'),
                    object.attr('data-totalmins')
                );

            return "<table class='table-tooltip'>" +
                        "<tr>" +
                            "<td>Details</td>" +
                            "<td>" + result + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td>Remark</td>" +
                            "<td>" + object.attr('data-remark') + "</td>" +
                        "</tr>" +
                "</table>";
        }
    });
}

function query(isFirstLoad) {
    var $form = $('form');
    if (!$form.valid()) {
        return;
    }

    $.ajax({
        url: _global.root + 'report/timesheet',
        type: 'POST',
        dataType: 'json',
        data: $('form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('table#data tbody').children().remove();

            if (response.data && response.data.length > 0) {
                rowIndex = 1;

                var tmpl = $.templates('#template'),
                    helpers = {
                        index: index,
                        formatJsonDate: formatJsonDate,
                        totalHoursToTime: totalHoursToTime,
                        displayMinute: displayMinute
                    },
                    html = tmpl.render(response.data, helpers);

                $(html).appendTo('table#data');

                initTooltip();
            } else {
                $(noInformation(12)).appendTo('table#data');
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

$(document).ready(function () {
    initMonthPicker();

    query(true);

    $('#submit').click(function () {
        query(false);
    });
});
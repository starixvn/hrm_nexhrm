/*global actingButton, noInformation, reloadButtonText, _messages, _global*/
/*exported index*/
'use strict';

var rowIndex = 1;
function index() {
    return rowIndex++;
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

function view() {
    var $form = $('form');
    if (!$form.valid()) {
        return;
    }

    actingButton('#view', true);
    $('#nodata').remove();

    $.ajax({
        url: _global.root + 'ajax/timesheet',
        type: 'POST',
        dataType: 'json',
        data: $('form').serialize(),
        success: function (response) {
            if (response.data.length > 0) {
                rowIndex = 1;
                $('#data tbody').children().remove();
                $('#itemTemplate').tmpl(response.data).appendTo('#data');

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
            } else {
                $('#data tbody').children().remove();
                $(noInformation(12)).appendTo('#data');
            }
            reloadButtonText('#view');
        },
        error: function () {
            reloadButtonText('#view');
            alert(_messages.errorTryAgain);
        }
    });
}

$(document).ready(function () {
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

    view();

    $('#view').click(function (evt) {
        evt.preventDefault();
        view();
    });
});
/*global _global, _messages, noInformation, floatThead*/
/*exported index*/
"use strict";
var rowIndex = 1;

function index() {
    return rowIndex++;
}

function query(isFirstLoad) {
    var $form = $('form');
    if (!$form.valid()) {
        return;
    }

    //if no user is selected, clear the empId also
    if ($('#Employee').val() === '') {
        $('#EmpId').val('');
    }

    $.ajax({
        url: _global.root + 'report/GetEmployeeAttendances',
        data: $('form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('table#data tbody').children().remove();

            if (data && data.length > 0) {
                rowIndex = 1;

                var tmpl = $.templates('#template'),
                    helpers = {
                        index: index,
                        formatDate: formatDate,
                        totalHoursToTime: totalHoursToTime,
                        displayMinute: displayMinute
                    },
                    html = tmpl.render(data, helpers);

                $(html).appendTo('table#data');

                initTooltip();
            } else {
                $(noInformation(14)).appendTo('table#data');
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

function initTooltip() {
    $('.tooltip-request').tipsy({
        className: 'tipsy-request',
        gravity: 'ne',
        html: true,
        title: function () {
            var object = $(this);
            var result = getRequestDetail(
                        object.attr('data-requesttype'),
                        object.attr('data-fromdate'),
                        object.attr('data-todate'),
                        object.attr('data-createddate'),
                        object.attr('data-fromshift'),
                        object.attr('data-toshift'),
                        object.attr('data-totaldays'),
                        object.attr('data-totalmins'));

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

function getRequestDetail(requestType, fromDate, toDate, createddate, fromShift, toShift, totalDays, totalMins) {
    var result = '';
    $.ajax({
        url: _global.root + "request/GetRequestDetail",
        async: false,
        type: 'post',
        data: { "requesttype": requestType, "fromdate": fromDate, "todate": toDate, "createddate": createddate,
            "fromshiftCode": fromShift, "toshiftcode": toShift, "totaldays": totalDays, "totalmins": totalMins
        },
        success: function (data) {
            result = data;
        }
    });

    return result;
}

$(document).ready(function () {
    setDateTimeRange(1);
    initAutocomplete('#Employee', '#EmployeeId');

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#submit').on('click', function (evt) {
        evt.preventDefault();

        if (validateFromDateAndToDate()) {
            query(false);
        }
    });
    query(true);
});
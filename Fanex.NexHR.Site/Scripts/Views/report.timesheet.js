/*global actingButton, noInformation, reloadButtonText, _messages*/
/*exported index*/
"use strict";

var rowIndex = 1;
function index() {
    return rowIndex++;
}

function getRequestDetail(requestType, fromDate, toDate, createddate, fromShift, toShift, totalDays, totalMins) {
    var result = '';
    dontBlock = true;
    $.ajax({
        url: _global.root + "ajax/GetRequestDetail",
        async: false,
        type: 'post',
        data: { "requesttype": requestType, "fromdate": fromDate, "todate": toDate, "createddate": createddate,
            "fromshiftCode": fromShift, "toshiftcode": toShift, "totaldays": totalDays, "totalmins": totalMins
        },
        success: function (data) {
            result = data;
        }
    });
    dontBlock = false;
    return result;
}

function view() {
    var $form = $('form');
    if (!$form.valid()) {
        return;
    }

    actingButton('#view', true);
    $('#nodata').remove();
    $.getJSON("/api/report/timesheet", $('form').serialize(), function (response) {
        if (response.length > 0) {
            rowIndex = 1;
            $('#data tbody').children().remove();
            $('#itemTemplate').tmpl(response).appendTo('#data');

            $('.tooltip-request').tooltip({
                track: true,
                delay: 0,
                showURL: false,
                bodyHandler: function () {
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

                    return "<div class='tooltip'>" +
                        "<table class='table-tooltip checkinout'>" +
                        "<tr class='hasborder'>" +
                        "<td class='col1'>Details</td>" +
                        "<td class='col2'>" + result +
                        "</td></tr><tr><td>Remark</td><td class='col2'>" +
                        object.attr('data-remark') +
                        "</td></tr></table></div>";
                }
            });
        }
        else {
            $('#data tbody').children().remove();
            $(noInformation(12)).appendTo('#data');
        }
        reloadButtonText('#view');
    }).error(function () {
        reloadButtonText('#view');
        alert(_messages.errorTryAgain);
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
            var Month = $('#monthPickr').monPickr("getMonth", "MM");
            var Year = $('#monthPickr').monPickr("getMonth", "yyyy");
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
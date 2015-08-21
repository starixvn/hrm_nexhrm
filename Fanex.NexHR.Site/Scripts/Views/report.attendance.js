/*global _global, actingButton, noInformation, reloadButtonText, _messages*/
/*exported index*/
"use strict";

var nickName;
var rowIndex = 1;

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

    //if no user is selected, clear the empId also
    if ($('#Employee').val() === '') {
        $('#EmpId').val('');
    }
    $('#nodata').remove();
    $.getJSON("/api/report/getemployeeattendances", $('form').serialize(), function (response) {
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
            $(noInformation(14)).appendTo('#data');
        }
        reloadButtonText('#view');
    }).error(function () {
        reloadButtonText('#view');
        alert(_messages.errorTryAgain);
    });
}

function index() {
    return rowIndex++;
}

$(document).ready(function () {
    $("#Employee").autocomplete({
        source: function (request, response) {
            var term = request.term;
            var index = term.indexOf(' (');
            if (index > 0) {
                term = term.substr(0, index);
            }
            $.ajax({
                dataType: 'json',
                url: _global.root + 'api/employee/getemployees',
                data: {
                    query: term,
                    $top: 10
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.NickName + ' (' + item.FullName + ')',
                            value: item.Id
                        };
                    }));
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            nickName = ui.item.label;
            $('#EmpId').val(ui.item.value);
        },
        focus: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item.value);
            event.preventDefault();
        },
        change: function (event, ui) {
            if (ui && ui.item) {
                nickName = ui.item.label;
                $('#Employee').val(nickName);
                $('#EmpId').val(ui.item.value);
            }
            else {
                $('#EmpId').val('0');
            }
        },
        close: function () {
            $('#Employee').val(nickName);
        }
    });

    $('input[type=text]').keypress(function (evt) {
        if (evt.keyCode === 13) {
            view();
        }
    });
    $('#view').click(function (evt) {
        evt.preventDefault();
        view();
    });
    $('#exportExcel').click(function (evt) {
        evt.preventDefault();
        if ($('#Employee').val() === '') {
            $('#EmpId').val('');
        }

        window.location.href = _global.root + 'Export/Attendance?fromDate=' + $('#FromDate').val() + '&toDate=' + $('#ToDate').val() +
			'&empId=' + $('#EmpId').val('') + '&groupId=' + $('#GroupId').val() +
			'&attendanceType=' + $('#AttendanceType').val() + '&excludeUserDelete=' + $('#ExcludeUserDelete:checked').val();
    });

    view();
});


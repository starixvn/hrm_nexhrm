/*global _global, _messages, noInformation, floatThead*/
/*exported index*/
"use strict";

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function getRequestDetail(requestType, fromDate, toDate, createddate, fromShift, toShift, totalDays, totalMins) {
    var result = '';
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

    return result;
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

function initExportExcel() {
    $('#exportExcel').on('click', function (evt) {
        evt.preventDefault();
        if ($('#Employee').val() === '') {
            $('#EmpId').val('');
        }

        window.location.href = _global.root + 'Export/Attendance?fromDate=' + $('#FromDate').val() + '&toDate=' + $('#ToDate').val() +
            '&empId=' + $('#EmpId').val() + '&groupId=' + $('#GroupId').val() +
            '&attendanceType=' + $('#AttendanceType').val() + '&excludeUserDelete=' + $('#ExcludeUserDelete').prop('checked');
    });
}

function initSearchAutoComplete() {
    $("#Employee").autocomplete({
        source: function (request, response) {
            var term = request.term,
                index = term.indexOf(' (');

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
            event.preventDefault();
            $('#Employee').val(ui.item.label);
            $('#EmpId').val(ui.item.value);
        },
        change: function (event, ui) {
            if (ui && ui.item) {
                $('#Employee').val(ui.item.label);
                $('#EmpId').val(ui.item.value);
            } else {
                $('#EmpId').val('0');
            }
        }
    });
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
        url: _global.root + 'api/report/getemployeeattendances',
        data: $('form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('table#data tbody').children().remove();

            if (data.length > 0) {
                rowIndex = 1;

                $.template('template', $('#template'));
                $.tmpl('template', data).appendTo('table#data');

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

$(document).ready(function () {
    initSearchAutoComplete();
    initExportExcel();

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select();
        $('#DateOption').attr('value', '0');
    });

    $('#Date').val($('#DateOption').val()).change();

    $('#submit').on('click', function (evt) {
        evt.preventDefault();
        query(false);
    });

    query(true);
});
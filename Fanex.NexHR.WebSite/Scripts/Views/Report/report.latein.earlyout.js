'use strict';
/*global _global, _messages, floatThead, noInformation, formatDate*/
/*exported displayFormatTotalLateInEarlyOut, displayNumberHours*/

function displayFormatTotalLateInEarlyOut(minutes, times, employeeId, fromMonth, toMonth, isHightlight) {
    var year = parseInt($('#Year').val()),
        fromDate = formatDate(new Date(year, fromMonth - 1, 1), 'dd MMM, yyyy'),
        toDate = formatDate(new Date(year, toMonth, 0), 'dd MMM, yyyy'),
        url = "LateInEarlyOutDetail?employeeId=" + employeeId + "&fromDate=" + fromDate + "&toDate=" + toDate,
        result = '-';

    if (minutes !== 0) {
        var color = (times > 3 && isHightlight) ? 'red' : '';
        result = '<a style="color:' + color + '" href="' + url + '">' + (minutes / 60).toFixed(1) + ' / ' + times + '</a>';
    }

    return result;
}

function displayNumberHours(number, employeeId, fromMonth, toMonth) {
    var year = parseInt($('#Year').val()),
        fromDate = formatDate(new Date(year, fromMonth - 1, 1), 'dd MMM, yyyy'),
        toDate = formatDate(new Date(year, toMonth, 0), 'dd MMM, yyyy'),
        url = "LeaveDeductionDetail?employeeId=" + employeeId + "&fromDate=" + fromDate + "&toDate=" + toDate,
        result = '-';

    if (number !== 0) {
        result = '<a href="' + url + '">' + (number / 60).toFixed(1) + '</a>';
    }

    return result;
}

function displayOTHours(number, employeeId, fromMonth, toMonth) {
    var year = parseInt($('#Year').val()),
    fromDate = formatDate(new Date(year, fromMonth - 1, 1), 'dd MMM, yyyy'),
    toDate = formatDate(new Date(year, toMonth, 0), 'dd MMM, yyyy'),
    url = "/report/OvertimeRequest?employeeId=" + employeeId + "&fromDate=" + fromDate + "&toDate=" + toDate + "&Status=2",
    result = '-';

    if (number !== 0) {
        result = '<a href="' + url + '">' + (number).toFixed(1) + '</a>';
    }

    return result;
}

function displayBalance(balanceCurrentYear) {
    var result = '-';

    if (balanceCurrentYear != 0) {
        result = (balanceCurrentYear / 60).toFixed(1);
    }

    return result;
}

function initAddLeaveDeduction() {
    $('.addLeavededuction').click(function () {
        var employeeId = $(this).attr('data-id'),
            annualBalance = $(this).attr('data-annualbalance');

        $('#EmpId').val(employeeId);
        $('#annualBalance').text(annualBalance);

        $('#insertform').dialog({
            modal: true,
            width: 'auto',
            height: 'auto',
            open: function () {
                $('#TotalDays').val('0.5');
                initDatePicker();
            },
            show: {
                effect: "blind",
                duration: 200
            },
            hide: {
                effect: "blind",
                duration: 100
            }
        });
    });

    $('#insertform form').submit(function (evt) {
        evt.preventDefault();

        $.post(_global.root + 'AnnualLeave/AddLeaveDeduction',
            {
                empId: $('#EmpId').val(),
                totalDays: $('#TotalDays').val(),
                deductDate: $('#DeductDate').val(),
                description: $('#Description').val(),
                isAnnual: $("#IsAnnual").prop("checked")
            }, function (response) {
                if (response.Code === 0) {
                    $('#insertform').dialog('close');
                    document.location = _global.root + 'report/LateInEarlyOut';
                } else {
                    alert(response.Message);
                }
            });
    });

    $('#cancel').click(function () {
        $('#insertform').dialog('close');
    });
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Report/LateInEarlyOut',
        type: 'POST',
        dataType: 'json',
        data: $('.card-container-base .form form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('table#data tbody').children().remove();

            if (response.data && response.data.length > 0) {
                var tmpl = $.templates('#template'),
                    helpers = {
                        displayFormatTotalLateInEarlyOut: displayFormatTotalLateInEarlyOut,
                        displayNumberHours: displayNumberHours,
                        displayOTHours: displayOTHours,
                        displayBalance: displayBalance,
                        isHRRole: isHRRole
                    },
                    html = tmpl.render(response.data, helpers);

                $(html).appendTo('table#data');

                initAddLeaveDeduction();
            } else {
                $(noInformation(18)).appendTo('table#data');
            }

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
    query(true);


    $('#submit').on('click', function () {
        var year = parseInt($('#Year').val()),
            lastYear = $('table thead #last-year').text(year - 1),
            currentYear = $('table thead #current-year').text(year);

        query(false);
    });
});
'use strict';
/*global _global, _messages, floatThead, noInformation, formatDate*/
/*exported formatBalanceLastYear, displayFormatTotalLateInEarlyOut, displayNumberHours*/

function formatBalanceLastYear(balanceLastYear) {
    return (balanceLastYear / 60).toFixed(1);
}

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
        url = "LateInEarlyOutDetail?employeeId=" + employeeId + "&fromDate=" + fromDate + "&toDate=" + toDate,
        result = '-';

    if (number !== 0) {
        result = '<a href="' + url + '">' + (number / 60).toFixed(1) + '</a>';
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

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Report/LateInEarlyOutSummary',
        type: 'POST',
        dataType: 'json',
        data: $('.card-container-base .form form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#data tbody').children().remove();

            if (response.data.length > 0) {
                $('#template').tmpl(response.data).appendTo('#data');

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
            } else {
                $(noInformation(18)).appendTo('#data');
            }

            $('.loading-process').fadeOut(300);

            floatThead('table#data');
        },
        error: function () {
            $('.loading-process').fadeOut(300);
            alert(_messages.errorTryAgain);
        }
    });
}

$(document).ready(function () {
    $('#insertform form').submit(function (evt) {
        evt.preventDefault();

        $.post(_global.root + 'ajax/AddLeaveDeduction',
            {
                empId: $('#EmpId').val(),
                totalDays: $('#TotalDays').val(),
                deductDate: $('#DeductDate').val(),
                description: $('#Description').val(),
                isAnnual: $("#IsAnnual").attr("checked") === "checked"
            }, function (response) {
                if (response.Code === 0) {
                    $('#insertform').dialog('close');
                    document.location = _global.root + 'report/LateInEarlyOutSummary';
                } else {
                    alert(response.Message);
                }
            });
    });

    $('#cancel').click(function () {
        $('#insertform').dialog('close');
    });

    query(true);

    $('#submit').on('click', function () {
        query(false);
    });
});
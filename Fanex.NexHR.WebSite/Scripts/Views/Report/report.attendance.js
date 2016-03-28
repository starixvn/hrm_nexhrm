/*global _global, _messages, floatThead, validateFromDateAndToDate, setDateTimeRange, initAutocomplete*/
/*exported*/
"use strict";

function initPagination() {
    var currentPage = $('table#data #pagination #CurrentPage').val(),
        itemsOnPage = $('table#data #pagination #ItemsPerPage').val(),
        totalItem = $('table#data #pagination #TotalItem').val();

    currentPage = currentPage ? parseInt(currentPage) : 1;
    itemsOnPage = itemsOnPage ? parseInt(itemsOnPage) : 1;
    totalItem = totalItem ? parseInt(totalItem) : 1;

    $('table#data #pagination #numberlist').pagination({
        items: totalItem,
        itemsOnPage: itemsOnPage,
        currentPage: currentPage,
        displayedPages: 3,
        prevText: '<i class="mdi mdi-chevron-left"></i>',
        nextText: '<i class="mdi mdi-chevron-right"></i>',
        ellipsePageSet: false,
        cssStyle: 'compact-theme',
        onPageClick: function (pageNumber) {
            goToPage(pageNumber);
        }
    });
}

function getRequestDetail(requestType, fromDate, toDate, createddate, fromShift, toShift, totalDays, totalMins) {
    var result = '';
    $.ajax({
        url: _global.root + "request/GetRequestDetail",
        async: false,
        type: 'post',
        data: {
            "requesttype": requestType, "fromdate": fromDate, "todate": toDate, "createddate": createddate,
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

function query(isFirstLoad) {
    var $form = $('form.form-filter');
    if (!$form.valid()) {
        return;
    }

    //if no user is selected, clear the empId also
    if ($('#Employee').val() === '') {
        $('#EmpId').val('');
    }

    $.ajax({
        url: _global.root + 'report/ListAttendance',
        data: $('form.form-filter').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#attendance-detail').html(data);

            if ($('table#data #pagination').length > 0) {
                initTooltip();
                initPagination();
                floatThead('table#data');
            }
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

function goToPage(pageNumber) { // jshint ignore:line
    $('form.form-filter #CurrentPage').val(pageNumber);
    query(false);
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

$(document).ready(function () {
    initAutocomplete('#Employee', '#EmployeeId');
    initExportExcel();

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select(); // jshint ignore:line
        $('#DateOption').attr('value', '0');
    });

    $('#Date').val($('#DateOption').val()).change();

    $('#submit').on('click', function () {
        if (validateFromDateAndToDate()) {
            $('form.form-filter #CurrentPage').val(1);
            query(false);
        }
    });

    query(true);
});
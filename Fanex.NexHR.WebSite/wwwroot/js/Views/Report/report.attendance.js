/*global _global, _messages, floatThead, validateFromDateAndToDate, setDateTimeRange, initAutocomplete*/
/*exported*/
"use strict";

var sorts = [];

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
        $('#EmployeeId').val('');
    }

    $.ajax({
        url: _global.root + 'report/ListAttendance',
        data: $('form.form-filter').serialize() + '&sort=' + JSON.stringify(sorts),
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
                $('#attendance-detail').parents().addClass('table-success'); //use this class to style table
            }
            $('#attendance-detail').parents().addClass('table-fail'); //use this class to style table
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
            $('#EmployeeId').val('');
        }

        window.location.href = _global.root + 'Export/Attendance?fromDate=' + $('#FromDate').val() + '&toDate=' + $('#ToDate').val() +
            '&empId=' + $('#EmployeeId').val() + '&groupId=' + $('#GroupId').val() +
            '&attendanceType=' + $('#AttendanceType').val() + '&excludeUserDelete=' + $('#ExcludeUserDelete').prop('checked') + '&sort=' + JSON.stringify(sorts);
    });
}

function eventSorting() {
    $('html').on('click', 'th.col-sorter', function () {
        var $this = $(this).find('div.sorter', 'th.col-sorter'),
            property = $this.attr('data-property'),
            direction, index;

        index = sorts.map(function (e) { return e.Property; }).indexOf(property);

        if ($this.hasClass('desc')) {
            $this.removeClass('desc').addClass('asc');
            direction = 'asc';
        } else if ($this.hasClass('asc')) {
            $this.removeClass('asc').addClass('desc');
            direction = 'desc';
        } else {
            refreshSorting();
            $this.addClass('desc');
            direction = 'desc';
        }

        sorts.shift();
        sorts.push({ Property: property, Direction: direction });

        query(false);
    });
}

function refreshSorting() {
    sorts = [];

    $('th.col-sorter').each(function () {
        $(this).find('div.sorter').removeClass('asc').removeClass('desc');
    });
}

$(document).ready(function () {
    initAutocomplete('#Employee', '#EmployeeId', false);
    initExportExcel();

    eventSorting();

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
            refreshSorting();
            $('form.form-filter #CurrentPage').val(1);
            query(false);
        }
    });

    query(true);
});
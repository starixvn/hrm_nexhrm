/*global _global, _messages, noInformation, floatThead*/
/*exported index*/
"use strict";

var rowIndex = 1;
var pagingOption = {
    currentPage: 1,
    itemsPerPage: 50
};

function index() {
    var currentPage = parseInt($('#CurrentPage').val()),
        itemsPerPage = parseInt($('#ItemsPerPage').val());

    return (itemsPerPage * (currentPage - 1) + rowIndex++);
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

function fillData(response) {
    rowIndex = 1;

    var tmpl = $.templates('#template'),
        helpers = {
            index: index,
            formatDate: formatDate,
            totalHoursToTime: totalHoursToTime,
            displayMinute: displayMinute
        },
        html = tmpl.render(response.EmployeeAttendances, helpers);

    $(html).appendTo('table#data');

    var pager = $('#pager').html();
    $('table#data tfoot').html(pager);

    var currentPage = parseInt($('#CurrentPage').val()),
        itemsPerPage = parseInt($('#ItemsPerPage').val()),
        preNumberPages = parseInt(response.TotalItems / itemsPerPage),
        totalPages = response.TotalItems % itemsPerPage == 0 ? preNumberPages : preNumberPages + 1;

    $('table#data #pagination #numberlist').twbsPagination({
        totalPages: totalPages,
        startPage: currentPage,
        visiblePages: 5,
        initiateStartPageClick: false,
        href: 'javascript:;',
        first: '<i class="mdi mdi-chevron-double-left"></i>',
        prev: '<i class="mdi mdi-chevron-left"></i>',
        next: '<i class="mdi mdi-chevron-right"></i>',
        last: '<i class="mdi mdi-chevron-double-right"></i>',
        activeClass: 'active',
        onPageClick: function (event, page) {
            $('#CurrentPage').val(page);
            query();
        }
    });

    initTooltip();

    var from = (parseInt(currentPage) - 1) * parseInt(itemsPerPage) + 1,
        to = from + response.EmployeeAttendances.length - 1;
    var detailtext = 'Showing <strong>' + from + '</strong> to <strong> ' + to + '</strong> of <strong>' + response.TotalItems + '</strong> entries';
    $('table#data #pagination #detailtext').html(detailtext);
};

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

            if (data != null && data.EmployeeAttendances != null && data.EmployeeAttendances.length > 0) {
                fillData(data);

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

    $('#CurrentPage').val(pagingOption.currentPage);
    $('#ItemsPerPage').val(pagingOption.itemsPerPage);
    $('#EntryId').val(pagingOption.itemsPerPage);
    $('#GetAll').val(false);

    initAutocomplete('#Employee', '#EmployeeId');
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

    $('#submit').on('click', function () {
        if (validateFromDateAndToDate()) {
            $('#CurrentPage').val(pagingOption.currentPage);
            query(false);
        }
    });

    $('#EntryId').on('change', function (evt) {
        evt.preventDefault();
        $('#ItemsPerPage').val($('#EntryId').val());
        $('#CurrentPage').val(pagingOption.currentPage);
        query(false);
    });

    query(true);
});
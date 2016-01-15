'use strict';
/*global _global, setDateTimeRange, floatThead*/
/*exported index, formatJsonDateToDetailFormatedDate*/

var rowIndex = 1, pickerFromDate, pickerToDate;

function index() {
    return rowIndex++;
}

function formatJsonDateToDetailFormatedDate(jsonDate) {
    var date = new Date(parseInt(jsonDate.substr(6))),
        h = date.getHours(),
        m = date.getMinutes();

    if (h < 10) {
        h = '0' + h;
    }

    if (m < 10) {
        m = '0' + m;
    }

    date = $.format.date(date, 'E, dd MMM, yyyy');

    return date + ' - ' + h + 'h' + m + '\'';
};

function fillData(response) {
    rowIndex = (response.pagerOption.CurrentPage - 1) * response.pagerOption.ItemsPerPage + 1;

    $('#template').tmpl(response.data).appendTo('#data');

    var pager = $('#pager').html();
    $('#data tfoot').html(pager);

    $('#data #pagination #numberlist').twbsPagination({
        totalPages: response.pagerOption.TotalPage,
        startPage: response.pagerOption.CurrentPage,
        visiblePages: 5,
        initiateStartPageClick: false,
        href: 'javascript:;',
        first: '',
        prev: '<i class="mdi-image-navigate-before"></i>',
        next: '<i class="mdi-image-navigate-next"></i>',
        last: '',
        onPageClick: function (event, page) {
            $('#form #CurrentPage').val(page);
            query();
        }
    });

    var from = (response.pagerOption.CurrentPage - 1) * response.pagerOption.ItemsPerPage + 1,
        to = from + response.data.length - 1;
    var detailtext = 'Showing <strong>' + from + '</strong> to <strong> ' + to + '</strong> of <strong>' + response.pagerOption.TotalItem + '</strong> entries';
    $('#data #pagination #detailtext').html(detailtext);
}

function query() {
    $.ajax({
        url: _global.root + 'Report/MyTimeRecord',
        type: 'POST',
        dataType: 'json',
        data: $('#form').serialize(),
        beforeSend: function () {
            $('.loading-process').fadeIn(300);
        },
        success: function (response) {
            $('#data tbody').children().remove();
            $('#data tfoot').children().remove();

            if (response.data.length > 0) {
                $('#datatable_length').removeClass('hide');
                fillData(response);
            } else {
                $('#datatable_length').removeClass('hide').addClass('hide');
                $(noInformation(6)).appendTo('#data');
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

function initEventForFromDateAndToDate() {
    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select();
        $('#DateOption').attr('value', '0');
    });

    $('.calendar-icon.from-date').on('click', function (event) {
        event.stopPropagation();
        event.preventDefault();

        pickerFromDate.open();
    });

    $('.calendar-icon.to-date').on('click', function (event) {
        event.stopPropagation();
        event.preventDefault();

        pickerToDate.open();
    });
}

$(document).ready(function () {
    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#submit').on('click', function () {
        $('#form #CurrentPage').val(1);
        query();
    });

    $('#Date').val($('#DateOption').val());
    setDateTimeRange($('#DateOption').val());

    pickerFromDate = initDatePicker('#FromDate');
    pickerToDate = initDatePicker('#ToDate');

    initEventForFromDateAndToDate();

    $('#datatable_length #ItemsPerPage').on('change', function () {
        $('#form #CurrentPage').val(1);
        $('#form #ItemsPerPage').val($(this).val());
        query();
    });

    query();
});
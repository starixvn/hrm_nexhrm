'use strict';
/*global _global, _messages, setDateTimeRange, floatThead, noInformation*/
/*exported index, formatJsonDateToDetailFormatedDate*/

var rowIndex = 1;

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

    $.template('template', $('#template'));
    $.tmpl('template', response.data).appendTo('table#data');

    var pager = $('#pager').html();
    $('table#data tfoot').html(pager);

    $('table#data #pagination #numberlist').twbsPagination({
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
    $('table#data #pagination #detailtext').html(detailtext);
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Report/MyTimeRecord',
        type: 'POST',
        dataType: 'json',
        data: $('.card-container-base #form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
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

function initEventForFromDateAndToDate() {
    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select();
        $('#DateOption').attr('value', '0');
    });
}

$(document).ready(function () {
    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#Date').val($('#DateOption').val()).change();

    initEventForFromDateAndToDate();

    $('#submit').on('click', function () {
        $('#form #CurrentPage').val(1);
        query(false);
    });

    $('#datatable_length #ItemsPerPage').on('change', function () {
        $('#form #CurrentPage').val(1);
        $('#form #ItemsPerPage').val($(this).val());
        query(false);
    });

    query(true);
});
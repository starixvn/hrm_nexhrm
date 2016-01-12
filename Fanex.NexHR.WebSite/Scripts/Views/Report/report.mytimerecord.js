'use strict';
/*global _global:false, setDateTimeRange:false*/

var rowIndex = 1,
    params = {
        fromDate: null,
        toDate: null,
        itemsPerPage: null,
        currentPage: null
    };

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
    floatThead('table#data');

    rowIndex = (response.pagerOption.CurrentPage - 1) * response.pagerOption.ItemsPerPage + 1;

    $('#template').tmpl(response.data).appendTo('#data');

    var pager = $('#pager').html();
    $('#data tfoot').html(pager);

    $('#data #pagination').twbsPagination({
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
            params.currentPage = page;
            query();
        }
    });

    $('#data #ItemsPerPage').val(response.pagerOption.ItemsPerPage);

    $('#data #ItemsPerPage').on('change', function () {
        query();
    });
}

function query() {
    params.fromDate = $('#FromDate').val();
    params.toDate = $('#ToDate').val();
    params.itemsPerPage = $('#ItemsPerPage').val();

    $.ajax({
        url: _global.root + 'Report/MyTimeRecord',
        type: 'POST',
        dataType: 'json',
        data: params,
        beforeSend: function () {
            $('.loading-process').fadeIn(200);
        },
        success: function (response) {
            $('#data tbody').children().remove();
            $('#data tfoot').children().remove();

            if (response.data.length > 0) {
                fillData(response);
            } else {
                $(noInformation(6)).appendTo('#data');
            }

            $('.loading-process').fadeOut(200);
        }
    });
}

$(document).ready(function () {
    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#DateOption').attr('value', '0');
    });

    $('#btnSubmit').on('click', function () {
        query();
    });

    $('#Date').val($('#DateOption').val());
    setDateTimeRange($('#DateOption').val());

    initDatePicker('#FromDate');
    initDatePicker('#ToDate');

    query();
});
'use strict';
/*global _global*/
/*exported index*/

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

    $('#template').tmpl(response.data).appendTo('#data');

    bindMenuIn('.in-action');
    bindMenuOut('.out-action');
    bindMenuInOut('.inout-action');

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
        url: _global.root + 'Report/TimeRecord',
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
        }
    });
}

function processInOut(inoutType, eventElement) {
    $.post(_global.root + 'Report/ProcessInOut',
    { inoutType: inoutType, recordTimeId: $(eventElement).attr('data-id') },
    function (response) {
        if (response.Code === 0) {
            query();
        } else {
            alert(response.Message);
            return;
        }
    });
}

function bindMenuIn(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            confirm('Are you sure you change to in?', function () {
                processInOut(0, element);
            });
        },
        items: {
            'in': { name: 'In' }
        }
    });
}

function bindMenuOut(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            confirm('Are you sure you change to out?', function () {
                processInOut(1, element);
            });
        },
        items: {
            'out': { name: 'Out'}
        }
    });
}

function bindMenuInOut(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            switch (command) {
                case 'in':
                    confirm('Are you sure you change to in?', function () {
                        processInOut(0, element);
                    });

                    break;
                case 'out':
                    confirm('Are you sure you change to out?', function () {
                        processInOut(1, element);
                    });

                    break;
            }
        },
        items: {
            'in': { name: 'In' },
            'out': { name: 'Out' }
        }
    });
}

$(document).ready(function () {
    $('#Date').change(function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#Date').val($('#DateOption').val());
    $('#FromDate, #ToDate').change(function () {
        $('#Date').val('0');
        $('#DateOption').attr('value', '0');
    });

    $('#Date').val($('#DateOption').val());
    setDateTimeRange($('#DateOption').val());

    initDatePicker();

    $('#submit').on('click', function () {
        $('#form #CurrentPage').val(1);
        query();
    });

    $('#datatable_length #ItemsPerPage').on('change', function () {
        $('#form #ItemsPerPage').val($(this).val());
        $('#form #CurrentPage').val(1);
        query();
    });

    query();
});
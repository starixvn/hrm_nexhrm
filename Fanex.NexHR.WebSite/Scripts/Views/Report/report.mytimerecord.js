'use strict';
/*global _global, _messages, setDateTimeRange, floatThead*/
/*exported*/

function initPagination() {
    var totalPages = $('table#data #pagination #TotalPages').val(),
        startPage = $('table#data #pagination #CurrentPage').val();

    totalPages = totalPages ? parseInt(totalPages) : 0;
    startPage = startPage ? parseInt(startPage) : 1;

    $('table#data #pagination #numberlist').twbsPagination({
        totalPages: totalPages,
        startPage: startPage,
        visiblePages: 5,
        initiateStartPageClick: false,
        href: 'javascript:;',
        first: '<i class="mdi mdi-chevron-double-left"></i>',
        prev: '<i class="mdi mdi-chevron-left"></i>',
        next: '<i class="mdi mdi-chevron-right"></i>',
        last: '<i class="mdi mdi-chevron-double-right"></i>',
        onPageClick: function (event, page) {
            $('form.form-filter #CurrentPage').val(page);
            query(false);
        }
    });
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Report/MyTimeRecordDetail',
        type: 'GET',
        dataType: 'html',
        data: $('.card-container-base form.form-filter').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#time-record-detail').html(data);

            if ($('table#data #pagination').length > 0) {
                initPagination();
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
        $('form.form-filter #CurrentPage').val(1);
        $('#datatable_length #ItemsPerPage').prop('selectedIndex', 0);
        $('#datatable_length #ItemsPerPage').material_select();
        $('form.form-filter #ItemsPerPage').val($('#datatable_length #ItemsPerPage').val());

        if (validateFromDateAndToDate()) {
            query(false);
        }
    });

    query(true);
});
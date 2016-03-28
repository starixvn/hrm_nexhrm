'use strict';
/*global _global, _messages, setDateTimeRange, floatThead, validateFromDateAndToDate*/
/*exported */

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Report/ListMyTimeRecord',
        type: 'POST',
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

function goToPage(pageNumber) {
    $('form.form-filter #CurrentPage').val(pageNumber);
    query(false);
}

function initPagination() { // jshint ignore:line
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

function initEventForFromDateAndToDate() {
    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select(); // jshint ignore:line
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
        if (validateFromDateAndToDate()) {
            $('form.form-filter #CurrentPage').val(1);
            query(false);
        }
    });

    query(true);
});
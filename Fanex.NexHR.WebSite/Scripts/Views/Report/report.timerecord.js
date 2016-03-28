'use strict';
/*global _global, _messages, floatThead, setDateTimeRange, validateFromDateAndToDate*/
/*exported*/

function processInOut(inoutType, eventElement) {
    $.post(_global.root + 'ajax/ProcessInOut',
    { inoutType: inoutType, recordTimeId: $(eventElement).attr('data-id') },
    function (response) {
        if (response.IsSuccessful) {
            query(false);   // jshint ignore:line
        } else {
            alert(response.Message);
            return;
        }
    });
}

function bindMenuIn() {
    $('table#data tbody td a.in-action').on('mouseup', function () {
        var $this = $(this);

        $.contextMenu({
            selector: '.in-action',
            trigger: 'left',
            build: function () {
                return {
                    callback: function () {
                        confirm('Are you sure you change to in?', function () {
                            processInOut(0, $this);
                        });
                    },
                    items: {
                        'in': { name: 'In' }
                    }
                };
            }
        });
    });
}

function bindMenuOut() {
    $('table#data tbody td a.out-action').on('mouseup', function () {
        var $this = $(this);

        $.contextMenu({
            selector: '.out-action',
            trigger: 'left',
            build: function () {
                return {
                    callback: function () {
                        confirm('Are you sure you change to out?', function () {
                            processInOut(1, $this);
                        });
                    },
                    items: {
                        'out': { name: 'Out' }
                    }
                };
            }
        });
    });
}

function bindMenuInOut() {
    $('table#data tbody td a.inout-action').on('mouseup', function () {
        var $this = $(this);

        $.contextMenu({
            selector: '.inout-action',
            trigger: 'left',
            build: function () {
                return {
                    callback: function (key) {
                        switch (key) {
                            case 'in':
                                confirm('Are you sure you change to in?', function () {
                                    processInOut(0, $this);
                                });

                                break;
                            case 'out':
                                confirm('Are you sure you change to out?', function () {
                                    processInOut(1, $this);
                                });

                                break;
                        }
                    },
                    items: {
                        'in': { name: 'In' },
                        'out': { name: 'Out' }
                    }
                };
            }
        });
    });
}

function query(isFirstLoad) {   // jshint ignore:line
    $.ajax({
        url: _global.root + 'Report/ListTimeRecord',
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
                bindMenuIn();
                bindMenuOut();
                bindMenuInOut();
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
            $('form.form-filter #CurrentPage').val(pageNumber);
            query(false);
        }
    });
}

$(document).ready(function () {
    $('#Date').change(function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#FromDate, #ToDate').change(function () {
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
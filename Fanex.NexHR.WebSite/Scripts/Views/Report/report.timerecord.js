'use strict';
/*global _global, _messages, floatThead*/
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
        url: _global.root + 'Report/TimeRecordDetail',
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
                bindMenuIn();
                bindMenuOut();
                bindMenuInOut();
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

function processInOut(inoutType, eventElement) {
    $.post(_global.root + 'Report/ProcessInOut',
    { inoutType: inoutType, recordTimeId: $(eventElement).attr('data-id') },
    function (response) {
        if (response.Code === 0) {
            query(false);
        } else {
            alert(response.Message);
            return;
        }
    });
}

function bindMenuIn() {
    $('table#data tbody td a.in-action').on('mouseup', function (e) {
        var $this = $(this);

        $.contextMenu({
            selector: '.in-action',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key, options) {
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
    $('table#data tbody td a.out-action').on('mouseup', function (e) {
        var $this = $(this);

        $.contextMenu({
            selector: '.out-action',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key, options) {
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
    $('table#data tbody td a.inout-action').on('mouseup', function (e) {
        var $this = $(this);

        $.contextMenu({
            selector: '.inout-action',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key, options) {
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

$(document).ready(function () {
    $('#Date').change(function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#FromDate, #ToDate').change(function () {
        $('#Date').val('0');
        $('#Date').material_select();
        $('#DateOption').attr('value', '0');
    });

    $('#Date').val($('#DateOption').val()).change();

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
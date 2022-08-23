'use strict';
/*global _global, _messages, floatThead, setDateTimeRange, validateFromDateAndToDate*/
/*exported*/

var recordTimeIdToChange = 0;

function processInOut(inoutType, eventElement) {
    $.post(_global.root + 'ajax/ProcessInOut',
    { inoutType: inoutType, recordTimeId: recordTimeIdToChange },
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
        //var $this = $(this);
        recordTimeIdToChange = $(this).data("id");

        $.contextMenu({
            selector: '.in-action',
            trigger: 'left',
            build: function () {
                return {
                    callback: function (key) {
                        switch (key) {
                            case 'in':
                                confirm('Are you sure you change to in?', function () {
                                    processInOut(0, recordTimeIdToChange);
                                });

                                break;
                            case 'none':
                                confirm('Are you sure you change to none?', function () {
                                    processInOut(null, recordTimeIdToChange);
                                });

                                break;
                        }
                    },
                    items: {
                        'in': { name: 'In' },
                        'none': { name: 'None' }
                    }
                };
            }
        });
    });
}

function bindMenuOut() {
    $('table#data tbody td a.out-action').on('mouseup', function () {
        //var $this = $(this);
        recordTimeIdToChange = $(this).data("id");

        $.contextMenu({
            selector: '.out-action',
            trigger: 'left',
            build: function () {
                return {
                    callback: function (key) {
                        switch (key) {
                            case 'out':
                                confirm('Are you sure you change to out?', function () {
                                    processInOut(1, recordTimeIdToChange);
                                });

                                break;
                            case 'none':
                                confirm('Are you sure you change to none?', function () {
                                    processInOut(null, recordTimeIdToChange);
                                });

                                break;
                        }
                    },
                    items: {
                        'out': { name: 'Out' },
                        'none': { name: 'None' }
                    }
                };
            }
        });
    });
}

function bindMenuInOut() {
    $('table#data tbody td a.inout-action').on('mouseup', function () {
        recordTimeIdToChange = $(this).data("id");

        $.contextMenu({
            selector: '.inout-action',
            trigger: 'left',
            build: function () {
                return {
                    callback: function (key) {
                        switch (key) {
                            case 'in':
                                confirm('Are you sure you change to in?', function () {
                                    processInOut(0, recordTimeIdToChange);
                                });

                                break;
                            case 'out':
                                confirm('Are you sure you change to out?', function () {
                                    processInOut(1, recordTimeIdToChange);
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
    const queryString = window.location.search;

    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }

    var data = $('.card-container-base form.form-filter').serialize();
    var selectedTeam = $(".select-teamIds .select-dropdown").val();
    var selectedDateOption = $(".select--daterange .select-dropdown").val();
    var url = _global.root + 'Report/ListTimeRecord?' + data;
    if (!isFirstLoad) {
        var orgUrl = _global.root + 'Report/TimeRecord?' + data + '&Team=' + selectedTeam + '&DateOptionValue=' + selectedDateOption;
        window.history.replaceState(null, null, orgUrl);
    }

    $.ajax({
        url: url,
        type: 'POST',
        dataType: 'html',
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

function initDataFirstQuery(queryString) {
    var urlParams = new URLSearchParams(queryString);
    var employeeParam = urlParams.get('Employee');
    var employeeIdParam = urlParams.get('EmployeeId');
    var dateOptionParam = urlParams.get('DateOption');
    var dateOptionValueParam = urlParams.get('DateOptionValue');
    var teamIdsParam = urlParams.get('TeamIds');
    var teamParam = urlParams.get('Team');
    var fromDateParam = urlParams.get('FromDate');
    var toDateParam = urlParams.get('ToDate');
    var currentPageParam = urlParams.get('CurrentPage');
    var itemsPerPageParam = urlParams.get('ItemsPerPage');

    var teamParam = teamParam;
    $("#Employee").val(employeeParam);
    $("#TeamIds").val(teamIdsParam);
    $(".select-teamIds .select-dropdown").val(teamParam);
    $("#EmployeeId").val(employeeIdParam);
    $("#DateOption").val(dateOptionParam);
    $(".select--daterange .select-dropdown").val(dateOptionValueParam);
    $("#FromDate").val(fromDateParam);
    $("#ToDate").val(toDateParam);
    $("#CurrentPage").val(currentPageParam);
    $("#ItemsPerPage").val(itemsPerPageParam);
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
/*global _global, setDateTimeRange, floatThead, validateFromDateAndToDate*/
'use strict';

function processRequest(command, eventElement) {
    $.post(_global.root + 'request/process-request',
        { command: command, id: $(eventElement).attr('requestId') },
        function (response) {
            if (response.IsSuccessful) {
                query(false);
            } else {
                alert(response.Message);
                return;
            }
        }
    );
}

function approveMultiRequest(requestIds) {
    $.post(_global.root + 'request/approveMultiple',
        { requestIds: requestIds },
        function (response) {
            if (response.IsSuccessful) {
                query(false);
            } else {
                alert(response.Message);
                return;
            }
        }
    );
}

function rejectMultiRequest(requestIds) {
    $.post(_global.root + 'request/rejectMultiple',
        { requestIds: requestIds },
        function (response) {
            if (response.IsSuccessful) {
                query(false);
            } else {
                alert(response.Message);
                return;
            }
        }
    );
}

function bindMenuPending() {
    $('table#data tbody td a.Pending').on('mouseup', function () {
        $.contextMenu({
            selector: '.Pending',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key) {
                        switch (key) {
                            case 'Approve':
                                confirm('Are you sure you want to Approve?', function () {
                                    processRequest(key, e.currentTarget);
                                });
                                break;
                            case 'Reject':
                                confirm('Are you sure you want to Reject?', function () {
                                    processRequest(key, e.currentTarget);
                                });
                                break;
                        }
                    },
                    items: {
                        'Approve': { name: 'Approve', icon: 'approve' },
                        'Reject': { name: 'Reject', icon: 'reject' }
                    }
                };
            }
        });
    });
}

function bindMenuApproved() {
    $('table#data tbody td a.Approved').on('mouseup', function () {
        $.contextMenu({
            selector: '.Approved',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key) {
                        confirm('Are you sure you want to Disapprove?', function () {
                            processRequest(key, e.currentTarget);
                        });
                    },
                    items: {
                        'Disapprove': { name: 'Disapprove', icon: 'reject' }
                    }
                };
            }
        });
    });
}

function query(isFirstLoad) {   // jshint ignore:line
    var action = $('.card-container-base .form').find('#ActionName').val();
    var actionOrgUrl = $('#action').val();

    const queryString = window.location.search;
    if (queryString && isFirstLoad) {
        initDataFirstQuery(queryString);
    }
    var data = $('.card-container-base form.form-filter').serialize();

    var url = _global.root + 'request/' + action + "?" + data ;
    if (!isFirstLoad) {
        var orgUrl = _global.root + 'request/' + actionOrgUrl + "?" + data + "&Flag=1";
        window.history.replaceState(null, null, orgUrl);
    }

    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'html',
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#all-request').html(data);
            
            if ($('table#data #pagination').length > 0) {
                // menu
                bindMenuPending();
                bindMenuApproved();

                initPagination();

                $('.viewhistory').click(function () {
                    var url = $(this).attr('id');

                    $.get(url, function (data) {
                        $('#dialoghistorycontent').html(data);

                        $('#dialoghistorycontent').dialog({
                            modal: true,
                            height: 'auto',
                            width: 'auto',
                            title: 'Request History',
                            show: {
                                effect: 'fade',
                                duration: 300
                            },
                            hide: {
                                effect: 'fade',
                                duration: 200
                            }
                        });
                    });
                });

                $('table#data tbody tr td.col-request a').click(function () {
                    var model = $(this).closest('td').find('.request-model').serialize(),
                        controller = $(this).attr('data-controller'),
                        action = $(this).attr('data-action');

                    document.location = _global.root + controller + '/' + action + '?' + model;
                });

                $('table#data').addClass('table-success'); // class to style mobile

                $('#checkuncheckall').on('click', function () {
                    var isCheckAll = $('#checkuncheckall').prop('checked');
                    
                    $('.chk-request').prop('checked', isCheckAll);
                });
            }
        },
        error: function (textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
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
    var dateOption = urlParams.get('DateOption');
    var fromDate = urlParams.get('FromDate');
    var toDate = urlParams.get('ToDate');
    var isFilterByEffectedDate = urlParams.get('IsFilterByEffectedDate');
    var teamId = urlParams.get('TeamId');
    var statusId = urlParams.get('statusId');
    var employee = urlParams.get('Employee');
    var employeeId = urlParams.get('EmployeeId');
    var leaveTypeId = urlParams.get('LeaveTypeId');
    var requestType = urlParams.get('RequestType');
    var itemsPerPage = urlParams.get('ItemsPerPage');
    var currentPage = urlParams.get('CurrentPage');
    var requestViewType = urlParams.get('RequestViewType');
    var flag = urlParams.get('Flag');

    if (flag == "1") {
        $("#DateOption").val(dateOption);
        $("#FromDate").val(fromDate);
        $("#ToDate").val(toDate);
        $("#IsFilterByEffectedDate").val(isFilterByEffectedDate);
        $("#TeamId").val(teamId);
        $("#statusId").val(statusId);
        $("#Employee").val(employee);
        $("#EmployeeId").val(employeeId);
        $("#LeaveTypeId").val(leaveTypeId);
        $("#RequestType").val(requestType);
        $("#ItemsPerPage").val(itemsPerPage);
        $("#CurrentPage").val(currentPage);
        $("#RequestViewType").val(requestViewType);
    }
}

function initApproveMultiEvent() {
    $('#btn-approve').on('click', function () {
        var requestIds = [];
        $('.chk-request').each(function (index, item) {
            var checked = $(item).prop('checked');
            if (checked) {
                requestIds.push($(item).attr('data-id'));
            }
        });

        if (requestIds.length > 0) {
            confirm('Are you sure you want to approve checked request?', function () {

                console.log(requestIds);
                approveMultiRequest(requestIds);
            });

            
        } else {
            alert('Please choose at least one request to continue!');
        }
    });
}

function initRejectMultiEvent() {
    $('#btn-reject').on('click', function () {
        var requestIds = [];
        $('.chk-request').each(function (index, item) {
            var checked = $(item).prop('checked');
            if (checked) {
                requestIds.push($(item).attr('data-id'));
            }
        });

        if (requestIds.length > 0) {
            confirm('Are you sure you want to reject checked request?', function () {

                console.log(requestIds);
                rejectMultiRequest(requestIds);
            });


        } else {
            alert('Please choose at least one request to continue!');
        }
    });
}

function goToNewPage(pageNumber) {
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
            goToNewPage(pageNumber);
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

    initApproveMultiEvent();
    initRejectMultiEvent();

    query(true);
});
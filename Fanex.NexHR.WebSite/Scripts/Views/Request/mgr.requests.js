/*global _global, _messages, setDateTimeRange, floatThead, validateFromDateAndToDate*/
'use strict';

function processRequest(command, eventElement) {
    $.post(_global.root + 'request/processrequest',
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

function initPagination() {
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

function query(isFirstLoad) {
    var action = $('.card-container-base .form').find('#ActionName').val();

    $.ajax({
        url: _global.root + 'request/' + action,
        type: 'GET',
        dataType: 'html',
        data: $('.card-container-base form.form-filter').serialize(),
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

                floatThead('table#data');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        },
        complete: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeOut(300);
            }
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
        $('#Date').material_select();
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
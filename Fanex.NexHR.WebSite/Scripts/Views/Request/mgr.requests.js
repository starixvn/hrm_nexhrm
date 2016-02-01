/*global _global, _messages, setDateTimeRange, floatThead*/
'use strict';

function processRequest(command, eventElement) {
    $.post(_global.root + 'ajax/processrequest',
    { command: command, id: $(eventElement).attr('requestId') },
        function (response) {
            if (response !== true) {
                alert(response);
                return;
            } else {
                query(false);
            }
        }
    );
}

function bindMenuPending() {
    $('table#data tbody td a.Pending').on('mouseup', function (e) {
        var $this = $(this);

        $.contextMenu({
            selector: '.Pending',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key, options) {
                        switch (key) {
                            case 'Approve':
                                confirm('Are you sure you want to Approve?', function () {
                                    processRequest(key, $this);
                                });
                                break;
                            case 'Reject':
                                confirm('Are you sure you want to Reject?', function () {
                                    processRequest(key, $this);
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
    $('table#data tbody td a.Approved').on('mouseup', function (e) {
        var $this = $(this);

        $.contextMenu({
            selector: '.Approved',
            trigger: 'left',
            build: function ($trigger, e) {
                return {
                    callback: function (key, options) {
                        confirm('Are you sure you want to Disapprove?', function () {
                            processRequest(key, $this);
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
            query(false);
        }
    });

    query(true);
});
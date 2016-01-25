/*global _global, actingButton, sleep, reloadButtonText, setDateTimeRange, floatThead*/
'use strict';

function processRequest(command, eventElement) {
    $.post(_global.root + 'ajax/processrequest',
    { command: command, id: $(eventElement).attr('requestId') },
        function (response) {
            if (response !== true) {
                alert(response);
                return;
            }
            else {
                window.location.reload();
            }
        }
    );
}

function bindMenuPending(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            switch (key) {
                case 'Approve':
                    confirm('Are you sure you want to Approve?', function () {
                        processRequest(key, element);
                    });
                    break;
                case 'Reject':
                    confirm('Are you sure you want to Reject?', function () {
                        processRequest(key, element);
                    });
                    break;
            }
        },
        items: {
            'Approve': { name: 'Approve', icon: 'approve' },
            'Reject': { name: 'Reject', icon: 'reject' }
        }
    });
}

function bindMenuApproved(element) {
    $.contextMenu({
        selector: element,
        trigger: 'left',
        callback: function (key, options) {
            confirm('Are you sure you want to Disapprove?', function () {
                processRequest(key, element);
            });
        },
        items: {
            'Disapprove': { name: 'Disapprove', icon: 'reject' }
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

function query(isFirstLoad) {
    var action = $('.card-container-base form').find('#ActionName').val();

    $.ajax({
        url: _global.root + 'request/' + action,
        type: 'GET',
        dataType: 'html',
        data: $('.card-container-base form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            if (data) {
                $('#all-request').html(data);

                // menu
                bindMenuPending('.Pending');
                bindMenuApproved('.Approved');

                $('.viewhistory').click(function () {
                    var url = $(this).attr('id');

                    $.get(url, function (data) {
                        $('#dialoghistorycontent').html(data);

                        $('#dialoghistorycontent').dialog({
                            modal: true,
                            height: 'auto',
                            width: 'auto',
                            title: 'Request History'
                        });
                    });
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
    $('#Date').val($('#DateOption').val());
    $('#Date').material_select();
    setDateTimeRange($('#DateOption').val());

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    initDatePicker();
    initEventForFromDateAndToDate();

    $('#submit').on('click', function () {
        query(false);
    });

    query(true);
});
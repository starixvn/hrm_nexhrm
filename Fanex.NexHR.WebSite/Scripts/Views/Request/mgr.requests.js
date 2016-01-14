/*global _global, actingButton: false, sleep: false, reloadButtonText: false, setDateTimeRange: false*/
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

$(document).ready(function () {
    // menu
    bindMenuPending('.Pending');
    bindMenuApproved('.Approved');

    $('#Date').val($('#DateOption').val());
    setDateTimeRange($('#DateOption').val());

    initDatePicker('#FromDate');
    initDatePicker('#ToDate');

    $('.viewhistory').click(function () {
        var url = $(this).attr('id');

        $.get(url, function (data) {
            $('#dialoghistorycontent').html(data);

            $('#dialoghistorycontent').dialog({
                modal: true,
                minHeight: 0,
                width: 1100,
                title: 'Request History'
            });
        });
    });

    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#DateOption').attr('value', '0');
    });

    $('#submit').click(function () {
        actingButton('#submit', true);
        sleep(1000);
        reloadButtonText('#submit');
    });

    floatThead('table#data');
});
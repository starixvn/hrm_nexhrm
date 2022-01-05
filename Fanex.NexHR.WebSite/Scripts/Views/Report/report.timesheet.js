/*global _messages, _global, floatThead*/
/*exported index editCheckIn, cancelEditCheckIn, editCheckInAction, editCheckOut, cancelEditCheckOut*/
'use strict';

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function initMonthPicker() {
    $('#monthPickr').monPickr({
        showDateRange: false,
        sortDesc: true,
        today: new Date(),
        minDate: '2010/1/1',
        maxDate: $('#maxDate').val(),
        afterChangeMonth: function () {
            var Month = $('#monthPickr').monPickr('getMonth', 'MM'),
                Year = $('#monthPickr').monPickr('getMonth', 'yyyy');
            $('#Month').val(Month);
            $('#Year').val(Year);
        }
    });

    $('#monthPickr .monpickr-prev span.ui-icon-circle-triangle-w').empty();
    $('#monthPickr .monpickr-next span.ui-icon-circle-triangle-e').empty();
}

function getRequestDetail(requestType, fromDate, toDate, createddate, fromShift, toShift, totalDays, totalMins) {
    var result = '';
    $.ajax({
        url: _global.root + 'request/GetRequestDetail',
        async: false,
        type: 'post',
        data: { 'requesttype': requestType, 'fromdate': fromDate, 'todate': toDate, 'createddate': createddate,
            'fromshiftCode': fromShift, 'toshiftcode': toShift, 'totaldays': totalDays, 'totalmins': totalMins
        },
        success: function (data) {
            result = data;
        }
    });

    return result;
}

function initTooltip() {
    $('.tooltip-request').tipsy({
        className: 'tipsy-request',
        gravity: 'ne',
        html: true,
        title: function () {
            var object = $(this),
                result = getRequestDetail(
                    object.attr('data-requesttype'),
                    object.attr('data-fromdate'),
                    object.attr('data-todate'),
                    object.attr('data-createddate'),
                    object.attr('data-fromshift'),
                    object.attr('data-toshift'),
                    object.attr('data-totaldays'),
                    object.attr('data-totalmins')
                );

            return "<table class='table-tooltip'>" +
                        "<tr>" +
                            "<td>Details</td>" +
                            "<td>" + result + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td>Remark</td>" +
                            "<td>" + object.attr('data-remark') + "</td>" +
                        "</tr>" +
                "</table>";
        }
    });
}

function query(isFirstLoad) {
    var $form = $('form');
    if (!$form.valid()) {
        return;
    }

    $.ajax({
        url: _global.root + 'report/listtimesheet',
        type: 'POST',
        dataType: 'html',
        data: $('.card-container-base .form-filter').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#list-timesheet').html(response);

            initTooltip();

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

function checkInCheckOut() {
    $('#btnCreateCheckIn').click(function () {
        $.ajax({
            url: _global.root + 'report/AddChekInTime',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.IsSuccessful) {
                    alert('Create Check In time successfully!', function () {
                        query(false);
                    });
                } else {
                    alert(response.Message);
                }
            },
            error: function () {
                alert(_messages.errorTryAgain);
            },
            complete: function () {
                $('.loading-process').fadeOut(300);
            }
        });
    });

    $('#btnCreateCheckOut').click(function () {
        $.ajax({
            url: _global.root + 'report/AddChekOutTime',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.IsSuccessful) {
                    alert('Create Check Out time successfully!', function () {
                        query(false);
                    });
                } else {
                    alert(response.Message);
                }
            },
            error: function () {
                alert(_messages.errorTryAgain);
            },
            complete: function () {
                $('.loading-process').fadeOut(300);
            }
        });
    });
}

var canNotEditCheckIn = function (id) {
    if ($('#spanCheckInTime_' + id).text() === '' || $('#spanCheckInTime_' + id).text() === null) {
        $('#txtCheckInTime_' + id).val('00:00');
    }
    else {
        $('#txtCheckInTime_' + id).val($('#spanCheckInTime_' + id).text());
    }
};

var canNotEditCheckOut = function (id) {
    if ($('#spanCheckOutTime_' + id).text() === '' || $('#spanCheckOutTime_' + id).text() === null) {
        $('#txtCheckOutTime_' + id).val('00:00');
    }
    else {
        $('#txtCheckOutTime_' + id).val($('#spanCheckOutTime_' + id).text());
    }
};

function editCheckIn(id) {
    $('#spanCheckInTime_' + id).hide();
    $('#txtCheckInTime_' + id).show();
    $('#btnEditCheckIn_' + id).hide();
    $('#btnUpdateCheckIn_' + id).show();
    $('#btnCancelUpdateCheckIn_' + id).show();
}

function cancelEditCheckIn(id) {
    $('#spanCheckInTime_' + id).show();
    $('#txtCheckInTime_' + id).hide();
    $('#btnEditCheckIn_' + id).show();
    $('#btnUpdateCheckIn_' + id).hide();
    $('#btnCancelUpdateCheckIn_' + id).hide();
}

function editCheckOut(id) {
    $('#spanCheckOutTime_' + id).hide();
    $('#txtCheckOutTime_' + id).show();
    $('#btnEditCheckOut_' + id).hide();
    $('#btnUpdateCheckOut_' + id).show();
    $('#btnCancelUpdateCheckOut_' + id).show();
}

function cancelEditCheckOut(id) {
    $('#spanCheckOutTime_' + id).show();
    $('#txtCheckOutTime_' + id).hide();
    $('#btnEditCheckOut_' + id).show();
    $('#btnUpdateCheckOut_' + id).hide();
    $('#btnCancelUpdateCheckOut_' + id).hide();
}

function editCheckInAction(id) {
    var checkinTime = $('#txtCheckInTime_' + id).val();
    var isValid = /^([0-1]?[0-9]|2[0-4]):([0-5][0-9])(:[0-5][0-9])?$/.test(checkinTime);
    if (!isValid) {
        alert('Input time is not valid');
        canNotEditCheckIn(id);
        return false;
    }

    confirm('Are you sure you want to change?', function () {
        var oldCheckinTime = $('#spanCheckInTime_' + id).val();
        if (checkinTime !== oldCheckinTime) {
            $.post(_global.root + 'Report/EditChekInOutTime', { time: checkinTime, inOutType: 0 }, function (response) {
                if (response.IsSuccessful) {
                    query(false);
                } else {
                    alert(response.Message);
                    canNotEditCheckIn(id);
                }
            }).error(function () {
                alert(_messages.errorTryAgain);
                canNotEditCheckIn(id);
            });
        } else {
            alert(_messages.errorTryAgain);
            canNotEditCheckIn(id);
        }
    });
}

function editCheckOutAction(id) {
    var checkoutTime = $('#txtCheckOutTime_' + id).val();
    var isValid = /^([0-1]?[0-9]|2[0-4]):([0-5][0-9])(:[0-5][0-9])?$/.test(checkoutTime);
    if (!isValid) {
        alert('Input time is not valid');
        canNotEditCheckOut(id);
        return false;
    }

    confirm('Are you sure you want to change?', function () {
        var oldCheckOutTime = $('#spanCheckOutTime_' + id).val();
        if (checkoutTime !== oldCheckOutTime) {
            $.post(_global.root + 'Report/EditChekInOutTime', { time: checkoutTime, inOutType: 1 }, function (response) {
                if (response.IsSuccessful) {
                    query(false);
                } else {
                    alert(response.Message);
                    canNotEditCheckOut(id);
                }
            }).error(function () {
                alert(_messages.errorTryAgain);
                canNotEditCheckOut(id);
            });
        } else {
            alert(_messages.errorTryAgain);
            canNotEditCheckOut(id);
        }
    });
}


$(document).ready(function () {
    initMonthPicker();

    query(true);

    $('#submit').click(function () {
        query(false);
    });

    checkInCheckOut();
});
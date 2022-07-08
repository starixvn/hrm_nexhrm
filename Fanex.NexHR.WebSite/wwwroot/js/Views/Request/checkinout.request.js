/*global _global, _messages, zeroPrefix, bindMenuPending*/
'use strict';

function updateZeroPrefix() {
    //add zeroPrefix
    $('input[Name=Hour]').val(zeroPrefix($('input[Name=Hour]').val()));
    $('input[Name=Minute]').val(zeroPrefix($('input[Name=Minute]').val()));
}

function bindingHourAndMinute() {
    if ($('#CheckInOutType').val() === "1") {
        $('#Hour').val($('#InTime').val().substring(0, 2));
        $('#Minute').val($('#InTime').val().trim().substring(3, 5));
    } else {
        $('#Hour').val($('#OutTime').val().substring(0, 2));
        $('#Minute').val($('#OutTime').val().trim().substring(3, 5));
    }
}

//this function for load Edit
function getShiftByDate() {
    var empId = parseInt($('#EmployeeId').val());
    if (empId > 0) {
        $.ajax({
            url: _global.root + 'shift/getshiftbydate',
            type: 'POST',
            dataType: 'json',
            data: { employeeId: empId, date: $('#ForDate').val() },
            success: function (data) {
                if (data != null) {
                    $('#shiftname').text(data.DisplayName);
                    $('#Shift').val(data.DisplayName);
                    $('#InTime').val(data.BeginCheckInTime);
                    $('#OutTime').val(data.BeginCheckOutTime);
                    $('input[type=submit]').show();
                    bindingHourAndMinute();
                } else {
                    $('#shiftname').text(_messages.notAvailable);
                    $('#Shift').val(_messages.notAvailable);
                }
            },
            error: function () {
                $('#shiftname').text(_messages.notAvailable);
                $('#Shift').val(_messages.notAvailable);
            }
        });
    }
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'CheckInOut/ListCheckInOutRequest',
        type: 'GET',
        dataType: 'html',
        data: { year: $('#Year').val() },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#list-check-in-out-request').html(response);

            bindMenuPending();
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

function initEventPickerDate() {
    $('#ForDate').on('change', function () {
        var empId = parseInt($('#EmployeeId').val());
        if (empId > 0) {
            getShiftByDate();
        }
    });
}

function initCreateUpdatePage() {
    //Process redirect to Check In/Out request.
    var successed = $('#successed').val();

    if (successed === 'true') {
        if (successed === 'true') {
            confirm('Successfull!\nPress OK to continue', function () {
                document.location = _global.root + 'checkinout/EmployeeView/' + $('#year').val();
            });
        }
    }

    updateZeroPrefix();
    //getShiftByDate();

    $('#CheckInOutType_1, #CheckInOutType_2').on('change', function () {
        if ($('#CheckInOutType_1').prop('checked')) {
            $('#CheckInOutType').val(1);
        }
        else {
            $('#CheckInOutType').val(2);
        }
        bindingHourAndMinute();
    });

    $('#legendInfo').tipsy({
        className: 'leave-request',
        gravity: 'ne',
        html: true,
        title: function () {
            return $('#requestInfo .requestInfo').html();
        }
    });

    initEventPickerDate();

    $('form').submit(function (e) {
        if ($('#Shift').val() !== 'U1' && ($("#$('#CheckInOutType_1')_2").prop('checked'))) {
            var timeOut = ($('#Hour').val().length < 2 ? '0' + $('#Hour').val() : $('#Hour').val()) + ':' +
                        ($('#Minute').val().length < 2 ? '0' + $('#Minute').val() : $('#Minute').val());

            if (timeOut < $('#InTime').val() || $('#Hour').val().length === 0 || $('#Minute').val().length === 0) {
                e.preventDefault();
                alert('Request time invalid!');
            }
        }
    });
}

$(document).ready(function () {
    if ($('.form-create-update').length > 0) {
        initCreateUpdatePage();
    } else {
        query(true);

        $('#submit').on('click', function () {
            query(false);
        });
    }
});
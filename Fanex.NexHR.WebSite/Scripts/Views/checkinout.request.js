'use strict';
/*global _global:alse , _messages: false, zeroPrefix*/

function updateZeroPrefix() {
    //add zeroPrefix
    $('input[Name=Hour]').val(zeroPrefix($('input[Name=Hour]').val()));
    $('input[Name=Minute]').val(zeroPrefix($('input[Name=Minute]').val()));
}

function bindingHourAndMinute() {
    if ($('#CheckInOutType_1').prop('checked')) {
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
        $.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: empId, date: $('#ForDate').val() },
        function (data) {
            if (data != null) {
                $('#shiftname').text(data.Name);
                $('#Shift').val(data.Name);
                $('#InTime').val(data.BeginCheckInTime);
                $('#OutTime').val(data.BeginCheckOutTime);
                $('input[type=submit]').show();
            } else {
                $('#shiftname').text(_messages.notAvailable);
                $('#Shift').val(_messages.notAvailable);
                $('input[type=submit]').hide();
            }
        });
    }
}

$(document).ready(function () {
    if ($('.form-create-update').length > 0) {
        //Process redirect to Check In/Out request.
        var successed = $('#successed').val(),
            year = $('#year').val();

        if (successed === 'true') {
            if (successed === 'true') {
                confirm('Successfull!\nPress OK to continue', function () {
                    document.location = _global.root + 'checkinout/EmployeeView/' + year;
                });
            }
        }

        updateZeroPrefix();
        getShiftByDate();

        $('#CheckInOutType_1, #CheckInOutType_2').on('change', function () {
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

        initDatePicker('.datepicker');

        $('.datepicker').on('change', function () {
            var empId = parseInt($('#EmployeeId').val());

            if (empId > 0) {
                $.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: empId, date: $('#ForDate').val() },
                function (data) {
                    if (data != null) {
                        $('#shiftname').text(data.Name);
                        $('#Shift').val(data.Name);
                        $('#InTime').val(data.BeginCheckInTime);
                        $('#OutTime').val(data.BeginCheckOutTime);
                        bindingHourAndMinute();
                        $('input[type=submit]').show();
                    }
                    else {
                        $('#shiftname').text(_messages.notAvailable);
                        $('#Shift').val(_messages.notAvailable);
                        $('input[type=submit]').hide();
                    }
                });
            }
        });

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
});

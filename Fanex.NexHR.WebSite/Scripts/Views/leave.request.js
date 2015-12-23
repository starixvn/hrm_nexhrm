/*global _global*/
'use strict';

function setValueforToTime() {
    var arr = $('#FromTime').val().split(':');
    var x = parseInt(arr[0].trim(), 10);
    x = x + 4;
    if (x >= 24) {
        x = x - 24;
    }
    var hour = '';
    if (x < 10) {
        hour = '0' + x;
    }

    else {
        hour = x;
    }

    $('#ToTime').val(hour + ':' + arr[1]);
    $('#ToTime').material_select();
}

function enableControlforHalfday() {
    if ($('#HalfDayOff').prop('checked')) {
        $('#ToDate').val($('#FromDate').val());
        $('.time').removeClass('hide');
        $('#ToDate').prop('readonly', true);
    }
    else {
        $('.time').addClass('hide');
        $('#ToDate').prop('readonly', false);
    }
}

function bindHalfDay() {
    var empId = parseInt($('#EmployeeId').val());

    if (empId > 0) {
        enableControlforHalfday();

        if ($('#HalfDayOff').prop('checked')) {
            $('#TotalDays').val(0.5);
            //get shift information
            $.getJSON(_global.root + 'api/shift/GetShiftByDate',
                { employeeId: empId, date: $('#FromDate').val() },
                function (response) {
                    if (response != null) {
                        $('#FromTime').val(response.BeginCheckInTime);
                        $('#FromTime').material_select();
                        setValueforToTime();
                    }
                }
            );
        }
        else {
            $('#TotalDays').val(1);
        }
    }
}

$(document).ready(function () {
    if ($('.form-create-update').length > 0) {
        var successed = $('#successed').val(),
        year = $('#year').val();

        $('#legendInfo').tipsy({
            className: 'leave-request',
            gravity: 'ne',
            html: true,
            title: function () {
                return $('#requestInfo .requestInfo').html();
            }
        });

        if (successed === 'true') {
            confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + 'Leave/EmployeeView/' + year;
                }
            );
        }

        $('#TotalDays').prop('readonly', true);
        enableControlforHalfday();

        $('.datepicker').on('click', function (event) {
            event.preventDefault();

            if (!$(this).prop('readonly')) {
                initDatePicker(this);
            }
        });

        $('#HalfDayOff').on('change', function () {
            bindHalfDay();
        });

        $('#FromTime').on('change', function () {
            setValueforToTime();
        });

        $('#Employee').on('change', function () {
            var empId = parseInt($('#EmployeeId').val());
            if (empId) {
                $.getJSON(_global.root + 'api/employee/GetUnavailDate', { employeeId: empId },
                function (response) {
                    if (response != null) {
                        $('#FromDate, #ToDate').attr('disableddate', response.join(','));
                    }
                }
            );
            }
        });

        $('#FromDate, #ToDate').on('change', function () {
            var empId = parseInt($('#EmployeeId').val());

            if (empId > 0) {
                if ($('#HalfDayOff').attr('checked')) {
                    $('#ToDate').val($('#FromDate').val());
                    $.getJSON(_global.root + 'api/shift/GetShiftByDate', { employeeId: empId, date: $('#FromDate').val() },
                    function (response) {
                        if (response != null) {
                            $('#FromTime').val(response.BeginCheckInTime);
                            $('#FromTime').material_select();
                            setValueforToTime();
                            $('input[type=submit]').show();
                        } else {
                            $('input[type=submit]').hide();
                        }
                    }
                );
                } else {
                    $.getJSON(_global.root + 'api/employee/GetRangeDate',
                    { employeeId: empId, fromDate: $('#FromDate').val(), toDate: $('#ToDate').val() },
                    function (response) {
                        if (response != null) {
                            $('#TotalDays').val(response);
                        }
                    });
                }
            }
        });
    }

    $('form').submit(function () {
        $('#LeaveTypeName').val($('#LeaveTypeId').find(':selected').text().trim());
    });
});
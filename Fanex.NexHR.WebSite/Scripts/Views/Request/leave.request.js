/*global _global, _messages*/
'use strict';

function setValueforToTime() {
    var arr = $('#FromTime').val().split(':'),
        x = parseInt(arr[0].trim(), 10),
        hour = '';
    x = x + 4;

    if (x >= 24) {
        x = x - 24;
    }

    if (x < 10) {
        hour = '0' + x;
    } else {
        hour = x;
    }

    $('#ToTime').val(hour + ':' + arr[1]);
    $('#ToTime').material_select();
}

function enableControlforHalfday() {
    if ($('#HalfDayOff').prop('checked')) {
        $('#ToDate').val($('#FromDate').val());
        $('.time').removeClass('hide');
        $('#ToDate').prop('readonly', true).datepicker('disable').prop('disabled', false);
    } else {
        $('.time').addClass('hide');
        $('#ToDate').prop('readonly', false).datepicker('enable');
    }
}

function bindHalfDay() {
    var empId = parseInt($('#EmployeeId').val());

    if (empId > 0) {
        enableControlforHalfday();

        if ($('#HalfDayOff').prop('checked')) {
            $('#TotalDays').val(0.5);
            //get shift information
            $.post(_global.root + 'shift/GetShiftByDate',
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

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Leave/ListLeaveRequest',
        type: 'GET',
        dataType: 'html',
        data: { year: $('#Year').val() },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#list-leave-request').html(response);

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

function initEventForFromDateAndToDate() {
    $('#FromDate, #ToDate').on('change', function () {
        var empId = parseInt($('#EmployeeId').val());
        if (empId > 0) {
            if ($('#HalfDayOff').prop('checked')) {
                $('#ToDate').val($('#FromDate').val());

                $.post(_global.root + 'shift/GetShiftByDate',
                    { employeeId: empId, date: $('#FromDate').val() },
                    function (response) {
                        if (response != null) {
                            $('#FromTime').val(response.BeginCheckInTime);
                            $('#FromTime').material_select();
                            setValueforToTime();
                            $('input[type=submit]').show();
                        } else {
                            $('input[type=submit]').hide();
                        }
                    });
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

function initCreateUpdatePage() {
    var successed = $('#successed').val();

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
                    document.location = _global.root + 'Leave/EmployeeView/' + $('#year').val();
                }
            );
    }

    $('#TotalDays').prop('readonly', true);

    enableControlforHalfday();

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

    initEventForFromDateAndToDate();

    $('form').submit(function () {
        $('#LeaveTypeName').val($('#LeaveTypeId').find(':selected').text().trim());
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
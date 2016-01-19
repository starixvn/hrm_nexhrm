/*global _global: false, _messages: false, noInformation: false*/
/*exported index, formatTotalDays*/
'use strict';

var rowIndex = 1;

function index() {
    return rowIndex++;
}

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

function formatTotalDays(totalDays) {
    return totalDays.toFixed(1);
};

function query() {
    $.ajax({
        url: _global.root + 'Leave/EmployeeView',
        type: 'POST',
        dataType: 'json',
        data: { year: $('#Year').val() },
        beforeSend: function () {
            $('.loading-process').fadeIn(300);
        },
        success: function (response) {
            if (response.data.length > 0) {
                rowIndex = 1;
                $('#data tbody').children().remove();
                $('#template').tmpl(response.data).appendTo('#data');
            } else {
                $('#data tbody').children().remove();
                $(noInformation(10)).appendTo('#data');
            }

            $('.loading-process').fadeOut(300);
        },
        error: function () {
            $('.loading-process').fadeOut(300);
            alert(_messages.errorTryAgain);
        }
    });
}

function initEventForFromDateAndToDate() {
    $('#FromDate, #ToDate').on('change', function () {
        var empId = parseInt($('#EmployeeId').val());

        if (empId > 0) {
            if ($('#HalfDayOff').prop('checked')) {
                $('#ToDate').val($('#FromDate').val());

                $.getJSON(_global.root + 'api/shift/GetShiftByDate',
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

    initDatePicker('#FromDate');
    initDatePicker('#ToDate');


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
        query();

        $('#submit').on('click', function () {
            query();
        });
    }
});
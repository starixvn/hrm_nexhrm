/*global _global*/
"use strict";

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
}

function enableControlforHalfday() {
    var $input = $('#ToDate').pickadate(),
        picker = $input.pickadate('picker');

    if ($('#HalfDayOff').prop('checked')) {
        $('#ToDate').val($('#FromDate').val());
        $('.time').removeClass('hide');

        picker.stop();
        $('#ToDate').prop('readonly', true).prop('disabled', false);
    }
    else {
        $('.time').addClass('hide');

        picker.start();
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

function initPickerDate(el) {
    var $input = $(el).pickadate({ selectYears: 2 });
    var picker = $input.pickadate('picker');

    // get minDate, maxDate, disabledDates
    var mindate = $(el).attr('mindate');
    var maxdate = $(el).attr('maxdate');
    var disabledDates = $(el).attr('disabledDate');

    if (mindate !== 'null') {
        mindate = mindate.split('/');
        picker.set('min', new Date(mindate[0], mindate[1] - 1, mindate[2]));
    }

    if (maxdate !== 'null') {
        maxdate = maxdate.split('/');
        picker.set('max', new Date(maxdate[0], maxdate[1] - 1, maxdate[2]));
    }

    if (disabledDates.length > 0) {
        var disabledDays = [],
                datesToDisables = disabledDates.split(",");

        $.each(datesToDisables, function (index, datesToDisable) {
            var date = new Date(datesToDisable).format('yyyy/mm/dd').split('/');
            disabledDays.push(new Date(date[0], date[1] - 1, date[2]));
        });

        picker.set('disable', disabledDays);
    }
}

$(document).ready(function () {
    if ($('.form-create-update').length > 0) {
        var successed = $("#successed").val(),
        year = $("#year").val();

        $('#legendInfo').tipsy({
            className: 'leave-request',
            gravity: 'ne',
            html: true,
            title: function () {
                return $('#requestInfo .requestInfo').html();
            }
        });

        if (successed === "true") {
            confirm('<i class="icon-success mdi mdi-checkbox-marked-circle-outline"></i><p>Successfull!</p><p>Press OK to continue</p>',
                function () {
                    document.location = _global.root + "Leave/EmployeeView/" + year;
                }
            );
        }

        initPickerDate('#FromDate');
        initPickerDate('#ToDate');

        $('#TotalDays').prop('readonly', true);
        enableControlforHalfday();
    }

    $('#HalfDayOff').on('change', function () {
        bindHalfDay();
    });

    $('#FromTime').on('change', function () {
        setValueforToTime();
    });

    $("#Employee").on('change', function () {
        var empId = parseInt($('#EmployeeId').val());
        if (empId) {
            $.getJSON(_global.root + 'api/employee/GetUnavailDate', { employeeId: empId },
                function (response) {
                    if (response != null) {
                        $("#FromDate, #ToDate").attr("disableddate", response.join(","));
                    }
                }
            );
        }
    });

    $("#FromDate, #ToDate").on('change', function () {
        var empId = parseInt($('#EmployeeId').val());

        if (empId > 0) {
            if ($('#HalfDayOff').attr('checked')) {
                $('#ToDate').val($('#FromDate').val());
                $.getJSON(_global.root + 'api/shift/GetShiftByDate', { employeeId: empId, date: $('#FromDate').val() },
                    function (response) {
                        if (response != null) {
                            $('#FromTime').val(response.BeginCheckInTime);
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
                            $("#TotalDays").val(response);
                        }
                    });
            }
        }
    });

    $('form').submit(function () {
        $('#LeaveTypeName').val($('#LeaveTypeId').find(":selected").text().trim());
    });
});
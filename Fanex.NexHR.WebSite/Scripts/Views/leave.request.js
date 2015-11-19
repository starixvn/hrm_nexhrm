/*global _global*/
"use strict";

function setValueforToTime() {
    var arr = $('#FromTime').val().split(':');
    var x = parseInt(arr[0].trim(), 10);
    x = x + 4;
    if (x >= 24) {
        x = x - 24;
    }
    var hour = "";
    if (x < 10) {
        hour = "0" + x;
    }

    else {
        hour = x;
    }

    $('#ToTime').val(hour + ":" + arr[1]);
}

function enableControlforHalfday() {
    if ($('#HalfDayOff').prop('checked')) {
        $('#ToDate').val($('#FromDate').val());
        $('.time').show();
        $('#ToDate').prop('readonly', true).datepicker('disable').prop('disabled', false);
    }
    else {
        $('.time').hide();
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
            $.getJSON(_global.root + 'api/shift/getshiftbydate',
            { employeeId: empId, date: $('#FromDate').val() },
            function (response) {
                if (response != null) {
                    $('#FromTime').val(response.BeginCheckInTime);
                    setValueforToTime();
                }
            });
        }
        else {
            $('#TotalDays').val(1);
        }
    }
}

$(document).ready(function () {
    var successed = $("#successed").val();
    var year = $("#year").val();
    if (successed === "true") {
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "Leave/EmployeeView/" + year;
        });
    }

    $('#TotalDays').prop('readonly', true);
    enableControlforHalfday();
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

    $("#FromDate, #ToDate").change(function () {
        var empId = parseInt($('#EmployeeId').val());
        if (empId > 0) {
            if ($('#HalfDayOff').attr('checked')) {
                $('#ToDate').val($('#FromDate').val());
                $.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: empId, date: $('#FromDate').val() },
                    function (response) {
                        if (response != null) {
                            $('#FromTime').val(response.BeginCheckInTime);
                            setValueforToTime();
                            $('input[type=submit]').show();
                        }
                        else {
                            $('input[type=submit]').hide();
                        }
                    }
                );
            }
            else {
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
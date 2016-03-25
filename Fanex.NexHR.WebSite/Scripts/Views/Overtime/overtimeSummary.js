'use strict';
/*global _global, _messages, initDatePicker, floatThead*/

function initAddLeaveConversion() {
    $('.addLeaveConversion').click(function () {
        var employeeId = $(this).attr('data-id');
        $('#EmpId').val(employeeId);
        $('#insertform').dialog('open');
    });
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Overtime/OvertimeSummary',
        type: 'POST',
        dataType: 'html',
        data: {
            empId: $('#EmployeeId').val(),
            teamid: $('#TeamId').val(),
            year: $('#Year').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('#listOvertimeSummary').html(response);
            initAddLeaveConversion();
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

$(document).ready(function () {
    query(true);
    $('#submit').on('click', function () {
        query(false);
    });

    $('#insertform').dialog({
        modal: true,
        autoOpen: false,
        width: 'auto',
        height: 'auto',
        open: function () {
            $('#TotalDays').val('0.5');
            initDatePicker();
        },
        show: {
            effect: "blind",
            duration: 200
        },
        hide: {
            effect: "blind",
            duration: 100
        }
    });

    $('#insertform form').submit(function (evt) {
        evt.preventDefault();
        $.post(_global.root + 'AnnualLeave/AddLeaveConversion',
            {
                empId: $('#EmpId').val(),
                totalDays: $('#TotalDays').val(),
                ondate: $('#OnDate').val(),
                description: $('#Description').val()
            }, function (response) {
                if (response.IsSuccessful) {
                    $('#insertform').dialog('close');
                    document.location = _global.root + 'overtime/OvertimeSummary';
                } else {
                    alert(response.Message);
                }
            });
    });

    $('#cancel').click(function () {
        $('#insertform').dialog('close');
    });
});
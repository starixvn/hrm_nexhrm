'use strict';
/*global _global:false */

$(document).ready(function () {
    $('.addLeavededuction').click(function () {
        var employeeId = $(this).attr('data-id');
        var annualBalance = $(this).attr('data-annualbalance');
        $('#EmpId').val(employeeId);
        $('#annualBalance').text(annualBalance);
        $('#insertform').dialog({
            modal: true,
            width: 700,
            height: 370,
            open: function () {
                $('#TotalDays').val('0.5');
                initDatePicker('.datepicker');
            }
        });
    });

    $('#insertform form').submit(function (evt) {
        evt.preventDefault();
        $('.loading-process').fadeIn(200)
        $.post(_global.root + 'ajax/AddLeaveDeduction',
            {
                empId: $('#EmpId').val(),
                totalDays: $('#TotalDays').val(),
                deductDate: $('#DeductDate').val(),
                description: $('#Description').val(),
                isAnnual: $("#IsAnnual").attr("checked") === "checked"
            }, function (response) {
                if (response.Code === 0) {
                    $('#insertform').dialog('close');
                    document.location = _global.root + 'report/LateInEarlyOutSummary';
                }
                else {
                    alert(response.Message);
                }
                $('.loading-process').fadeOut(500);
            });
    });

    $('#updateform form').submit(function (evt) {
        evt.preventDefault();
        var url = window.location.href;
        var $form = $(this);
        if ($form.valid()) {
            $.post(_global.root + 'ajax/EditLeaveDeduction',
            {
                leaveDeductionId: $('#LeaveDeductionId').val(),
                totalDays: $('#TotalDays').val(),
                deductDate: $('#DeductDate').val(),
                description: $('#Description').val(),
                isAnnual: $("#IsAnnual").attr("checked") === "checked"
            }, function (response) {
                if (response.Code === 0) {
                    $('#updateform').dialog('close');
                    document.location = url;
                }
                else {
                    alert(response.Message);
                }
            });
        }
    });

    $('#cancel').click(function () {
        $('#insertform').dialog('close');
        $('#updateform').dialog('close');
    });
});
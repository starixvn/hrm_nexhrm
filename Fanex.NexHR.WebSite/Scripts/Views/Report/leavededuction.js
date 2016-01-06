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
            width: 800,
            height: 370,
            open: function () {
                $('#TotalDays').val('0.5');
            }
        });
    });

    $('#insertform form').submit(function (evt) {
        evt.preventDefault();
        var $form = $(this);
        if ($form.valid()) {
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
            });
        }
    });

    $('.editLeaveDeduction').click(function () {
        var leaveDeductionId = $(this).attr('data-id');
        var leaveDays = $(this).attr('data-leavedays');
        var deductDate = $(this).attr('data-deductdate');
        var isAnnual = $(this).attr('data-isannual') === "True";
        var description = $(this).parent().parent().find(".description").first().html();
        $('#updateform').dialog({
            modal: true,
            width: 400,
            height: 320,
            open: function () {
                $('#TotalDays').val(leaveDays);
                $('#DeductDate').val(deductDate);
                $('#Description').val(description);
                $("#IsAnnual").attr("checked", isAnnual);
                $('#LeaveDeductionId').val(leaveDeductionId);
            }
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

    $('.deleteLeaveDeduction').click(function () {
        confirm('Are you sure you want to delete this leave deduction?', function () {
            var leaveDeductionId = $(this).attr('data-id');
            var url = window.location.href;

            $.post(_global.root + 'ajax/DeleteLeaveDeduction', { leavededuction: leaveDeductionId }, function (response) {
                if (response.Code === 0) {
                    document.location = url;
                }
                else {
                    alert(response.Message);
                }
            });
        });
    });

    $('#cancel').click(function () {
        $('#insertform').dialog('close');
        $('#updateform').dialog('close');
    });
});
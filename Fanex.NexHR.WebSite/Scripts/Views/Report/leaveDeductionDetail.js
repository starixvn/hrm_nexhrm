'use strict';
/*global _global:false */

$(document).ready(function () {
    initDatePicker();

    $('.editLeaveDeduction').click(function () {
        var leaveDeductionId = $(this).attr('data-id'),
            leaveDays = $(this).attr('data-leavedays'),
            deductDate = $(this).attr('data-deductdate'),
            isAnnual = $(this).attr('data-isannual') === "True",
            description = $(this).parent().parent().find(".description").first().html();

        $('#updateform').dialog({
            modal: true,
            width: 700,
            open: function () {
                initDatePicker();
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
                } else {
                    alert(response.Message);
                }
            });
    });

    $('.deleteLeaveDeduction').click(function () {
        var leaveDeductionId = $(this).attr('data-id');
        confirm('Are you sure you want to delete this leave deduction?', function () {
            var url = window.location.href;
            $.post(_global.root + 'ajax/DeleteLeaveDeduction', { leaveDeductionId: leaveDeductionId }, function (response) {
                alert('response.Message');
                if (response.Code === 0) {
                    document.location = url;
                } else {
                    alert(response.Message);
                }
            });
        });
    });

    $('#cancel').click(function () {
        $('#updateform').dialog('close');
    });
});
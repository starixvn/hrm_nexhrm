'use strict';
/*global _global, _messages, noInformation, initDatePicker, floatThead, formatJsonDate, setDateTimeRange,
 validateFromDateAndToDate, isHRRole*/
/*exported index*/

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function initEditLeaveDeduction() {
    $('.editLeaveDeduction').click(function () {
        var leaveDeductionId = $(this).attr('data-id'),
            leaveDays = $(this).attr('data-leavedays'),
            deductDate = $(this).attr('data-deductdate'),
            isAnnual = $(this).attr('data-isannual') === 'True',
            description = $(this).closest('tr').find('.description').html();

        $('#updateform').dialog({
            modal: true,
            width: 'auto',
            open: function () {
                initDatePicker();
                $('#TotalDays').val(leaveDays);
                $('#DeductDate').val(deductDate);
                $('#Description').val(description);
                $('#IsAnnual').prop('checked', isAnnual);
                $('#LeaveDeductionId').val(leaveDeductionId);
            },
            show: {
                effect: 'blind',
                duration: 200
            },
            hide: {
                effect: 'blind',
                duration: 100
            }
        });
    });

    $('#updateform form').submit(function (evt) {
        evt.preventDefault();
        var url = window.location.href;
        $.post(_global.root + 'AnnualLeave/EditLeaveDeduction',
            {
                leaveDeductionId: $('#LeaveDeductionId').val(),
                totalDays: $('#TotalDays').val(),
                deductDate: $('#DeductDate').val(),
                description: $('#Description').val(),
                isAnnual: $('#IsAnnual').prop('checked')
            }, function (response) {
                if (response.IsSuccessful) {
                    $('#updateform').dialog('close');
                    document.location = url;
                } else {
                    alert(response.Message);
                }
            });
    });

    $('#cancel').click(function () {
        $('#updateform').dialog('close');
    });
}

function initDeleteLeaveDeduction() {
    $('.deleteLeaveDeduction').click(function () {
        var leaveDeductionId = $(this).attr('data-id');
        confirm('Are you sure you want to delete this leave deduction?', function () {
            var url = window.location.href;
            $.post(_global.root + 'AnnualLeave/DeleteLeaveDeduction', { leaveDeductionId: leaveDeductionId }, function (response) {
                if (response.IsSuccessful) {
                    document.location = url;
                } else {
                    alert(response.Message);
                }
            });
        });
    });
}

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'AnnualLeave/LeaveDeductionDetail',
        type: 'POST',
        dataType: 'json',
        data: $('.card-container-base .form form').serialize(),
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (response) {
            $('table#data tbody').children().remove();

            if (response.data && response.data.length > 0) {
                rowIndex = 1;

                var tmpl = $.templates('#template'),
                    helpers = {
                        index: index,
                        formatJsonDate: formatJsonDate,
                        isHRRole: isHRRole
                    },
                    html = tmpl.render(response.data, helpers);

                $(html).appendTo('table#data');

                initEditLeaveDeduction();
                initDeleteLeaveDeduction();
            } else {
                $(noInformation(10)).appendTo('table#data');
            }

            floatThead('table#data');
        },
        error: function () {
            alert(_messages.errorTryAgain);
        },
        complete: function () {
            $('.loading-process').fadeOut(300);
        }
    });
}

$(document).ready(function () {
    $('#Date').on('change', function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val('0');
        $('#Date').material_select();   // jshint ignore:line
        $('#DateOption').attr('value', '0');
    });

    $('#Date').val($('#DateOption').val()).change();

    query(true);

    $('#submit').on('click', function () {
        if (validateFromDateAndToDate()) {
            query(false);
        }
    });
});
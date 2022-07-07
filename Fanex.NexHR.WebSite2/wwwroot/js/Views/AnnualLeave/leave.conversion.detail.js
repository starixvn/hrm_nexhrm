'use strict';
/*global _global, _messages, noInformation, initDatePicker, floatThead, formatJsonDate, isHRRole, setDateTimeRange, validateFromDateAndToDate*/
/*exported index*/

var rowIndex = 1;

function index() {
    return rowIndex++;
}

function initEditLeaveConversion() {
    $('.editLeaveConversion').click(function () {
        var leaveConversionId = $(this).attr('data-id'),
            empId = $(this).attr('data-empid'),
            leaveDays = $(this).attr('data-leavedays'),
            date = $(this).attr('data-date'),
            description = $(this).closest('tr').find('.description').html();

        $('#updateform').dialog({
            modal: true,
            width: 'auto',
            open: function () {
                initDatePicker();
                $('#TotalDays').val(leaveDays);
                $('#OnDate').val(date);
                $('#Description').val(description);
                $('#LeaveConversionId').val(leaveConversionId);
                $('#EmpId').val(empId);
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
        $.post(_global.root + 'AnnualLeave/EditLeaveConversion',
            {
                leaveConversionId: $('#LeaveConversionId').val(),
                totalDays: $('#TotalDays').val(),
                ondate: $('#OnDate').val(),
                description: $('#Description').val(),
                empid: $('#EmpId').val()
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

function initDeleteLeaveConversion() {
    $('.deleteLeaveConversion').click(function () {
        var leaveConversionId = $(this).attr('data-id');
        confirm('Are you sure you want to delete this leave conversion?', function () {
            var url = window.location.href;
            $.post(_global.root + 'AnnualLeave/DeleteLeaveConversion', { leaveConversionId: leaveConversionId }, function (response) {
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
        url: _global.root + 'AnnualLeave/LeaveConversionDetail',
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

            if (response && response.length > 0) {
                rowIndex = 1;

                var tmpl = $.templates('#template'),
                    helpers = {
                        index: index,
                        formatJsonDate: formatJsonDate,
                        isHRRole: isHRRole
                    },
                    html = tmpl.render(response, helpers);

                $(html).appendTo('table#data');

                initEditLeaveConversion();
                initDeleteLeaveConversion();
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
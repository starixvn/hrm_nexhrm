'use strict';
/*global _global:false , _messages:false, initDatePicker:false*/

var _checked = false;
var _totalDays = 0;
var _ids;

$(document).ready(function () {
    initDatePicker('#AffectedFrom');

    $('#addadj').on('click', function () {
        _ids = [];
        $('.aba').each(function (index, item) {
            var checked = $(item).prop('checked');
            if (checked) {
                _ids.push($(item).attr('data-id'));
            }
        });

        $('#EmpIds').val(_ids);

        if (_ids.length > 0) {
            var title = $(this).attr('title');

            $('#insertform').dialog({
                title: title,
                modal: true,
                width: 450,
                dialogClass: 'add-adjustment',
                open: function () {
                    $('#TotalDays').val('');
                }
            });
        } else {
            alert('Please choose at least one employee to continue!');
        }
    });

    $('#resetblc').on('click', function () {
        var title = $(this).attr('title');

        $('#resetform').dialog({
            title: title,
            modal: true,
            width: 450,
            dialogClass: 'reset-balance',
            open: function () {
                $('#TotalDays').val('');
            }
        });
    });

    $('#addbalance').on('click', function () {
        confirm('Are you sure you add annual leave balance for new year?', function () {
            $.post(_global.root + 'ajax/OpenAnnualLeaves', function (response) {
                if (response) {
                    document.location = _global.root + 'annualleavebalance/index';
                } else {
                    alert(_messages.errorTryAgain);
                }
            });
        });
    });

    $('#insertform form').submit(function (evt) {
        evt.preventDefault();
        var $form = $(this);
        _totalDays = parseFloat($('#TotalDays').val());

        if ($form.valid()) {
            $.post(_global.root + 'ajax/AdjustAnnualLeaves', $('form').serialize(), function (response) {
                if (response === -1) {
                    alert(_messages.invalidInputData);
                } else if (response) {
                    $.each(_ids, function (index, value) {
                        var currentAnnualLeaveBalance = parseFloat($('#alb' + value).text());
                        $('#alb' + value).text(currentAnnualLeaveBalance + _totalDays);
                    });

                    document.location = _global.root + 'annualleavebalance/index';
                } else {
                    alert(_messages.errorTryAgain);
                }
            });
        }
    });

    $('#resetform form').submit(function (evt) {
        evt.preventDefault();
        var $form = $(this);
        _totalDays = parseFloat($('#TotalDays').val());

        if ($form.valid()) {
            $.post(_global.root + 'ajax/ResetAnnualLeaves', $('form').serialize(), function (response) {
                if (response === -1) {
                    alert(_messages.invalidInputData);
                } else if (response) {
                    $('#resetform').dialog('close');
                } else {
                    alert(_messages.errorTryAgain);
                }
            });
        }
    });

    $('#cancel').click(function () {
        $('#insertform').dialog('close');
    });

    $('#cancelReset').click(function () {
        $('#resetform').dialog('close');
    });

    $('.aba').each(function () {
        if ($(this).prop('checked')) {
            $(this).closest('tr').addClass('row-selected');
        }
    });

    $('.aba').on('click', function () {
        var tr = $(this).closest('tr');

        if (tr.hasClass('row-selected')) {
            tr.removeClass();
        }
        else {
            tr.addClass('row-selected');
        }
    });

    $('.checkuncheckall').on('click', function () {
        _checked = !_checked;
        $('.aba').prop('checked', _checked);
        $('.checkuncheckall').prop('checked', _checked);

        if (_checked) {
            $('tbody tr').not('.row-divider').addClass('row-selected');
        } else {
            $('tbody tr').not('.row-divider').removeClass('row-selected');
        }
    });
});
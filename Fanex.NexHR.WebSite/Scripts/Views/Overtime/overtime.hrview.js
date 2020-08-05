'use strict';

var overtimeHrView = (function ($) {

    var domElement = {
        edit: '.btnEditNEX',
        confirmEdit: '.action-update',
        actionCancel: '.action-cancel',
        reset: '.btnReset'
    };

    function registerEditEvent() {
        $(domElement.edit).on('click', function () {
            let id = $(this).attr('data-id');
            $('#spanAdjustHour_' + id).hide();
            $('#inputAdjustHour_' + id).show();
            $('#inputAdjustHour_' + id).focus();
            $(this).hide();
            $('#' + id).find(domElement.confirmEdit).show();
            $('#' + id).find(domElement.actionCancel).show();
        });
    }

    function registerCancelEvent() {
        $(domElement.actionCancel).on('click', function () {
            let id = $(this).attr('data-id');
            resetStateInit(id);
        });
    }

    function registerConfirmEditEvent() {
        $(domElement.confirmEdit).on('click', function () {
            let id = $(this).attr('data-id');
            let OvertimeId = $(this).attr('data-overtimeId');
            let EmployeeId = $(this).attr('data-empId');
            let Date = $(this).attr('data-date');
            let AdjustHour = $('#inputAdjustHour_' + id).val();
            if (AdjustHour <= 0) {
                notifyError('Adjusted Hour must greater than 0');
                return;
            }
            confirm('Are you sure you want to adjust NEX Hour?', function () {
                $.ajax({
                    url: _global.root + 'Overtime/UpdateNexHour',
                    type: 'POST',
                    data: {
                        OvertimeId,
                        EmployeeId,
                        Date,
                        AdjustHour
                    },
                    success: function (response) {
                        $('#spanAdjustHour_' + id).html(AdjustHour);
                        resetStateInit(id);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        notifyError(textStatus + ': ' + errorThrown);
                    }
                });
            });
        });
    }

    function registerResetEvent() {
        $(domElement.reset).on('click', function () {
            let id = $(this).attr('data-overtimeId');
            confirm('Are you sure you want to reset request to pending?', function () {
                $.ajax({
                    url: _global.root + 'Overtime/DisapproveRequest',
                    type: 'POST',
                    data: { id },
                    success: function (response) {
                        query(false);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        notifyError(textStatus + ': ' + errorThrown);
                    }
                });
            });
        });
    }

    function resetStateInit(rowId) {
        $('#spanAdjustHour_' + rowId).show();
        $('#inputAdjustHour_' + rowId).hide();
        $('#' + rowId).find(domElement.actionCancel).hide();
        $('#' + rowId).find(domElement.confirmEdit).hide();
        $('#' + rowId).find(domElement.edit).show();
    }

    function query(isFirstLoad) {
        $.ajax({
            url: _global.root + 'Overtime/HRView',
            type: 'POST',
            dataType: 'html',
            data: $('.card-container-base .form form').serialize(),
            beforeSend: function () {
                if (!isFirstLoad) {
                    $('.loading-process').fadeIn(300);
                }
            },
            success: function (response) {
                $('table#data tbody').html(response);
                floatThead('table#data');
                registerEditEvent();
                registerCancelEvent();
                registerConfirmEditEvent();
                registerResetEvent();
            },
            error: function () {
                notifyError(_messages.errorTryAgain);
            },
            complete: function () {
                if (!isFirstLoad) {
                    $('.loading-process').fadeOut(300);
                }
            }
        });
    }

    function registerSubmitEvent() {
        $('#submit').on('click', function () {
            query(false);
        });
    }

    function initDateChangeEvent() {
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
    }

    function init() {
        query(true);
        initAutocomplete('#Employee', '#EmployeeId');
        initAutocomplete('#CreatedName', '#CreatedBy');
        initDateChangeEvent();
        registerSubmitEvent();
    }

    return {
        init
    }
})(jQuery);

$(document).ready(function () {
    overtimeHrView.init();
});
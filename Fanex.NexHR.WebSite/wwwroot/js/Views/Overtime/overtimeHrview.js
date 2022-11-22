'use strict';

var overtimeHrView = (function ($) {

    var domElement = {
        edit: '.btnEditNEX',
        confirmEdit: '.action-update',
        actionCancel: '.action-cancel',
        reset: '.btnReset',
        contextmenu: '.contextmenu',
        dialog: '#dialog-NEX-detail',
        tableDetail: '#data-detail'
    };

    let projectName = '';

    function registerContextMenuEvent() {
        let contextmenu = $(domElement.contextmenu);
        contextmenu.mouseover(function () {
            $(this).find(".drop-menu").show();
        });

        contextmenu.mouseout(function () {
            $(this).find(".drop-menu").hide();
        });

        contextmenu.click(function () {
            const id = $(this).find('a').attr('requestid');
            projectName = $(this).find('a').attr('projectName');
            getDetailProject(id);
        });
    }

    function bindApproveMenu() {
        var element = ".Approve";
        $.contextMenu({
            selector: element,
            trigger: 'left',
            callback: function (key, options) {
                processRequest(key, options);
            },
            items: {
                "verify": { name: "Verify", icon: "approve" },
                "void": { name: "Void", icon:"reject" }
            }
        });
    }

    function bindVerifiedMenu() {
        var element = ".Verified";
        $.contextMenu({
            selector: element,
            trigger: 'left',
            callback: function (key, options) {
                processRequest(key, options);
            },
            items: {
                "verify": { name: "Verify", icon: "approve" },
                "void": { name: "Void", icon: "reject" }
            }
        });
    }

    function bindVoidMenu() {
        var element = ".Void";
        $.contextMenu({
            selector: element,
            trigger: 'left',
            callback: function (key, options) {
                processRequest(key, options);
            },
            items: {
                "verify": { name: "Verify", icon: "approve" }
            }
        });
    }

    function processRequest(command, eventElement) {
        var requestId = eventElement.$trigger.attr('requestId');
        projectName = eventElement.$trigger.attr('projectName');
        switch (command) {
            case 'verify':
                getDetailProject(requestId);
                break;
            case 'void':
                voidRequest(requestId);
                break;
            default: break;
        }
    }

    function registerEditEvent() {
        $(domElement.edit).on('click', function () {
            let id = $(this).attr('data-id');
            $('#spanNEXHour_' + id).hide();
            $('#inputNEXHour_' + id).show();
            const value = $('#inputNEXHour_' + id).val();
            $('#inputNEXHour_' + id).focus().val('').val(value);
            $(this).hide();
            $('#' + id).find(domElement.confirmEdit).show();
            $('#' + id).find(domElement.actionCancel).show();
        });
    }

    function registerCancelEvent() {
        $(domElement.actionCancel).on('click', function () {
            let id = $(this).attr('data-id');
            resetStateInit(id);
            const value = +$('#spanNEXHour_' + id).text().trim();
            $('#inputNEXHour_' + id).val(value);
        });
    }

    function registerConfirmEditEvent() {
        $(domElement.confirmEdit).on('click', function () {
            let id = $(this).attr('data-id');
            let AdjustHour = $('#inputNEXHour_' + id).val();
            if (AdjustHour < 0 || isNaN(AdjustHour)) {
                notifyError('NEX Hour must greater than or equal to 0');
                return;
            }
            $('#spanNEXHour_' + id).html(AdjustHour);
            resetStateInit(id);
        });
    }

    function registerResetEvent() {
        $(domElement.reset).on('click', function () {
            let id = $(this).attr('requestid');
            confirm('Are you sure you want to reset request to pending?', function () {
                $.ajax({
                    url: _global.root + 'Overtime/ResetRequest',
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
        $('#spanNEXHour_' + rowId).show();
        $('#inputNEXHour_' + rowId).hide();
        $('#' + rowId).find(domElement.actionCancel).hide();
        $('#' + rowId).find(domElement.confirmEdit).hide();
        $('#' + rowId).find(domElement.edit).show();
    }

    function submitVerifyRequest() {
        const overtimeId = $(domElement.tableDetail).find('tr.row-detail').first().attr('overtimeId');
        const TimeSheetData = getTimeSheetData();
        $.ajax({
            type: 'POST',
            url: _global.root + 'Overtime/VerifyRequest',
            data: {
                Id: overtimeId,
                Description: projectName,
                TimeSheetData
            },
            success: function (data) {
                if (data) {
                    let errorCont = $('#divErrorCont');
                    errorCont.removeClass();
                    errorCont.html('');

                    if (data.IsSuccessful) {
                        $(domElement.dialog).dialog('close');
                        query(false);
                    }
                    else {
                        var errors = '<ul><li>' + data.Message + '</li></ul>';
                        errorCont.addClass('validation-summary-errors');
                        errorCont.append(errors);
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                notifyError(textStatus + ': ' + errorThrown);
            }
        });
    }

    function initDialogDetail() {
        $(domElement.dialog).dialog({
            modal: true,
            width: 'auto',
            buttons: [
                {
                    text: 'Close',
                    class: 'btn-cancel waves-effect waves-light m-r--m',
                    click: function () {
                        $(this).dialog('close');
                    }
                },
                {
                    text: 'Submit',
                    class: 'btn-submit waves-effect waves-light',
                    click: function () {
                        confirm('Are you sure you want to save this request?', function () {
                            submitVerifyRequest();
                        });
                    }
                }
            ],
            show: {
                effect: 'fade',
                duration: 300
            },
            hide: {
                effect: 'fade',
                duration: 200
            }
        });
    }

    function getDetailProject(id) {
        $.ajax({
            url: _global.root + 'Overtime/HRViewDetail',
            type: 'POST',
            data: { id },
            success: function (response) {
                $(domElement.dialog).html(response);
                registerEditEvent();
                registerCancelEvent();
                registerConfirmEditEvent();
                initDialogDetail();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                notifyError(textStatus + ': ' + errorThrown);
            }
        });
    }

    function getTimeSheetData() {
        let NexDetails = $(domElement.tableDetail).find('tr.row-detail');
        if (NexDetails.length > 0) {
            let timeRecords = [];
            for (var i = 0; i < NexDetails.length; i++) {
                let empId = +$(NexDetails[i]).attr('empId');
                let date = $(NexDetails[i]).attr('dateOT');
                let nexHour = +$(NexDetails[i]).find('td.nex-hour input.value-nex-hour').val();
                let hour = +$(NexDetails[i]).attr('hour');
                timeRecords.push({
                    Employee: {
                        Id: empId
                    },
                    Date: date,
                    NexHour: nexHour,
                    Hour: hour
                });
            }
            return JSON.stringify(timeRecords);
        } else {
            return "";
        }
    }

    function voidRequest(id) {
        confirm('Are you sure you want to void this request?', function () {
            $.ajax({
                url: _global.root + 'Overtime/VoidRequest',
                type: 'POST',
                data: { id },
                success: function (response) {
                    if (response.IsSuccessful) {
                        query(false);
                    } else {
                        notifyError(response.Message);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    notifyError(textStatus + ': ' + errorThrown);
                }
            });
        });
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
                registerContextMenuEvent();
                registerResetEvent();
                bindApproveMenu();
                bindVerifiedMenu();
                bindVoidMenu();
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
        initAutocomplete('#CreatedName', '#CreatedBy', false);
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
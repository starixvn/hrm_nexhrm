'use strict';

var overtimeHrView = (function ($) {

    var domElement = {
        edit: '.btnEditNEX',
        confirmEdit: '.action-update',
        actionCancel: '.action-cancel',
        reset: '.btnReset',
        contextmenu: '.contextmenu',
        dialog: '#dialog-NEX-detail'
    };

    function registerContextMenuEvent() {
        let contextmenu = $(domElement.contextmenu);
        contextmenu.mouseover(function () {
            $(this).find(".drop-menu").show();
        });

        contextmenu.mouseout(function () {
            $(this).find(".drop-menu").hide();
        });

        contextmenu.click(function () {
            const id = contextmenu.find('a').attr('requestid');
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
            $('#inputNEXHour_' + id).focus();
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
                            console.log('submit');
                            $(domElement.dialog).dialog('close');
                        });
                        //$.ajax({
                        //    type: 'POST',
                        //    url: _global.root + 'toeic/updatefulltoeic',
                        //    dataType: 'json',
                        //    data: $('#frmUpdFullTOEIC').serialize(),
                        //    success: function (data) {
                        //        if (data) {
                        //            var errorCont = $('#divErrorCont');
                        //            errorCont.removeClass();
                        //            errorCont.html('');

                        //            if (data.IsSuccessful) {
                        //                errorCont.addClass('success validation-summary-success');
                        //                errorCont.append('Update TOEIC score for Employee successfully');
                        //                setTimeout(function () { $('#dialog-toeic').dialog("close"); query(); }, 3000);
                        //            }
                        //            else {
                        //                var errors = '<ul><li>' + data.Message + '</li></ul>';
                        //                errorCont.addClass('validation-summary-errors');
                        //                errorCont.append(errors);
                        //            }
                        //        }
                        //    },
                        //    error: function (jqXHR, textStatus, errorThrown) {
                        //        notifyError(textStatus + ': ' + errorThrown);
                        //    }
                        //});
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
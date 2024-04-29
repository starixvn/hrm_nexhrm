
'use strict';

function loadTeamSupervisor(isFirstLoad) {
    $.ajax({
        url: _global.root + 'SupervisorFlow/GetTeamSupervisor',
        dataType: 'html',
        method: "POST",
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#list-team').html(data);

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

function loadPersonalSupervisor(isFirstLoad) {
    $.ajax({
        url: _global.root + 'SupervisorFlow/GetPersonalSupervisor',
        dataType: 'html',
        method: "POST",
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#list-personal').html(data);
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

function showErrorMessage(message) {
    $('<div>' + '<p>' + message + '</p>').dialog({
        title: 'NexHRM',
        modal: true,
        resizable: false,
        buttons: [
            {
                text: 'OK',
                class: 'btn-ok waves-effect waves-light',
                click: function () {
                    $(this).dialog('close');
                    return true;
                }
            }
        ],
        show: 'fade',
        hide: 'fade',
        minHeight: 50,
        dialogClass: 'modal-shadow'
    });
}

function init() {
    loadTeamSupervisor(true);
}

var supervisor = (function ($) {
    var $dialog = $('#dialog-supervisor');

    function initializeDialog($form, title) {
        $('.select-2', $dialog).select2({ closeOnSelect: false });

        $dialog.dialog({
            modal: true,
            width: 'auto',
            title: title,
            buttons: [
                {
                    text: 'Cancel',
                    class: 'btn-cancel waves-effect waves-light m-r--m btn-dialog',
                    click: function () {
                        $(this).dialog('close');
                    }
                },
                {
                    text: 'Save',
                    class: 'btn-submit waves-effect waves-light btn-dialog',
                    click: function () {
                        $form.submit();
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
    function initializePersonalDialog($form, title) {
        $.validator.unobtrusive.parse($form);
        $('.select-2', $dialog).select2({ closeOnSelect: false });
        initAutocomplete('form #Employee', 'form #EmpId', false);

        $dialog.dialog({
            modal: true,
            width: 'auto',
            title: title,
            buttons: [
                {
                    text: 'Cancel',
                    class: 'btn-cancel waves-effect waves-light m-r--m btn-dialog',
                    click: function () {
                        $(this).dialog('close');
                    }
                },
                {
                    text: 'Save',
                    class: 'btn-submit waves-effect waves-light btn-dialog',
                    click: function () {
                        $form.submit();
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

    function initEditTeamDialog() {
        initializeDialog($('#edit-form'), 'Update Team\'s Approver');
    }

    function initCreateDialog() {
        initializePersonalDialog($('#create-form'), 'New Personal');
    }

    function initEditPersonalDialog() {
        initializeDialog($('#edit-form'), 'Update Employee\'s Approver');
    }

    function handleUpdateTeamSuccess(response) {
        let valid = $('.validation-summary-valid');
        if (response.IsSuccessful) {
            valid.addClass('success validation-summary-success');
            valid.append('Update successfully');
            setTimeout(function () { $dialog.dialog('close'); loadTeamSupervisor(false); }, 1000);
        } else {
            showErrorMessage(response.Message);
        }
    }

    function handleSuccess(response, successMessage) {
        let valid = $('.validation-summary-valid');
        if (response.IsSuccessful) {
            valid.addClass('success validation-summary-success');
            valid.append(successMessage);
            setTimeout(function () { $dialog.dialog('close'); loadPersonalSupervisor(false); }, 1000);
        } else {
            showErrorMessage(response.Message);
        }
    }

    function onUpdatePersonalSuccess(response) {
        handleSuccess(response, 'Update successfully');
    }

    function onInsertPersonalSuccess(response) {
        handleSuccess(response, 'Insert successfully');
    }

    return {
        init: init,
        onLoadingCreateFormSuccess: initCreateDialog,
        onLoadingEditTeamFormSuccess: initEditTeamDialog,
        onLoadingEditPersonalFormSuccess: initEditPersonalDialog,
        onInsertPersonalSuccess: onInsertPersonalSuccess,
        onUpdatePersonalSuccess: onUpdatePersonalSuccess,
        onUpdateTeamSuccess: handleUpdateTeamSuccess
    }

}(jQuery));

$(document).ready(function () {
    supervisor.init();
});
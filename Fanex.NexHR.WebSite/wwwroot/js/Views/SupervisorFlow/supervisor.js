
'use strict';

function query(isFirstLoad) {
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

function switchTabPersonal() {
    $.ajax({
        url: _global.root + 'SupervisorFlow/GetPersonalSupervisor',
        dataType: 'html',
        method: "POST",
        beforeSend: function () {
            $('.loading-process').fadeIn(300);
        },
        success: function (data) {
            $('#list-personal').html(data);
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
    query(true);
}

var supervisor = (function ($) {
    var $dialog = $('#dialog-supervisor');

    function initializeDialog($form, title) {
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

    function initEditDialog() {
        initializeDialog($('#edit-form'), 'Update Aprrover By');
    }

    function handleSuccess(response, successMessage) {
        let errorDiv = $('.validation-summary-errors');
        let valid = $('.validation-summary-valid');
        if (response.IsSuccessful) {
            if (errorDiv.length > 0) {
                errorDiv.remove();
                valid = $('<div class="validation-summary-valid"></div>').prependTo('#create-form');
            }
            valid.addClass('success validation-summary-success');
            valid.append(successMessage);
            setTimeout(function () { $dialog.dialog('close'); query(false); }, 1000);
        } else {
            showErrorMessage(response.Message);
        }
    }

    function onUpdateSuccess(response) {
        handleSuccess(response, 'Update successfully');
    }

    return {
        init: init,
        onLoadingEditFormSuccess: initEditDialog,
        onUpdateSuccess: onUpdateSuccess
    }

}(jQuery));

$(document).ready(function () {
    supervisor.init();
});
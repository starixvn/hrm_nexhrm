
'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Transfer/GetTransfers',
        dataType: 'html',
        method: "POST",
        data: {
            FromDate: $('#FromDate').val(),
            ToDate: $('#ToDate').val(),
            EmployeeId: $('#EmployeeId').val(),
            TeamId: $('#TeamId').val()
        },
        beforeSend: function () {
            if (!isFirstLoad) {
                $('.loading-process').fadeIn(300);
            }
        },
        success: function (data) {
            $('#list-transfer').html(data);

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

function deleteTransfer(id) {
    confirm('Are you sure you want to delete this transfer?', function () {
        $.post(_global.root + 'Transfer/Delete', { id: id },
            function (response) {
                if (response.IsSuccessful) {
                    $('#row' + id).remove();
                    autoIndex();
                }
                else {
                    alert(response.Message);
                }
            });
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
    $('#submit').on('click', function () {
        query(false);
    });

    query(true);
}

var transfer = (function ($) {
    var $dialog = $('#dialog-transfer');

    function initializeDialog($form, title) {
        $.validator.unobtrusive.parse($form);
        initDatePicker();
        initAutocomplete('form #Employee', 'form #EmployeeId', false);
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

    function initCreateDialog() {
        initializeDialog($('#create-form'), 'Create New Transfer');
    }

    function initEditDialog() {
        initializeDialog($('#edit-form'), 'Edit Transfer');
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

    function onInsertSuccess(response) {
        handleSuccess(response, 'Insert successfully');
    }

    function onUpdateSuccess(response) {
        handleSuccess(response, 'Update successfully');
    }

    return {
        init: init,
        onLoadingCreateFormSuccess: initCreateDialog,
        onLoadingEditFormSuccess: initEditDialog,
        onInsertSuccess: onInsertSuccess,
        onUpdateSuccess: onUpdateSuccess
    }

}(jQuery));

$(document).ready(function () {
    transfer.init();
});
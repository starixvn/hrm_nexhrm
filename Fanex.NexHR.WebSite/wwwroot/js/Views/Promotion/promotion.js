
'use strict';

function query(isFirstLoad) {
    $.ajax({
        url: _global.root + 'Promotion/GetPromotions',
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
            $('#list-promotion').html(data);

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

function deletePromotion(id) {
    confirm('Are you sure you want to delete this promotion?', function () {
        $.post(_global.root + 'Promotion/Delete', { id: id},
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

var promotion = (function ($) {
    var $dialog = $('#dialog-promotion');

    $.validator.addMethod("validateSpace", function (value) {
        return value.trim().length > 0; 
    }, "The {0} field is required.");

    function initializeDialog($form, title) {
        // $.validator.unobtrusive.parse($form);
        // $("#EmployeeId").rules("add", {
        //     validateSpace: "Employee"
        // });

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
        initializeDialog($('#create-form'), 'Create New Promotion');
    }
    
    function initEditDialog() {
        initializeDialog($('#edit-form'), 'Edit Promotion');
    }

    function onInsertSuccess(response) {
        let valid = $('.validation-summary-valid');
        if (response.IsSuccessful) {
            valid.addClass('success validation-summary-success');
            valid.append('Insert successfully');
            setTimeout(function () { $dialog.dialog("close"); query(false); }, 1000);
        } else {
            showErrorMessage(response.Message);
        }
    }

    function onUpdateSuccess(response) {
        let valid = $('.validation-summary-valid');
        if (response.IsSuccessful) {
            valid.addClass('success validation-summary-success');
            valid.append('Update successfully');
            setTimeout(function () { $dialog.dialog("close"); query(false); }, 1000);
        } else {
            showErrorMessage(response.Message);
        }
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
    promotion.init();
});
var conversationManagement = (function ($) {
    var $dialog;
    var $editForm;
    var $searchForm;
    var $createForm;

    function initializeValidation($form) {
        $.validator.unobtrusive.parse($form);

        var validator = $form.data("validator");
        validator.settings.ignore = null;

        validator.settings.highlight = function (element, errorClass, validClass) {
            if (element.type === "radio") {
                this.findByName(element.name).addClass(errorClass).removeClass(validClass);
            } else {
                var $element = $(element);
                $element.addClass(errorClass).removeClass(validClass);

                if ($element.hasClass('material-select')) {
                    $element.siblings('input.select-dropdown').addClass(errorClass);
                }
            }
        };

        validator.settings.unhighlight = function (element, errorClass, validClass) {
            if (element.type === "radio") {
                this.findByName(element.name).removeClass(errorClass).addClass(validClass);
            } else {
                var $element = $(element);
                $element.removeClass(errorClass).addClass(validClass);

                if ($element.hasClass('material-select')) {
                    $element.siblings('input.select-dropdown').removeClass(errorClass);
                }
            }
        };
    }

    function initEditDialog() {
        $('.select-2', $dialog).select2({ closeOnSelect: false });

        $editForm = $('#edit-form');
        initializeValidation($editForm);

        $dialog.dialog({
            modal: true,
            width: 'auto',
            title: 'Edit Scheduled Message',
            buttons: [
                {
                    text: 'Cancel',
                    class: 'btn-cancel waves-effect waves-light m-r--m',
                    click: function () {
                        $(this).dialog('close');
                    }
                },
                {
                    text: 'Save',
                    class: 'btn-submit waves-effect waves-light',
                    click: function () {
                        $editForm.submit();
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
        $('.select-2', $dialog).select2({ closeOnSelect: false });

        $createForm = $('#create-conversation-form');
        initializeValidation($createForm);

        $dialog.dialog({
            modal: true,
            width: 'auto',
            title: 'Create New Conversation',
            buttons: [
                {
                    text: 'Cancel',
                    class: 'btn-cancel waves-effect waves-light m-r--m',
                    click: function () {
                        $(this).dialog('close');
                    }
                },
                {
                    text: 'Save',
                    class: 'btn-submit waves-effect waves-light',
                    click: function () {
                        $createForm.submit();
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

    function initConfirmationDialog($form) {
        $dialog.html('Are you sure you want to remove this scheduled message?');
        $dialog.dialog({
            modal: true,
            width: 'auto',
            title: 'Remove Scheduled Message',
            buttons: [
                {
                    text: 'No',
                    class: 'btn-cancel waves-effect waves-light m-r--m',
                    click: function () {
                        $(this).dialog('close');
                    }
                },
                {
                    text: 'Yes',
                    class: 'btn-submit waves-effect waves-light',
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

    function init() {
        $dialog = $('#dialog');
        $searchForm = $('#search-form');
        $searchForm.submit();

        $('#conversation-table').on('click', 'a.remove-conversation', function (e) {
            e.preventDefault();
            var formId = $(this).data('form-target-id');
            initConfirmationDialog($(formId));
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

    return {
        init: init,
        onLoadingEditFormSuccess: initEditDialog,
        onLoadingCreateFormSuccess: initCreateDialog,
        onEditSuccess: function (response) {
            if (response.isSuccessful) {
                $dialog.dialog('close');
                $searchForm.submit();
            } else {
                if (response.errors) {
                    $editForm.validate().showErrors(response.errors);
                }

                if (response.message) {
                    showErrorMessage(response.message);
                }
            }
        },
        onRemoveSuccess: function (response) {
            if (response.isSuccessful) {
                $dialog.dialog('close');
                $searchForm.submit();
            }
        },
        onCreateSuccess: function (response) {
            if (response.isSuccessful) {
                $dialog.dialog('close');
            } else {
                if (response.errors) {
                    $createForm.validate().showErrors(response.errors);
                }
                if (response.message) {
                    showErrorMessage(response.message);
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    conversationManagement.init();
});
var scheduledMessage = (function ($) {
    var $dialog;
    var $createForm;
    var $editForm;
    var $searchForm;

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

                if ($element.hasClass('initialized')) {
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

                if ($element.hasClass('initialized')) {
                    $element.siblings('input.select-dropdown').removeClass(errorClass);
                }
            }
        };
    }

    function initCreateDialog() {
        $('select', $dialog).on('change', function () {
            $(this).valid();
        }).material_select();

        initDatePicker();

        $createForm = $('#create-form');
        //  initializeValidation($createForm);

        $dialog.dialog({
            modal: true,
            width: 'auto',
            title: 'Create New Scheduled Message',
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

    function initEditDialog() {
        $('select', $dialog).material_select();
        initDatePicker();
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
                        debugger;
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

        $('#scheduled-message-table').on('click', 'a.remove-scheduled-message', function (e) {
            e.preventDefault();
            var formId = $(this).data('form-target-id');
            initConfirmationDialog($(formId));
        });
    }

    return {
        init: init,
        onLoadingCreateFormSuccess: initCreateDialog,
        onLoadingEditFormSuccess: initEditDialog,
        onCreateSuccess: function (response) {
            if (response.isSuccessful) {
                $dialog.dialog('close');
                $searchForm.submit();
            } else {
                if (response.errors) {
                    $createForm.validate().showErrors(response.errors);
                }
            }
        },
        onEditSuccess: function (response) {
            if (response.isSuccessful) {
                $dialog.dialog('close');
                $searchForm.submit();
            } else {
                if (response.errors) {
                    $editForm.validate().showErrors(response.errors);
                }
            }
        },
        onRemoveSuccess: function (response) {
            if (response.isSuccessful) {
                $dialog.dialog('close');
                $searchForm.submit();
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    scheduledMessage.init();
});
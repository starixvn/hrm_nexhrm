var scheduledMessage = (function ($) {
    var $dialog;
    var $createForm;

    function initCreateDialog() {
        $('select', $dialog).material_select();
        initDatePicker();
        $createForm = $('#create-form');
        $.validator.unobtrusive.parse($createForm);

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

    function init() {
        $dialog = $('#dialog');
    }

    return {
        init: init,
        onLoadingCreateFormSuccess: initCreateDialog,
        onEditBioDeviceSuccess: function (response) {
            if (response.isSuccessful) {
                var deviceId = $('#DeviceId').val()
                $dialog.dialog('close');
                $('#reload-bio-' + deviceId).click();
            } else {
                if (response.errors) {
                    $editBioForm.validate().showErrors(response.errors);
                }
            }
        }
    }
}(jQuery));

$(document).ready(function () {
    scheduledMessage.init();
});
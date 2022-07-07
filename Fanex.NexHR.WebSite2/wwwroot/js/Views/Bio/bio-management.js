var bioManagement = (function ($) {
    var $dialog;
    var $editBioForm;

    function initDialog() {
        $('select', $dialog).material_select();
        $editBioForm = $('#edit-bio-form');
        $.validator.unobtrusive.parse($editBioForm);

        $dialog.dialog({
            modal: true,
            width: 'auto',
            title: 'Edit Bio Device #' + $('#DeviceId').val(),
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
                        $editBioForm.submit();
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
        onLoadingEditBioSuccess: initDialog,
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
    bioManagement.init();
});